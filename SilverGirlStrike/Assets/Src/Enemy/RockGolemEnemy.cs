using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//変更履歴
//2018/10/26 板倉（作成）

namespace RockGolem
{
    /*
            public int hitPoint;                        :　俗にいう生命力                      
            public int invincible;                      :　無敵時間
            public float moveSpeed;                     :　待機位置への移動速度                         
            public CharacterMover mover;                :　CharacterMoverへのアクセス
            public Animator animator;                   :　アニメーションへのアクセス
            public IdlePosition idlePositions;          :　待機位置がそれぞれ入っている（親からの相対位置）
            public NarrowAttacker[] attackCollisions;   :　攻撃コリジョンの配列
     */

    //ボスキャラ、ただの岩に魔法によって知能と生命を与えられた姿
    public class RockGolemEnemy : CharacterObject
    {
        //ステートの定数
        public enum State
        {
            MOVE,
            IDLE,
            TACKLE,
            DAMAGE,
        }

        //現在地を示す定数
        enum Place
        {
            CENTER,
            LEFTUP,
            RIGHTUP,
        }

        //ボスキャラの待機位置
        [System.Serializable]
        public class IdlePosition
        {
            public Vector3 center;
            public Vector3 rightUp;
            public Vector3 leftUp;
        }


        //Inspectorで値変えたい系の変数
        [System.Serializable]
        public class InspectorParam
        {
            public int hitPoint;
            public int invincible;
            public float moveSpeed;

            public CharacterMover mover;
            public Animator animator;
            public GameObject targetCharacter;
            public IdlePosition idlePositions;
            public NarrowAttacker[] attackCollisions;
        }


        //よそに渡す用のクラス
        //初期化はちゃんとStart関数でやってね（はあと
        public class Parameter
        {
            public GameObject myself;
            //毎移動時の移動ベクトル
            public Vector2 moveVector;

            //次にMoveステートに入った際の移動位置
            public Vector3 currentIdlePosition;
        }

        //メンバ変数
        public InspectorParam inspectorParam;
        public Parameter param;


        void Start()
        {
            GetData().hitPoint.SetMaxHP(this.inspectorParam.hitPoint);
            GetData().hitPoint.Recover(this.inspectorParam.hitPoint);
            GetData().hitPoint.SetInvincible(this.inspectorParam.invincible);


            this.param = new Parameter();
            this.param.myself = this.gameObject;
            this.param.moveVector = Vector2.zero;
            this.param.currentIdlePosition = this.inspectorParam.idlePositions.center;
            //ステート追加
            AddState((int)State.MOVE, new MoveState(this));
            AddState((int)State.IDLE, new IdleState(this));
            AddState((int)State.TACKLE, new ChasePress(this));
            ChangeState((int)State.MOVE);
        }

        public override void ApplyDamage()
        {
            GetData().hitPoint.DamageUpdate();
        }

        public override void Damage(AttackData attackData)
        {
            GetData().hitPoint.Damage(attackData.power, attackData.chain);
        }

        public override void MoveCharacter()
        {
            this.inspectorParam.mover.UpdateVelocity(this.param.moveVector.x, this.param.moveVector.y, 0.0f, false);
            this.param.moveVector = Vector3.zero;
        }

        public override void UpdateCharacter()
        {
            UpdateState();
            //判定垂れ流し
            this.inspectorParam.attackCollisions[0].StartAttack();

            //無敵時間中は色を変える
            if (GetData().hitPoint.IsInvincible())
            {
                GetComponent<SpriteRenderer>().color = new Color(0.9f, 0.5f, 0.5f, 1.0f);
            }
            else
            {
                GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
        }

        //ステート
        public abstract class BaseState : StateParameter
        {
            RockGolemEnemy param;

            public BaseState(RockGolemEnemy param)
            {
                this.param = param;
            }

            //メソッドだけ追加
            public RockGolemEnemy.Parameter GetParam()
            {
                return param.param;
            }
            public RockGolemEnemy.InspectorParam GetInspectorParam()
            {
                return param.inspectorParam;
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////
        //所定位置への移動
        class MoveState : BaseState
        {

            //めんば
            Vector2 targetDirection;
            Vector3 targetPos;


            public MoveState(RockGolemEnemy param) : base(param)
            {
            }

            public override void Enter(ref StateManager manager)
            {
                this.targetPos = GetParam().currentIdlePosition;

                this.targetDirection = new Vector2(
                    this.targetPos.x - GetParam().myself.transform.localPosition.x,
                    this.targetPos.y - GetParam().myself.transform.localPosition.y);

                //速度をかけるため正規化
                this.targetDirection.Normalize();
            }

            public override void Exit(ref StateManager manager)
            {
            }

            public override bool Transition(ref StateManager manager)
            {
                //進行方向と、現在のアンカーへ向かうベクトルを比較し、後ろにあったら待機に移行
                float dot = Vector2.Dot(this.targetDirection, this.targetPos - GetParam().myself.transform.localPosition);

                if (dot < 0.0f || this.targetDirection.magnitude < 0.01f)
                {
                    manager.SetNextState((int)RockGolemEnemy.State.IDLE);
                    return true;
                }
                return false;
            }

            public override void Update()
            {
                //移動先への移動
                GetParam().moveVector += this.targetDirection * GetInspectorParam().moveSpeed;
            }
        }
        /////////////////////////////////////////////////////////////////////////////////////
        //待機
        class IdleState : BaseState
        {

            public IdleState(RockGolemEnemy param) : base(param)
            {
            }

            public override void Enter(ref StateManager manager)
            {
                ResetTime();
            }

            public override void Exit(ref StateManager manager)
            {
            }

            public override bool Transition(ref StateManager manager)
            {
                if (GetTime() > 120)
                {
                    //移動先を確定
                    manager.SetNextState((int)RockGolemEnemy.State.TACKLE);
                    return true;
                }
                if (GetTime() > 120)
                {
                    //ランダムで移動先を決定（今の位置と違うものだけ）
                    Vector3 movePosition = GetParam().currentIdlePosition;
                    do
                    {
                        var idlePos = (RockGolemEnemy.Place)Random.Range(0, 3);
                        switch (idlePos)
                        {
                            case Place.CENTER:
                                {
                                    movePosition = GetInspectorParam().idlePositions.center;
                                    break;
                                }
                            case Place.LEFTUP:
                                {
                                    movePosition = GetInspectorParam().idlePositions.leftUp;
                                    break;
                                }
                            case Place.RIGHTUP:
                                {
                                    movePosition = GetInspectorParam().idlePositions.rightUp;
                                    break;
                                }
                        }
                    } while (GetParam().currentIdlePosition == movePosition);

                    //移動先を確定
                    GetParam().currentIdlePosition = movePosition;
                    manager.SetNextState((int)RockGolemEnemy.State.MOVE);
                    return true;
                }



                return false;
            }

            public override void Update()
            {
                //待機モーション中に、緩やかに位置がグルグル回る
                GetParam().moveVector = new Vector2(Mathf.Sin((GetTime() * 0.8f) * Mathf.Deg2Rad) * 0.1f, Mathf.Sin((GetTime() * 2.0f) * Mathf.Deg2Rad));
            }
        }
        /////////////////////////////////////////////////////////////////////////////////////
        //右か左に体当たり(プレイヤー追従)
        class Tackle1State : BaseState
        {
            enum Direction
            {
                LEFT,
                RIGHT,
            }

            //メンバー
            Vector2 attackDirection;

            public Tackle1State(RockGolemEnemy param) : base(param)
            {
            }

            public override void Enter(ref StateManager manager)
            {
                //タックル方向を決定
                Direction direction = (Direction)Random.Range(0, 2);

                switch (direction)
                {
                    case Direction.LEFT:
                        {
                            this.attackDirection = Vector2.left;
                        }
                        break;
                    case Direction.RIGHT:
                        {
                            this.attackDirection = Vector2.right;
                        }
                        break;
                }
            }

            public override void Exit(ref StateManager manager)
            {
            }

            public override bool Transition(ref StateManager manager)
            {

                return false;
            }

            public override void Update()
            {
                //突進
                GetParam().moveVector += this.attackDirection;
            }
        }
        /////////////////////////////////////////////////////////////////////////////////////
        //右か左に体当たり(プレイヤー追従)
        class ChasePress : BaseState
        {
            float fallVelocity;


            public ChasePress(RockGolemEnemy param) : base(param)
            {
            }

            public override void Enter(ref StateManager manager)
            {
                fallVelocity = -10.0f;
            }

            public override void Exit(ref StateManager manager)
            {
            }

            public override bool Transition(ref StateManager manager)
            {
                if (GetTime() > 260)
                {
                    //移動先を確定
                    Vector3 movePosition = GetInspectorParam().idlePositions.center;
                    GetParam().currentIdlePosition = movePosition;
                    manager.SetNextState((int)RockGolemEnemy.State.MOVE);
                    return true;
                }
                return false;
            }

            public override void Update()
            {
                if (GetTime() < 100)
                {
                    //プレイヤー上空でX追尾
                    const float movePower = 5.0f;
                    GetParam().moveVector += new Vector2(0.0f, (GetInspectorParam().idlePositions.leftUp.y - this.GetParam().myself.transform.localPosition.y) * movePower);
                    GetParam().moveVector += new Vector2((GetInspectorParam().targetCharacter.transform.position.x - this.GetParam().myself.transform.position.x) * movePower, 0.0f);
                }
                else if(GetTime() < 180)
                {
                    GetParam().moveVector += Vector2.down * this.fallVelocity;
                    this.fallVelocity += 1.4f;
                }
                else
                {
                    //プレイヤー上空でX追尾
                    const float movePower = 3.0f;
                    GetParam().moveVector += new Vector2(0.0f, (GetInspectorParam().idlePositions.leftUp.y - this.GetParam().myself.transform.localPosition.y) * movePower);
                }


            }
        }
    }
}

