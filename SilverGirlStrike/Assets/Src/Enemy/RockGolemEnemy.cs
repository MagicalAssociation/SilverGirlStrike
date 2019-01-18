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
            START,
            MOVE,
            IDLE,
            TACKLE,
            TACKLE2,
            PRESS,

            STONE_BLAST,

            DEATH,
        }

        //現在地を示す定数
        enum Place
        {
            CENTER,
            LEFTUP,
            RIGHTUP,
        }

        public enum Direction
        {
            LEFT,
            RIGHT,
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

            public Collider2D hitRigidCollision;
            public CharacterMover mover;
            public Animator animator;
            public CharacterObject targetCharacter;
            public IdlePosition idlePositions;
            public NarrowAttacker[] attackCollisions;
            public Bullet.BulletData[] bulletDatas;
        }


        //よそに渡す用のクラス
        //初期化はちゃんとStart関数でやってね（はあと
        public class Parameter
        {
            public CharacterObject myself;
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
            this.param.myself = this;
            this.param.moveVector = Vector2.zero;
            this.param.currentIdlePosition = this.inspectorParam.idlePositions.center;
            //ステート追加
            AddState((int)State.START, new StartState(this));
            AddState((int)State.MOVE, new MoveState(this));
            AddState((int)State.IDLE, new IdleState(this));
            AddState((int)State.TACKLE, new Tackle1State(this));
            AddState((int)State.TACKLE2, new Tackle2State(this));
            AddState((int)State.PRESS, new ChasePress(this));
            AddState((int)State.STONE_BLAST, new StoneBlast(this));
            AddState((int)State.DEATH, new DeathState(this));
            ChangeState((int)State.START);
        }

        public override void ApplyDamage()
        {
            GetData().hitPoint.DamageUpdate();
        }

        public override bool Damage(AttackData attackData)
        {
            return GetData().hitPoint.Damage(attackData.power, attackData.chain);
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
            if (GetData().stateManager.GetNowStateNum() != (int)State.START)
            {
                this.inspectorParam.attackCollisions[0].StartAttack();
            }

            if (GetData().stateManager.GetNowStateNum() != (int)State.DEATH)
            {
                //死亡条件はHPが０になること
                if (GetData().hitPoint.GetHP() == 0)
                {
                    GetData().stateManager.ChengeState((int)State.DEATH);
                }
            }

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

        public override void Dispose()
        {
            Destroy(this.transform.parent.gameObject);
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


            //ランダムで移動先を決定（今の位置と違うものだけ）
            public void SetIdlePositionRandom()
            {
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
            }


            //プレイヤーが右にいるか左にいるかを判定
            public Direction CheckDirectionToTarget()
            {
                var targetPos = GetInspectorParam().targetCharacter.transform.position;
                //ターゲットが左にいる
                if (this.GetParam().myself.transform.position.x > targetPos.x)
                {
                    return Direction.LEFT;
                }
                else
                {
                    return Direction.RIGHT;
                }
            }

            public Vector3 SetDirectionWithDirection(Direction direction)
            {
                //向きに合わせて方向決定
                switch (direction)
                {
                    case Direction.LEFT:
                        {
                            return Vector2.left;
                        }
                    case Direction.RIGHT:
                        {
                            return Vector2.right;
                        }
                    default:
                        return Vector2.zero;
                }
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////
        //登場演出
        class StartState : BaseState
        {
            int count;
            bool isActive;

            float movePower;


            public StartState(RockGolemEnemy param) : base(param)
            {
            }

            public override void Enter(ref StateManager manager)
            {
                GetInspectorParam().hitRigidCollision.isTrigger = true;
                GetParam().myself.transform.position += Vector3.down * 8.0f;
                GetParam().myself.transform.position += Vector3.forward;
                this.count = 0;
                this.isActive = false;
                this.movePower = 45.0f;
            }

            public override void Exit(ref StateManager manager)
            {
                GetInspectorParam().hitRigidCollision.isTrigger = false;
                GetParam().myself.transform.position += -Vector3.forward;
            }

            public override bool Transition(ref StateManager manager)
            {
                if(this.count > 160)
                {
                    manager.SetNextState((int)RockGolemEnemy.State.IDLE);
                    return true;
                }
                return false;
            }

            public override void Update()
            {
                if (!CheckActive())
                {
                    //有効じゃないなら何もしない
                    return;
                }
                ++this.count;

                if(this.count < 120 || this.count > 160)
                {
                    return;
                }

                //ジャンプのような飛び出し挙動
                GetParam().moveVector = Vector3.up * this.movePower;
                this.movePower += -1.6f;
            }


            bool CheckActive()
            {
                //一度有効になったらもう止まらない
                if (this.isActive)
                {
                    return true;
                }

                //プレイヤーが死んでいるときはもう関係ない
                if(GetInspectorParam().targetCharacter == null)
                {
                    return false;
                }
                if (GetInspectorParam().targetCharacter != null && GetInspectorParam().targetCharacter.IsDead())
                {
                    GetInspectorParam().targetCharacter = null;
                    return false;
                }


                float distance = Vector3.Distance(GetInspectorParam().targetCharacter.transform.position, GetParam().myself.transform.position);
                if (distance < 6.0f)
                {
                    isActive = true;
                    return true;
                }
                return false;
            }
        }
        /////////////////////////////////////////////////////////////////////////////////////
        //所定位置への移動
        class MoveState : BaseState
        {

            //めんば
            Vector2 targetDirection;
            Vector3 targetPos;
            float power;


            public MoveState(RockGolemEnemy param) : base(param)
            {
            }

            public override void Enter(ref StateManager manager)
            {
                this.power = -1.0f;
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
                //進行方向と、目的地へ向かうベクトルを比較し、後ろにあったら待機に移行
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
                this.power += 0.1f;
                if (this.power > 1.0f)
                {
                    this.power = 1.0f;
                }
                //移動先への移動
                GetParam().moveVector += this.targetDirection * GetInspectorParam().moveSpeed * this.power;
            }
        }
        /////////////////////////////////////////////////////////////////////////////////////
        //待機
        class IdleState : BaseState
        {
            int stateEnterCount;


            public IdleState(RockGolemEnemy param) : base(param)
            {
                this.stateEnterCount = 0;
            }

            public override void Enter(ref StateManager manager)
            {

            }

            public override void Exit(ref StateManager manager)
            {
            }

            public override bool Transition(ref StateManager manager)
            {
                if (GetTime() < 1)
                {
                    ++this.stateEnterCount;
                    //三かい待機に来たら、移動ステートを挟む
                    if (this.stateEnterCount >= 3)
                    {
                        this.stateEnterCount = 0;
                    }
                }

                if (GetTime() > 100 && this.stateEnterCount == 2)
                {
                    SetIdlePositionRandom();
                    manager.SetNextState((int)RockGolemEnemy.State.MOVE);
                    return true;
                }

                if (GetTime() > 100)
                {
                    //移動先を確定
                    int randomNumber = Random.Range(0, 3);

                    switch (randomNumber)
                    {
                        case 0:
                            {
                                manager.SetNextState((int)RockGolemEnemy.State.PRESS);
                                break;
                            }
                        case 1:
                            {
                                manager.SetNextState((int)RockGolemEnemy.State.TACKLE);
                                break;
                            }
                        case 2:
                            {
                                manager.SetNextState((int)RockGolemEnemy.State.STONE_BLAST);
                                break;
                            }
                        default:
                            break;
                    }

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


            //メンバー
            Vector2 attackDirection;
            float power;

            public Tackle1State(RockGolemEnemy param) : base(param)
            {
            }

            public override void Enter(ref StateManager manager)
            {
                this.power = 0.0f;


                //タックル方向を決定
                Direction direction = CheckDirectionToTarget();


                this.attackDirection = SetDirectionWithDirection(direction);
                Sound.PlaySE("charge1");
            }

            public override void Exit(ref StateManager manager)
            {
            }

            public override bool Transition(ref StateManager manager)
            {
                //ちょうど画面外に出るときに待機位置へ戻る
                if (GetParam().myself.transform.localPosition.x < -16.0f || GetParam().myself.transform.localPosition.x > 16.0f)
                {
                    //移動先を確定
                    SetIdlePositionRandom();
                    manager.SetNextState((int)RockGolemEnemy.State.TACKLE2);
                    return true;
                }
                return false;
            }

            public override void Update()
            {
                if (this.power > 2.0f)
                {
                    //判定垂れ流し
                    GetInspectorParam().attackCollisions[1].StartAttack();
                }
                //突進
                GetParam().moveVector += this.attackDirection * this.power;
                this.power += 1.0f;
            }

        }
        /////////////////////////////////////////////////////////////////////////////////////
        //プレス攻撃(プレイヤー追従)
        class ChasePress : BaseState
        {
            float fallVelocity;
            float movePower;
            const int pressWaitTime = 100;
            const int pressTime = 30;
            const int finishWaitTime = 12;

            public ChasePress(RockGolemEnemy param) : base(param)
            {
            }

            public override void Enter(ref StateManager manager)
            {
                this.fallVelocity = -16.0f;
                this.movePower = 0.0f;
            }

            public override void Exit(ref StateManager manager)
            {
            }

            public override bool Transition(ref StateManager manager)
            {
                if (GetTime() > pressWaitTime + pressTime + finishWaitTime + 20)
                {
                    //移動先を確定
                    Vector3 movePosition = GetInspectorParam().idlePositions.center;
                    GetParam().currentIdlePosition = movePosition;
                    manager.SetNextState((int)RockGolemEnemy.State.TACKLE);
                    return true;
                }
                return false;
            }

            public override void Update()
            {
                if (GetTime() < pressWaitTime)
                {
                    this.movePower = (float)GetTime() / pressWaitTime * 5.0f;
                    //プレイヤー上空でX追尾
                    GetParam().moveVector += new Vector2(0.0f, (GetInspectorParam().idlePositions.leftUp.y - this.GetParam().myself.transform.localPosition.y) * this.movePower);
                    GetParam().moveVector += new Vector2((GetInspectorParam().targetCharacter.transform.position.x - this.GetParam().myself.transform.position.x) * this.movePower, 0.0f);

                }
                else if (GetTime() < pressWaitTime + pressTime)
                {
                    //落下攻撃
                    GetParam().moveVector += Vector2.down * this.fallVelocity;
                    this.fallVelocity += 2.0f;
                }
                else if (GetTime() > pressWaitTime + pressTime + finishWaitTime)
                {

                    //ちょっと上がる
                    GetParam().moveVector += Vector2.up * 4.0f;
                }

                //着地
                if (GetTime() == pressWaitTime + pressTime - 1)
                {
                    Sound.PlaySE("press1");
                }
            }
        }
        /////////////////////////////////////////////////////////////////////////////////////
        //サインタックル(プレイヤー追従)
        class Tackle2State : BaseState
        {
            //メンバー
            Vector2 attackDirection;
            float power;
            Direction direction;

            public Tackle2State(RockGolemEnemy param) : base(param)
            {
            }

            public override void Enter(ref StateManager manager)
            {
                //タックル方向を決定
                this.direction = CheckDirectionToTarget();
                this.attackDirection = SetDirectionWithDirection(direction);
                this.power = 5.0f;
            }

            public override void Exit(ref StateManager manager)
            {
            }

            public override bool Transition(ref StateManager manager)
            {
                //ちょうど向こう側の画面外に出るときに待機位置へ戻る
                if (GetParam().myself.transform.localPosition.x > 16.0f && this.direction == Direction.RIGHT)
                {
                    //移動先を確定
                    SetIdlePositionRandom();
                    manager.SetNextState((int)RockGolemEnemy.State.MOVE);
                    return true;
                }
                //ちょうど向こう側の画面外に出るときに待機位置へ戻る
                if (GetParam().myself.transform.localPosition.x < -16.0f && this.direction == Direction.LEFT)
                {
                    //移動先を確定
                    SetIdlePositionRandom();
                    manager.SetNextState((int)RockGolemEnemy.State.MOVE);
                    return true;
                }
                return false;
            }

            public override void Update()
            {
                //判定垂れ流し
                GetInspectorParam().attackCollisions[1].StartAttack();
                //突進
                GetParam().moveVector += this.attackDirection * this.power;
                GetParam().moveVector += Vector2.up * Mathf.Sin((float)(GetTime() - 10) * Mathf.Deg2Rad * 8.0f) * 28.0f;
                this.power += 0.03f;

                if (GetTime() < 10)
                {
                    //床につくようにめっちゃ落下
                    GetParam().moveVector = Vector2.down * 200.0f;
                }
            }



        }
        /////////////////////////////////////////////////////////////////////////////////////
        //プレイヤーに向けて石を飛ばす
        class StoneBlast : BaseState
        {
            const int interval = 5;
            const int bulletCount = 10;
            int bulletCurrentCount;


            public StoneBlast(RockGolemEnemy param) : base(param)
            {
            }

            public override void Enter(ref StateManager manager)
            {
                this.bulletCurrentCount = 0;
            }

            public override void Exit(ref StateManager manager)
            {
            }

            public override bool Transition(ref StateManager manager)
            {
                //打ち終わったら遷移
                if (GetTime() == 60 + bulletCount * interval)
                {
                    manager.SetNextState((int)RockGolemEnemy.State.IDLE);
                    return true;
                }
                return false;
            }

            public override void Update()
            {
                //待機モーション中に、緩やかに位置がグルグル回る
                GetParam().moveVector += new Vector2(Mathf.Cos(GetTime() * Mathf.Deg2Rad * 15.0f), Mathf.Sin(GetTime() * Mathf.Deg2Rad * 15.0f)) * 3.0f;
                if (GetTime() == 60 + this.bulletCurrentCount * interval)
                {
                    //発射
                    Sound.PlaySE("shot2");
                    GetInspectorParam().bulletDatas[0].target = GetInspectorParam().targetCharacter.gameObject;
                    Bullet.MagicBullet.Create(GetParam().myself, GetInspectorParam().bulletDatas[0], GetParam().myself.transform.position);
                    this.bulletCurrentCount += 1;
                }
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////
        //死亡(プレイヤー追従)
        class DeathState : BaseState
        {
            //メンバー
            const int burstStartTime = 120;
            const int burstTime = 120;

            public DeathState(RockGolemEnemy param) : base(param)
            {
            }

            public override void Enter(ref StateManager manager)
            {
                GetParam().myself.GetData().hitPoint.SetDamageShutout(true);

                Sound.StopBGM();
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

                GetParam().myself.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.0f, 0.0f, 1.0f);


                if (base.GetTime() % 5 == 0 && base.GetTime() >= burstStartTime && base.GetTime() <= burstStartTime + burstTime)
                {
                    Sound.PlaySE("bombSmall");
                    Vector3 randomMove = new Vector3(Random.Range(0.0f, 2.0f) - 1.0f, Random.Range(0.0f, 2.0f) - 1.0f, 0.0f);
                    Effect.Get().CreateEffect("manyBombs", GetParam().myself.transform.position - Vector3.forward + randomMove, Quaternion.identity, Vector3.one * 2);
                }

                if(base.GetTime() > burstStartTime + burstTime)
                {
                    Sound.PlaySE("slashFlash");
                    Effect.Get().CreateEffect("defeat", GetParam().myself.transform.position - Vector3.forward, Quaternion.identity, Vector3.one);
                    GetParam().myself.KillMyself();
                }
            }

        }

    }
}

