using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//変更履歴
//2018/11/20 板倉 : 作成
//2018/11/21 板倉 : 内部実装
//2018/12/04 板倉：ダメージ関数の変更を反映
//2018/12/04 板倉：ダメージ関数処理追加、攻撃の挙動をより自由度の高い方面へ調整
//　　　　　　　　 小ジャンプ実装

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
            START,
            IDLE,
            WALK,
            JUMP,
            FALL,
            WIRE_DASH,
            STRIKE_ASULT,
            ATTACK1,
            ATTACK2,
            ATTACK3,
            JUMP_ATTACK1,
            JUMP_ATTACK2,
            JUMP_ATTACK3,
            DAMAGE,
            DEATH,
        }

        //Inspectorで値変えたい系の変数
        [System.Serializable]
        public class InspectorParam
        {
            public float gravity;
            public float playerMoveSpeed;
            public float playerDashSpeed;
            public float anchorDashSpeed;
            public float anchorMoveAcceleration;
            public float jumpPower;
            public float damageKnockBackPower;
            public float damageKnockBackTime;
            public Foot footObject;
            public CharacterMover mover;
            public AnchorSelector anchor;
            public Animator playerAnim;
            public NarrowAttacker[] attackCollisions;
            //ストライクアサルトのクールタイム
            public int strikeAsultCoolTime;
            //無敵時間
            public int damageInvincible;
        }


        //よそに渡す用のクラス
        //初期化はちゃんとStart関数でやってね（はあと
        public class Parameter
        {
            public CharacterObject myself;
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

            //ダッシュ比率
            public float dashRatio;

            //ストライクアサルトのクールタイムカウンタ
            public int strikeAsultRestTime;
        }

        public InspectorParam inspectorParam;
        public Parameter param;

        private void Awake()
        {
            this.param = new Parameter();
            this.param.myself = this;
            this.param.moveVector = Vector2.zero;
            this.param.direction = Direction.RIGHT;
            this.param.currentDashSpead = 0.0f;
            this.param.anchorTarget = null;
            this.param.dashRatio = 0.0f;
            this.param.strikeAsultRestTime = 0;
            //ステート追加
            AddState((int)State.START, new StartState(this));
            AddState((int)State.IDLE, new IdleState(this));
            AddState((int)State.WALK, new WalkState(this));
            AddState((int)State.JUMP, new JumpState(this));
            AddState((int)State.FALL, new FallState(this));
            AddState((int)State.WIRE_DASH, new AnchorDashState(this));
            AddState((int)State.STRIKE_ASULT, new StrikeAsult(this));
            AddState((int)State.ATTACK1, new Attack1State(this));
            AddState((int)State.ATTACK2, new Attack2State(this));
            AddState((int)State.ATTACK3, new Attack3State(this));
            AddState((int)State.DAMAGE, new DamageState(this));
            AddState((int)State.DEATH, new DeathState(this));

            ChangeState((int)State.IDLE);


            GetData().hitPoint.SetInvincible(this.inspectorParam.damageInvincible);
        }

        //メンバ関数
        PlayerObject()
        {

        }
        public override void ApplyDamage()
        {
            GetData().hitPoint.DamageUpdate();

            if (this.GetData().hitPoint.GetHP() <= 0 && !IsCurrentState((int)State.DEATH))
            {
                base.ChangeState((int)State.DEATH);
            }
        }

        public override bool Damage(AttackData attackData)
        {
            //普通に食らう
            var isDamaged = GetData().hitPoint.Damage(attackData.power, attackData.chain);
            //ステート変更
            if (isDamaged)
            {
                ChangeState((int)PlayerObject.State.DAMAGE);
            }
            return isDamaged;
        }

        public override void MoveCharacter()
        {
            this.inspectorParam.mover.UpdateVelocity(this.param.moveVector.x, this.param.moveVector.y, this.inspectorParam.gravity, this.param.onGround);
            //移動が終わったら移動ベクトルを初期化
            this.param.moveVector = Vector2.zero;
        }

        public override void UpdateCharacter()
        {
            //ダッシュ速度を自動的に減速
            this.param.dashRatio += -0.02f;
            if (this.param.dashRatio < 0.0f)
            {
                this.param.dashRatio = 0.0f;
            }
            //クールタイム消費
            --this.param.strikeAsultRestTime;
            if (this.param.strikeAsultRestTime < 0)
            {
                this.param.strikeAsultRestTime = 0;
            }

            //無敵時間中は色を変える
            if (GetData().hitPoint.IsInvincible() && GetData().stateManager.GetNowStateNum() != (int)State.DEATH)
            {
                GetComponent<SpriteRenderer>().color = new Color(0.9f, 0.5f, 0.5f, 1.0f);
            }
            else
            {
                GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }

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

        public float GetPlayerMoveSpeed()
        {
            return this.param.inspectorParam.playerMoveSpeed * (1.0f - this.param.param.dashRatio) + this.param.inspectorParam.playerDashSpeed * this.param.param.dashRatio;
        }

        //ステート遷移時に使用する、アンカー判定
        public bool ShotAnchor()
        {
            float x = M_System.input.Axis(SystemInput.Tag.LSTICK_RIGHT);
            float y = M_System.input.Axis(SystemInput.Tag.LSTICK_DOWN);
            //スティックの向きを取得
            Vector2 direction = new Vector2(x, y * -1);

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

            //アンカーがなかったらfalse
            if (GetParam().anchorTarget == null)
            {
                //エフェクト
                Effect.Get().CreateEffect("wireFailed", GetParam().myself.transform.position, Quaternion.FromToRotation(Vector2.up, direction), Vector3.one);
                return false;
            }
            return true;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //登場ステート
    public class StartState : BaseState
    {
        public StartState(PlayerObject param)
            : base(param)

        {
        }

        //入った時の関数
        public override void Enter(ref StateManager manager)
        {
            GetInspectorParam().playerAnim.Play("Start");
            //エフェクト
            Effect.Get().CreateEffect("wireHit", GetParam().myself.transform.position, Quaternion.identity, Vector3.one);
        }
        //出た時の関数
        public override void Exit(ref StateManager manager)
        {

        }
        //遷移を行う
        public override bool Transition(ref StateManager manager)
        {

            //歩きへ遷移
            if (GetTime() > 80.0f)
            {
                manager.SetNextState((int)PlayerObject.State.IDLE);
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
    //待機
    public class IdleState : BaseState
    {
        public IdleState(PlayerObject param)
            : base(param)

        {
        }

        //入った時の関数
        public override void Enter(ref StateManager manager)
        {
            //登場ステートからの移行の場合、アニメーションはすでに待機モーション
            if (manager.GetPreStateNum() == (int)PlayerObject.State.START)
            {
                return;
            }


            GetInspectorParam().playerAnim.Play("Idle");
            if (manager.GetPreStateNum() == (int)PlayerObject.State.FALL)
            {
                GetInspectorParam().playerAnim.Play("onGround");
            }
            if (manager.GetPreStateNum() == (int)PlayerObject.State.WALK)
            {
                GetInspectorParam().playerAnim.Play("DashStop");
            }
            if (manager.GetPreStateNum() == (int)PlayerObject.State.ATTACK1)
            {
                GetInspectorParam().playerAnim.Play("attackEnd");
            }
            if (manager.GetPreStateNum() == (int)PlayerObject.State.ATTACK2)
            {
                GetInspectorParam().playerAnim.Play("attackEnd");
            }
            if (manager.GetPreStateNum() == (int)PlayerObject.State.ATTACK3)
            {
                GetInspectorParam().playerAnim.Play("attackEnd");
            }
        }
        //出た時の関数
        public override void Exit(ref StateManager manager)
        {

        }
        //遷移を行う
        public override bool Transition(ref StateManager manager)
        {
            float axis = Func.FixXAxis(M_System.input.Axis(SystemInput.Tag.LSTICK_RIGHT));

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
            if (M_System.input.Down(SystemInput.Tag.WIRE))
            {
                Sound.PlaySE("wireShot");
                if (ShotAnchor())
                {
                    manager.SetNextState((int)PlayerObject.State.WIRE_DASH);
                    return true;
                }
            }
            //攻撃
            if (M_System.input.Down(SystemInput.Tag.ATTACK))
            {
                manager.SetNextState((int)PlayerObject.State.ATTACK1);
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
        public override void Enter(ref StateManager manager)
        {
            //アニメ
            GetInspectorParam().playerAnim.Play("DashStart");

        }
        //出た時の関数
        public override void Exit(ref StateManager manager)
        {

        }
        //遷移を行う
        public override bool Transition(ref StateManager manager)
        {

            float axis = Func.FixXAxis(M_System.input.Axis(SystemInput.Tag.LSTICK_RIGHT));

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
            //落下
            if (!GetParam().onGround)
            {
                manager.SetNextState((int)PlayerObject.State.FALL);
                return true;
            }
            //アンカーショット
            if (M_System.input.Down(SystemInput.Tag.WIRE))
            {
                Sound.PlaySE("wireShot");
                if (ShotAnchor())
                {
                    manager.SetNextState((int)PlayerObject.State.WIRE_DASH);
                    return true;
                }
            }
            //攻撃
            if (M_System.input.Down(SystemInput.Tag.ATTACK))
            {
                manager.SetNextState((int)PlayerObject.State.ATTACK1);
                return true;
            }

            return false;
        }
        //ステート処理
        public override void Update()
        {
            float axis = M_System.input.Axis(SystemInput.Tag.LSTICK_RIGHT);
            var vec = new Vector2(Func.FixXAxis(axis) * GetPlayerMoveSpeed(), 0.0f);
            //横移動
            GetParam().moveVector += vec;
            //移動方向にて向きを変える
            Func.ChangeDirectionFromMoveX(vec.x, ref GetParam().direction, GetParam().myself.transform);
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
        public override void Enter(ref StateManager manager)
        {
            //アニメ
            GetInspectorParam().playerAnim.Play("JumpUp");
            if (manager.GetPreStateNum() != (int)PlayerObject.State.WIRE_DASH)
            {
                Sound.PlaySE("jump");
                GetInspectorParam().mover.Jump(GetInspectorParam().jumpPower);
            }
            ResetTime();
        }
        //出た時の関数
        public override void Exit(ref StateManager manager)
        {

        }
        //遷移を行う
        public override bool Transition(ref StateManager manager)
        {
            //落下
            if (GetInspectorParam().mover.IsFall())
            {
                manager.SetNextState((int)PlayerObject.State.FALL);
                return true;
            }
            //アンカーショット
            if (M_System.input.Down(SystemInput.Tag.WIRE))
            {
                Sound.PlaySE("wireShot");
                if (ShotAnchor())
                {
                    manager.SetNextState((int)PlayerObject.State.WIRE_DASH);
                    return true;
                }
            }
            //攻撃
            if (M_System.input.Down(SystemInput.Tag.ATTACK))
            {
                manager.SetNextState((int)PlayerObject.State.ATTACK1);
                return true;
            }


            return false;
        }
        //ステート処理
        public override void Update()
        {
            float axis = M_System.input.Axis(SystemInput.Tag.LSTICK_RIGHT);
            var vec = new Vector2(Func.FixXAxis(axis) * GetPlayerMoveSpeed(), 0.0f);
            //横移動
            GetParam().moveVector += vec;
            //移動方向にて向きを変える
            Func.ChangeDirectionFromMoveX(vec.x, ref GetParam().direction, GetParam().myself.transform);

            //一定時間以内にボタンを離すと、上昇力が格段に落ちて小ジャンプになる
            if (M_System.input.Up(SystemInput.Tag.JUMP) && GetTime() < 20)
            {
                GetInspectorParam().mover.Jump(GetInspectorParam().jumpPower * 0.4f);
            }
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
        public override void Enter(ref StateManager manager)
        {
            //アニメ
            GetInspectorParam().playerAnim.Play("JumpDownStart");
        }
        //出た時の関数
        public override void Exit(ref StateManager manager)
        {

        }
        //遷移を行う
        public override bool Transition(ref StateManager manager)
        {
            //着地
            if (GetParam().onGround)
            {
                manager.SetNextState((int)PlayerObject.State.IDLE);
                return true;
            }
            //アンカーショット
            if (M_System.input.Down(SystemInput.Tag.WIRE))
            {
                Sound.PlaySE("wireShot");
                if (ShotAnchor())
                {
                    manager.SetNextState((int)PlayerObject.State.WIRE_DASH);
                    return true;
                }
            }
            //攻撃
            if (M_System.input.Down(SystemInput.Tag.ATTACK))
            {
                manager.SetNextState((int)PlayerObject.State.ATTACK1);
                return true;
            }

            return false;
        }
        //ステート処理
        public override void Update()
        {
            float axis = M_System.input.Axis(SystemInput.Tag.LSTICK_RIGHT);
            var vec = new Vector2(Func.FixXAxis(axis) * GetPlayerMoveSpeed(), 0.0f);
            //横移動
            GetParam().moveVector += vec;
            //移動方向にて向きを変える
            Func.ChangeDirectionFromMoveX(vec.x, ref GetParam().direction, GetParam().myself.transform);
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
        public override void Enter(ref StateManager manager)
        {
            this.timeCnt = 0;
            //アンカーショットの処理を以下で行う
            this.GetInspectorParam().mover.SetActiveGravity(false, true);

            this.GetInspectorParam().playerAnim.Play("anchorShot");

            //ｶｲｰﾝ
            Sound.PlaySE("wireHit");


            //現在地から目標のアンカーへ向かうベクトル
            this.targetDirection = new Vector2(GetParam().anchorTarget.transform.localPosition.x - GetParam().myself.transform.localPosition.x, GetParam().anchorTarget.transform.localPosition.y - GetParam().myself.transform.localPosition.y);
            this.targetDirection.Normalize();
            this.GetParam().currentDashSpead = 0.0f;


            //移動方向にて向きを変える
            Func.ChangeDirectionFromMoveX(this.targetDirection.x, ref GetParam().direction, GetParam().myself.transform);

            //頭をアンカーに向ける
            float angle = Vector2.Angle(new Vector2(0.0f, 1.0f), this.targetDirection);
            if (this.GetParam().direction == PlayerObject.Direction.RIGHT)
            {
                angle *= -1.0f;
            }

            this.GetParam().myself.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            //エフェクト
            Effect.Get().CreateEffect("wire", GetParam().myself.transform.position, GetParam().myself.transform.rotation, Vector3.one);

            //ヒットエフェクト
            Effect.Get().CreateEffect("wireHit", GetParam().anchorTarget.transform.position, Quaternion.identity, Vector3.one);
        }
        //出た時の関数
        public override void Exit(ref StateManager manager)
        {

            if (manager.GetNextStateNum() != (int)PlayerObject.State.STRIKE_ASULT)
            {
                GetParam().anchorTarget = null;
                GetParam().myself.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            }
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
                GetInspectorParam().mover.Jump(this.targetDirection.y * GetParam().currentDashSpead * 0.65f);
                manager.SetNextState((int)PlayerObject.State.JUMP);
                return true;
            }

            //クールタイムが終わっているか
            if (GetParam().strikeAsultRestTime == 0)
            {
                //ワイヤー中にやればストライクアサルト
                if (GetParam().anchorTarget != null && M_System.input.Down(SystemInput.Tag.ATTACK))
                {
                    manager.SetNextState((int)PlayerObject.State.STRIKE_ASULT);
                    return true;
                }
            }


            return false;
        }
        //ステート処理
        public override void Update()
        {
            //速度をダッシュ中のそれにする
            this.GetParam().dashRatio = 1.0f;

            //アンカーが見つかっている場合にのみ処理を行う
            if (GetParam().anchorTarget != null && this.timeCnt > 2)
            {
                //アンカーに向かっての移動
                GetParam().moveVector += this.targetDirection * GetParam().currentDashSpead;


                //最高値まで加速
                if (GetInspectorParam().anchorDashSpeed > GetParam().currentDashSpead)
                {
                    GetParam().currentDashSpead += GetInspectorParam().anchorMoveAcceleration;
                    if (GetInspectorParam().anchorDashSpeed < GetParam().currentDashSpead)
                    {
                        //最大値にクランプ
                        GetParam().currentDashSpead = GetInspectorParam().anchorDashSpeed;
                    }
                }
            }
            ++this.timeCnt;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //アンカーダッシュ攻撃「ストライクアサルト」 キャンセル攻撃版
    public class StrikeAsultBurst : BaseState
    {
        Vector2 direction;
        private GameObject effectObj;
        private int effectID;

        public StrikeAsultBurst(PlayerObject param)
            : base(param)
        {
        }

        //入った時の関数
        public override void Enter(ref StateManager manager)
        {
            Sound.PlaySE("impact1");
            Sound.PlaySE("swingBig");

            //速度をダッシュ中のそれにする
            this.GetParam().dashRatio = 0.4f;

            //キャラの頭の向きから移動方向を割り出す
            this.direction = GetParam().myself.transform.rotation * new Vector2(0.0f, 1.0f);
            //最高値の速さに変更
            GetParam().currentDashSpead = GetInspectorParam().anchorDashSpeed;

            //エフェクト生成
            Vector3 pos = GetParam().myself.transform.position + new Vector3(0.0f, 0.0f, 1.0f);
            Quaternion directionRot = GetParam().myself.transform.rotation * Quaternion.AngleAxis(90.0f, Vector3.forward);

            this.effectID = Effect.Get().CreateEffect("magicAttack1", pos, directionRot, Vector3.one * 5.0f);
            this.effectObj = Effect.Get().GetEffectGameObject(this.effectID);

            GetParam().myself.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            GetInspectorParam().mover.SetActiveGravity(true, true);
            GetInspectorParam().attackCollisions[3].StartAttack();

            //アニメ
            if (GetParam().onGround && !GetInspectorParam().mover.IsJump())
            {
                GetInspectorParam().playerAnim.Play("SwordAttack3");
            }
            else
            {
                GetInspectorParam().playerAnim.Play("JumpSwordAttack3");
                GetInspectorParam().mover.Jump(12.0f);
            }
        }
        //出た時の関数
        public override void Exit(ref StateManager manager)
        {
            GetParam().anchorTarget = null;
        }
        //遷移を行う
        public override bool Transition(ref StateManager manager)
        {
            if (GetTime() >= 20)
            {
                if (GetParam().onGround)
                {
                    manager.SetNextState((int)PlayerObject.State.IDLE);
                }
                else
                {
                    manager.SetNextState((int)PlayerObject.State.FALL);
                }
                return true;
            }

            return false;
        }
        //ステート処理
        public override void Update()
        {
            //攻撃方向へ移動
            GetParam().moveVector += this.direction * GetParam().currentDashSpead * 1.0f * this.GetParam().dashRatio;

            //エフェクト追従
            Vector3 pos = GetParam().myself.transform.position + new Vector3(0.0f, 0.0f, 1.0f);
            this.effectObj.transform.position = pos;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //アンカーダッシュ攻撃「ストライクアサルト」
    public class StrikeAsult : BaseState
    {
        private Vector2 direction;
        private GameObject effectObj;
        private int effectID;

        public StrikeAsult(PlayerObject param)
            : base(param)
        {
        }

        //入った時の関数
        public override void Enter(ref StateManager manager)
        {
            Sound.PlaySE("impact1");
            //キャラの頭の向きから移動方向を割り出す
            this.direction = GetParam().myself.transform.rotation * new Vector2(0.0f, 1.0f);
            //最高値の速さに変更
            GetParam().currentDashSpead = GetInspectorParam().anchorDashSpeed;

            //エフェクト生成
            Vector3 pos = GetParam().myself.transform.position + new Vector3(0.0f, 0.0f, 1.0f);
            Quaternion directionRot = GetParam().myself.transform.rotation * Quaternion.AngleAxis(90.0f, Vector3.forward);
            this.effectID = Effect.Get().CreateEffect("tackle", pos, directionRot, Vector3.one);
            this.effectObj = Effect.Get().GetEffectGameObject(this.effectID);

            //無敵化
            GetParam().myself.GetData().hitPoint.SetDamageShutout(true);

        }
        //出た時の関数
        public override void Exit(ref StateManager manager)
        {
            GetParam().anchorTarget = null;
            Effect.Get().DeleteEffect(this.effectID);
            GetInspectorParam().mover.SetActiveGravity(true, true);
            GetParam().myself.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            GetParam().myself.GetData().hitPoint.SetDamageShutout(false);

        }
        //遷移を行う
        public override bool Transition(ref StateManager manager)
        {

            if (GetTime() > 30)
            {
                //GetInspectorParam().mover.Jump(this.direction.y * GetParam().currentDashSpead * 0.5f);
                GetInspectorParam().mover.Jump(0.5f);
                manager.SetNextState((int)PlayerObject.State.FALL);
                return true;
            }

            return false;
        }
        //ステート処理
        public override void Update()
        {
            if (GetTime() < 10)
            {
                //速度をダッシュ中のそれにする
                this.GetParam().dashRatio = 1.0f;
            }
            else
            {
                this.GetParam().dashRatio *= 0.93f;
            }
            //十分に減速したら無敵時間を解除
            if(GetTime() > 20)
            {
                GetParam().myself.GetData().hitPoint.SetDamageShutout(false);
                Effect.Get().DeleteEffect(this.effectID);
                this.effectID = -1;
            }
            else
            {
                //エフェクト追従
                Vector3 pos = GetParam().myself.transform.position + new Vector3(0.0f, 0.0f, 1.0f);
                this.effectObj.transform.position = pos;
            }


            //クールタイム発生
            GetParam().strikeAsultRestTime = GetInspectorParam().strikeAsultCoolTime;

            GetInspectorParam().attackCollisions[3].StartAttack();


            //攻撃方向へ移動
            GetParam().moveVector += this.direction * GetParam().currentDashSpead * 1.5f * this.GetParam().dashRatio;


        }
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //攻撃一段目
    public class Attack1State : BaseState
    {
        int timeCnt;


        public Attack1State(PlayerObject param)
            : base(param)

        {
        }

        //入った時の関数
        public override void Enter(ref StateManager manager)
        {
            //アニメ
            if (GetParam().onGround && !GetInspectorParam().mover.IsJump())
            {
                GetInspectorParam().playerAnim.Play("SwordAttack1");
                GetInspectorParam().attackCollisions[0].StartAttack();
            }
            else
            {
                GetInspectorParam().playerAnim.Play("JumpSwordAttack1");
                GetInspectorParam().mover.Jump(6.0f);
                GetInspectorParam().attackCollisions[0].StartAttack();
            }

            Sound.PlaySE("swingSmall");
            this.timeCnt = 0;
        }
        //出た時の関数
        public override void Exit(ref StateManager manager)
        {

        }
        //遷移を行う
        public override bool Transition(ref StateManager manager)
        {
            //硬直終わり
            if (this.timeCnt == 20)
            {
                if (GetParam().onGround)
                {
                    manager.SetNextState((int)PlayerObject.State.IDLE);
                }
                else
                {
                    manager.SetNextState((int)PlayerObject.State.FALL);
                }
                return true;
            }
            //ジャンプ、地面についてる時
            if (M_System.input.Down(SystemInput.Tag.JUMP) && GetParam().onGround && this.timeCnt > 0)
            {
                manager.SetNextState((int)PlayerObject.State.JUMP);
                return true;
            }
            //攻撃
            if (M_System.input.Down(SystemInput.Tag.ATTACK) && this.timeCnt > 0)
            {
                manager.SetNextState((int)PlayerObject.State.ATTACK2);
                return true;
            }
            //アンカーショット
            if (M_System.input.Down(SystemInput.Tag.WIRE))
            {
                Sound.PlaySE("wireShot");
                if (ShotAnchor())
                {
                    manager.SetNextState((int)PlayerObject.State.WIRE_DASH);
                    return true;
                }
            }

            return false;
        }
        //ステート処理
        public override void Update()
        {
            float axis = M_System.input.Axis(SystemInput.Tag.LSTICK_RIGHT);
            var vec = new Vector2(Func.FixXAxis(axis) * GetPlayerMoveSpeed(), 0.0f);
            //硬直中は移動速度を落とす
            if (GetParam().onGround)
            {
                vec.x *= 0.2f;
            }
            else if (this.timeCnt <= 5)
            {
                Func.ChangeDirectionFromMoveX(vec.x, ref GetParam().direction, GetParam().myself.transform);
            }
            //横移動
            GetParam().moveVector += vec;


            ++this.timeCnt;
        }
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //攻撃二段目
    public class Attack2State : BaseState
    {
        int timeCnt;


        public Attack2State(PlayerObject param)
            : base(param)

        {
        }

        //入った時の関数
        public override void Enter(ref StateManager manager)
        {
            //アニメ
            if (GetParam().onGround && !GetInspectorParam().mover.IsJump())
            {
                GetInspectorParam().playerAnim.Play("SwordAttack2");
                GetInspectorParam().attackCollisions[1].StartAttack();
            }
            else
            {
                GetInspectorParam().playerAnim.Play("JumpSwordAttack2");
                GetInspectorParam().mover.Jump(6.0f);
                GetInspectorParam().attackCollisions[1].StartAttack();
            }

            Sound.PlaySE("swingSmall");
            this.timeCnt = 0;
        }
        //出た時の関数
        public override void Exit(ref StateManager manager)
        {

        }
        //遷移を行う
        public override bool Transition(ref StateManager manager)
        {
            if (this.timeCnt == 20)
            {
                if (GetParam().onGround)
                {
                    manager.SetNextState((int)PlayerObject.State.IDLE);
                }
                else
                {
                    manager.SetNextState((int)PlayerObject.State.FALL);
                }
                return true;
            }
            //ジャンプ、地面についてる時
            if (M_System.input.Down(SystemInput.Tag.JUMP) && GetParam().onGround && this.timeCnt > 0)
            {
                manager.SetNextState((int)PlayerObject.State.JUMP);
                return true;
            }

            //攻撃
            if (M_System.input.Down(SystemInput.Tag.ATTACK) && this.timeCnt > 0)
            {
                manager.SetNextState((int)PlayerObject.State.ATTACK3);
                return true;
            }
            //アンカーショット
            if (M_System.input.Down(SystemInput.Tag.WIRE))
            {
                Sound.PlaySE("wireShot");
                if (ShotAnchor())
                {
                    manager.SetNextState((int)PlayerObject.State.WIRE_DASH);
                    return true;
                }
            }

            return false;
        }
        //ステート処理
        public override void Update()
        {
            float axis = M_System.input.Axis(SystemInput.Tag.LSTICK_RIGHT);
            var vec = new Vector2(Func.FixXAxis(axis) * GetPlayerMoveSpeed(), 0.0f);
            //硬直中は移動速度を落とす
            if (GetParam().onGround)
            {
                vec.x *= 0.2f;
            }
            else if (this.timeCnt <= 5)
            {
                Func.ChangeDirectionFromMoveX(vec.x, ref GetParam().direction, GetParam().myself.transform);
            }
            //横移動
            GetParam().moveVector += vec;


            ++this.timeCnt;
        }
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //攻撃三段目
    public class Attack3State : BaseState
    {
        int timeCnt;


        public Attack3State(PlayerObject param)
            : base(param)

        {
        }

        //入った時の関数
        public override void Enter(ref StateManager manager)
        {
            //アニメ
            if (GetParam().onGround && !GetInspectorParam().mover.IsJump())
            {
                GetInspectorParam().playerAnim.Play("SwordAttack3");
            }
            else
            {
                GetInspectorParam().playerAnim.Play("JumpSwordAttack3");
                GetInspectorParam().mover.Jump(6.0f);
            }


            this.timeCnt = 0;
        }
        //出た時の関数
        public override void Exit(ref StateManager manager)
        {

        }
        //遷移を行う
        public override bool Transition(ref StateManager manager)
        {
            if (this.timeCnt == 20)
            {
                if (GetParam().onGround)
                {
                    manager.SetNextState((int)PlayerObject.State.IDLE);
                }
                else
                {
                    manager.SetNextState((int)PlayerObject.State.FALL);
                }
                return true;
            }

            //ジャンプ、地面についてる時
            if (M_System.input.Down(SystemInput.Tag.JUMP) && GetParam().onGround && this.timeCnt > 0)
            {
                manager.SetNextState((int)PlayerObject.State.JUMP);
                return true;
            }

            //アンカーショット
            if (M_System.input.Down(SystemInput.Tag.WIRE))
            {
                Sound.PlaySE("wireShot");
                if (ShotAnchor())
                {
                    manager.SetNextState((int)PlayerObject.State.WIRE_DASH);
                    return true;
                }
            }

            return false;
        }
        //ステート処理
        public override void Update()
        {
            float axis = M_System.input.Axis(SystemInput.Tag.LSTICK_RIGHT);
            var vec = new Vector2(Func.FixXAxis(axis) * GetPlayerMoveSpeed(), 0.0f);
            //硬直中は移動速度を落とす
            if (GetParam().onGround)
            {
                vec.x *= 0.2f;
            }
            else if (this.timeCnt <= 14)
            {
                Func.ChangeDirectionFromMoveX(vec.x, ref GetParam().direction, GetParam().myself.transform);
            }
            //横移動
            GetParam().moveVector += vec;



            //攻撃処理は遅れて実行される
            //アニメ
            if (GetTime() == 5)
            {
                if (GetParam().onGround && !GetInspectorParam().mover.IsJump())
                {
                    GetInspectorParam().attackCollisions[2].StartAttack();
                }
                else
                {
                    GetInspectorParam().attackCollisions[2].StartAttack();
                }
                Sound.PlaySE("swingBig");
            }


            ++this.timeCnt;
        }
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //ダメージ
    public class DamageState : BaseState
    {
        public DamageState(PlayerObject param)
            : base(param)

        {
        }

        //入った時の関数
        public override void Enter(ref StateManager manager)
        {
            GetInspectorParam().mover.SetActiveGravity(false, false);

            ResetTime();
            //アニメ
            GetInspectorParam().playerAnim.Play("Damage");

            Sound.PlaySE("PlayerDamage");
        }
        //出た時の関数
        public override void Exit(ref StateManager manager)
        {
            GetInspectorParam().mover.SetActiveGravity(true, false);
        }
        //遷移を行う
        public override bool Transition(ref StateManager manager)
        {
            //硬直が終わったら待機へ遷移
            if (GetTime() > GetInspectorParam().damageKnockBackTime)
            {
                manager.SetNextState((int)PlayerObject.State.IDLE);
                return true;
            }


            return false;
        }
        //ステート処理
        public override void Update()
        {
            //後ろへノックバック
            Vector2 vec;
            if (GetParam().direction == PlayerObject.Direction.RIGHT)
            {
                vec = new Vector2(-GetInspectorParam().damageKnockBackPower, 0.0f);
            }
            else
            {
                vec = new Vector2(GetInspectorParam().damageKnockBackPower, 0.0f);
            }
            //横移動
            GetParam().moveVector += vec;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //死亡
    public class DeathState : BaseState
    {
        float knockBackPower;


        public DeathState(PlayerObject param)
            : base(param)

        {
        }

        //入った時の関数
        public override void Enter(ref StateManager manager)
        {
            GetInspectorParam().mover.SetActiveGravity(false, false);
            //アニメ
            GetInspectorParam().playerAnim.Play("Death");

            //以後一切のダメージ処理は行わない
            GetParam().myself.GetData().hitPoint.SetDamageShutout(true);

            //割合で表す
            this.knockBackPower = 1.0f;

            Sound.PlaySE("crash1");
        }
        //出た時の関数
        public override void Exit(ref StateManager manager)
        {
        }
        //遷移を行う
        public override bool Transition(ref StateManager manager)
        {



            return false;
        }
        //ステート処理
        public override void Update()
        {
            //後ろへノックバック
            Vector2 vec;
            if (GetParam().direction == PlayerObject.Direction.RIGHT)
            {
                vec = new Vector2(-GetInspectorParam().damageKnockBackPower * this.knockBackPower, 0.0f);
            }
            else
            {
                vec = new Vector2(GetInspectorParam().damageKnockBackPower * this.knockBackPower, 0.0f);
            }
            //横移動
            GetParam().moveVector += vec;

            //ちょっとだけノックバックを遅くする
            this.knockBackPower *= 0.95f;

            if(this.knockBackPower < 0.05f)
            {
                //十分に速度が落ちたらキャラ消去
                GetParam().myself.KillMyself();
                Sound.PlaySE("noiseLong");
                Effect.Get().CreateEffect("PlayerDeath", GetParam().myself.transform.position, Quaternion.identity, Vector3.one);
            }
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
        public static void ChangeDirection(PlayerObject.Direction direction, ref PlayerObject.Direction current, Transform transform)
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
        public static void ChangeDirectionFromMoveX(float xMove, ref PlayerObject.Direction current, Transform transform)
        {
            if (xMove > 0.0f)
            {
                ChangeDirection(PlayerObject.Direction.RIGHT, ref current, transform);
            }
            else if (xMove < 0.0f)
            {
                ChangeDirection(PlayerObject.Direction.LEFT, ref current, transform);
            }
        }
    }

}