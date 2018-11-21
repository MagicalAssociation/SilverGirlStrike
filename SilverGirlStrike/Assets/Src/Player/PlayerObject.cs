using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//2018/11/20 板倉 : 作成
//2018/11/21 板倉 : 内部実装

namespace Fuchan
{

    //新仕様のステート管理システムに対応したプレイヤーキャラ
    [System.Serializable]
    public class PlayerObject : CharacterObject
    {
        public enum Direction
        {
            LEFT,
            RIGHT,
        }
        //ステートの定数
        public enum State
        {
            IDLE,
            WALK,
            JUMP,
            FALL,
            WIRE_DASH,
            ATTACK1,
            ATTACK2,
            ATTACK3,
            JUMP_ATTACK1,
            JUMP_ATTACK2,
            JUMP_ATTACK3,
        }

        //Inspectorで値変えたい系の変数
        [System.Serializable]
        public class InspectorParam
        {
            public float gravity;
            public float speed;
            public float anchorDashSpeed;
            public float anchorMoveAcceleration;
            public float jumpPower;
            public Foot footObject;
            public CharacterMover mover;
            public AnchorSelector anchor;
            public Animator playerAnim;
        }


        //よそに渡す用のクラス
        //初期化はちゃんとStart関数でやってね（はあと
        public class Parameter
        {
            public GameObject myself;
            //毎移動時の移動ベクトル
            public Vector2 moveVector;
            //接地フラグ
            public bool onGround;
            //キャラ向き
            public Direction direction;
            //現在のアンカーダッシュスピード
            public float currentDashSpead;

            //アンカーショットターゲット
            public GameObject anchorTarget;
        }

        public InspectorParam inspectorParam;
        public Parameter param;

        private void Start()
        {
            this.param = new Parameter();
            this.param.myself = this.gameObject;
            this.param.moveVector = Vector2.zero;
            this.param.direction = Direction.RIGHT;
            this.param.currentDashSpead = 0.0f;
            this.param.anchorTarget = null;
            //ステート追加
            AddState((int)State.IDLE, new IdleState(this));
            AddState((int)State.WALK, new WalkState(this));
            AddState((int)State.JUMP, new JumpState(this));
            AddState((int)State.FALL, new FallState(this));
            AddState((int)State.WIRE_DASH, new AnchorDashState(this));
            ChangeState((int)State.IDLE);
        }

        //メンバ関数
        PlayerObject()
        {

        }
        public override void ApplyDamage()
        {
        }

        public override void Damage(AttackData attackData)
        {
        }

        public override void MoveCharacter()
        {
            this.inspectorParam.mover.UpdateVelocity(this.param.moveVector.x, this.param.moveVector.y, this.inspectorParam.gravity, this.param.onGround);
            //移動が終わったら移動ベクトルを初期化
            this.param.moveVector = Vector2.zero;
        }

        public override void UpdateCharacter()
        {
            //接地判定
            CheckGround();

            //ステート処理
            UpdateState();
        }

        //接地判定
        void CheckGround()
        {
            this.param.onGround = this.inspectorParam.footObject.CheckHit();
        }


    }


    //プレイヤーのステート
    //・待機
    //・走り
    //・ジャンプ
    //・対空
    //・ワイヤー
    //・攻撃（１・２・３）
    //・空中攻撃（１・２・３）
    //・ダメージ
    //・死亡
    //・登場

    public abstract class BaseState : StateParameter
    {
        PlayerObject param;

        public BaseState(PlayerObject param)
        {
            this.param = param;
        }

        //メソッドだけ追加
        public PlayerObject.Parameter GetParam()
        {
            return param.param;
        }
        public PlayerObject.InspectorParam GetInspectorParam()
        {
            return param.inspectorParam;
        }


        //ステート遷移時に使用する、アンカー判定
        public bool ShotAnchor()
        {
            Vector2 direction = new Vector2(Input.GetAxis("RStickX"), Input.GetAxis("RStickY") * -1);

            //スティックがニュートラルの際は、まっすぐX軸にそったレイを設定する
            if (direction.magnitude == 0.0f)
            {
                //向きによって違う
                if (this.GetParam().direction == PlayerObject.Direction.RIGHT)
                {
                    direction = new Vector2(1, 0);
                }
                else
                {
                    direction = new Vector2(-1, 0.0f);
                }

            }

            //アンカーを見つけて、そこへ向かう
            Debug.DrawRay(GetParam().myself.transform.position, new Vector3(direction.x, direction.y), Color.green, 1);
            var anchorDirection = new Vector2(GetParam().myself.transform.position.x + direction.x, GetParam().myself.transform.position.y + direction.y);
            GetInspectorParam().anchor.FindAnchor(GetParam().myself.transform.position, anchorDirection, out GetParam().anchorTarget);

            Debug.Log(GetParam().anchorTarget);
            //アンカーがなかったらfalse
            if (GetParam().anchorTarget == null)
            {
                return false;
            }
            return true;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //待機
    public class IdleState : BaseState
    {
        public IdleState(PlayerObject param)
            : base(param)

        {
        }

        //入った時の関数
        public override void Enter()
        {
            GetInspectorParam().playerAnim.Play("Idle");
        }
        //出た時の関数
        public override void Exit()
        {

        }
        //遷移を行う
        public override bool Transition(ref StateManager manager)
        {

            float axis = Func.FixXAxis(Input.GetAxis("RStickX"));

            //歩きへ遷移
            if (axis != 0.0f)
            {
                manager.SetNextState((int)PlayerObject.State.WALK);
                return true;
            }
            //落下
            if (!GetParam().onGround)
            {
                manager.SetNextState((int)PlayerObject.State.FALL);
                return true;
            }
            //ジャンプ
            if (M_System.input.Down(SystemInput.Tag.JUMP))
            {
                manager.SetNextState((int)PlayerObject.State.JUMP);
                return true;
            }
            //アンカーショット
            if (M_System.input.Down(SystemInput.Tag.WIRE) && ShotAnchor())
            {
                manager.SetNextState((int)PlayerObject.State.WIRE_DASH);
                return true;
            }


            return false;
        }
        //ステート処理
        public override void Update()
        {
            //移動しない、待機なんでね
            GetParam().moveVector = Vector3.zero;
        }
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //歩き
    public class WalkState : BaseState
    {
        public WalkState(PlayerObject param)
            : base(param)

        {
        }

        //入った時の関数
        public override void Enter()
        {
            //アニメ
            GetInspectorParam().playerAnim.Play("DashStart");

        }
        //出た時の関数
        public override void Exit()
        {

        }
        //遷移を行う
        public override bool Transition(ref StateManager manager)
        {

            float axis = Func.FixXAxis(Input.GetAxis("RStickX"));

            //待機へ遷移
            if (Mathf.Abs(axis) < 0.001f)
            {
                manager.SetNextState((int)PlayerObject.State.IDLE);
                return true;
            }
            //ジャンプ
            if (M_System.input.Down(SystemInput.Tag.JUMP))
            {
                manager.SetNextState((int)PlayerObject.State.JUMP);
                return true;
            }
            //アンカーショット
            if (M_System.input.Down(SystemInput.Tag.WIRE) && ShotAnchor())
            {
                manager.SetNextState((int)PlayerObject.State.WIRE_DASH);
                return true;
            }

            return false;
        }
        //ステート処理
        public override void Update()
        {
            var vec = new Vector2(Func.FixXAxis(Input.GetAxis("RStickX")) * GetInspectorParam().speed, 0.0f);
            //横移動
            GetParam().moveVector += vec;
            //移動方向にて向きを変える
            Func.ChangeDirectionFromMoveX(vec.x, ref GetParam().direction, ref GetParam().myself);
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //ジャンプはじめ
    public class JumpState : BaseState
    {
        public JumpState(PlayerObject param)
            : base(param)

        {
        }

        //入った時の関数
        public override void Enter()
        {
            Sound.PlaySE("jump");
            //アニメ
            GetInspectorParam().playerAnim.Play("JumpUp");
            GetInspectorParam().mover.Jump(GetInspectorParam().jumpPower);

        }
        //出た時の関数
        public override void Exit()
        {

        }
        //遷移を行う
        public override bool Transition(ref StateManager manager)
        {

            float axis = Func.FixXAxis(Input.GetAxis("RStickX"));

            //落下
            if (GetInspectorParam().mover.IsFall())
            {
                manager.SetNextState((int)PlayerObject.State.FALL);
                return true;
            }


            return false;
        }
        //ステート処理
        public override void Update()
        {
            var vec = new Vector2(Func.FixXAxis(Input.GetAxis("RStickX")) * GetInspectorParam().speed, 0.0f);
            //横移動
            GetParam().moveVector += vec;
            //移動方向にて向きを変える
            Func.ChangeDirectionFromMoveX(vec.x, ref GetParam().direction, ref GetParam().myself);
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //落下、あるいは滞空
    public class FallState : BaseState
    {
        public FallState(PlayerObject param)
            : base(param)

        {
        }

        //入った時の関数
        public override void Enter()
        {
            //アニメ
            GetInspectorParam().playerAnim.Play("JumpDownStart");
        }
        //出た時の関数
        public override void Exit()
        {

        }
        //遷移を行う
        public override bool Transition(ref StateManager manager)
        {

            float axis = Func.FixXAxis(Input.GetAxis("RStickX"));

            //着地
            if (GetParam().onGround)
            {
                manager.SetNextState((int)PlayerObject.State.IDLE);
                return true;
            }


            return false;
        }
        //ステート処理
        public override void Update()
        {
            var vec = new Vector2(Func.FixXAxis(Input.GetAxis("RStickX")) * GetInspectorParam().speed, 0.0f);
            //横移動
            GetParam().moveVector += vec;
            //移動方向にて向きを変える
            Func.ChangeDirectionFromMoveX(vec.x, ref GetParam().direction, ref GetParam().myself);
        }
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //アンカーダッシュのステート
    public class AnchorDashState : BaseState
    {
        Vector2 targetDirection;
        int timeCnt;

        public AnchorDashState(PlayerObject param)
            : base(param)

        {
        }


        //入った時の関数
        public override void Enter()
        {
            this.timeCnt = 0;
            //アンカーショットの処理を以下で行う
            this.GetInspectorParam().mover.SetActiveGravity(false, true);

            this.GetInspectorParam().playerAnim.Play("anchorShot");

            //ｶｲｰﾝ
            Sound.PlaySE("wireHit");
            //ヒットエフェクト
            //var effectObj = Instantiate(this.anchorHitEffect);
            //effectObj.transform.position = this.anchorObject.transform.position;

            //現在地から目標のアンカーへ向かうベクトル
            this.targetDirection = new Vector2(GetParam().anchorTarget.transform.localPosition.x - GetParam().myself.transform.localPosition.x, GetParam().anchorTarget.transform.localPosition.y - GetParam().myself.transform.localPosition.y);
            this.targetDirection.Normalize();
            this.GetParam().currentDashSpead = 0.0f;


            //移動方向にて向きを変える
            Func.ChangeDirectionFromMoveX(this.targetDirection.x, ref GetParam().direction, ref GetParam().myself);

            //頭をアンカーに向ける
            float angle = Vector2.Angle(new Vector2(0.0f, 1.0f), this.targetDirection);
            if (this.GetParam().direction == PlayerObject.Direction.RIGHT)
            {
                angle *= -1.0f;
            }

            this.GetParam().myself.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        //出た時の関数
        public override void Exit()
        {

        }
        //遷移を行う
        public override bool Transition(ref StateManager manager)
        {




            if (!GetParam().anchorTarget)
            {
                return false;
            }
            //進行方向と、現在のアンカーへ向かうベクトルを比較し、後ろにあったらJUMPに移行
            float dot = Vector2.Dot(this.targetDirection, GetParam().anchorTarget.transform.position - GetParam().myself.transform.position);

            if (GetParam().anchorTarget != null && dot < 0.0f)
            {
                //移行時、少しだけ滞空時間を延ばすためにジャンプのような挙動を行う
                GetInspectorParam().mover.SetActiveGravity(true, true);
                GetInspectorParam().mover.Jump(this.targetDirection.y * GetParam().currentDashSpead * 0.5f);
                manager.SetNextState((int)PlayerObject.State.JUMP);
                GetParam().anchorTarget = null;

                GetParam().myself.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                return true;
            }


            return false;
        }
        //ステート処理
        public override void Update()
        {
            //アンカーが見つかっている場合にのみ処理を行う
            if (GetParam().anchorTarget != null && this.timeCnt > 2)
            {
                //速度をダッシュ中のそれにする
                //this.speedRatio = 1.0f;
                //アンカーに向かっての移動
                GetParam().moveVector += this.targetDirection * GetParam().currentDashSpead;


                //最高値まで加速
                if (GetInspectorParam().anchorDashSpeed > GetParam().currentDashSpead)
                {
                    GetParam().currentDashSpead += GetInspectorParam().anchorMoveAcceleration;
                    if (GetInspectorParam().anchorDashSpeed > GetParam().currentDashSpead)
                    {
                        //最大値にクランプ
                        GetParam().currentDashSpead = GetInspectorParam().anchorDashSpeed;
                    }
                }
            }
            ++this.timeCnt;
        }
    }

    //便利関数系
    public class Func
    {
        //デジタルな操作を実現する入力補正関数
        public static float FixXAxis(float axisX)
        {
            if (axisX > 0.2)
            {
                return 1.0f;
            }
            if (axisX < -0.2)
            {
                return -1.0f;
            }
            return 0.0f;
        }

        //向き変更関数
        public static void ChangeDirection(PlayerObject.Direction direction, ref PlayerObject.Direction current, ref GameObject transform)
        {
            if (current != direction)
            {
                var scale = transform.transform.localScale;
                scale.x *= -1;
                transform.transform.localScale = scale;
            }

            current = direction;
        }

        //移動の値から方向を振り分ける関数
        public static void ChangeDirectionFromMoveX(float xMove, ref PlayerObject.Direction current, ref GameObject transform)
        {
            if (xMove > 0.0f)
            {
                ChangeDirection(PlayerObject.Direction.RIGHT, ref current, ref transform);
            }
            else if (xMove < 0.0f)
            {
                ChangeDirection(PlayerObject.Direction.LEFT, ref current, ref transform);
            }
        }
    }

}