﻿using System.Collections;
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

        }

        public InspectorParam inspectorParam;
        public Parameter param;

        private void Start()
        {
            this.param = new Parameter();
            this.param.myself = this;
            this.param.moveVector = Vector2.zero;
            this.param.direction = Direction.RIGHT;
            this.param.currentDashSpead = 0.0f;
            this.param.anchorTarget = null;
            this.param.dashRatio = 0.0f;
            //ステート追加
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
            ChangeState((int)State.IDLE);


            GetData().hitPoint.SetInvincible(100);
        }

        //メンバ関数
        PlayerObject()
        {

        }
        public override void ApplyDamage()
        {
            GetData().hitPoint.DamageUpdate();
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
            this.param.dashRatio += -0.02f;
            if (this.param.dashRatio < 0.0f)
            {
                this.param.dashRatio = 0.0f;
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
        public override void Enter(ref StateManager manager)
        {
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
            //落下
            if (!GetParam().onGround)
            {
                manager.SetNextState((int)PlayerObject.State.FALL);
                return true;
            }
            //アンカーショット
            if (M_System.input.Down(SystemInput.Tag.WIRE) && ShotAnchor())
            {
                manager.SetNextState((int)PlayerObject.State.WIRE_DASH);
                return true;
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
            var vec = new Vector2(Func.FixXAxis(Input.GetAxis("RStickX")) * GetPlayerMoveSpeed(), 0.0f);
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
            Sound.PlaySE("jump");
            //アニメ
            GetInspectorParam().playerAnim.Play("JumpUp");
            if (manager.GetPreStateNum() != (int)PlayerObject.State.WIRE_DASH)
            {
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

            float axis = Func.FixXAxis(Input.GetAxis("RStickX"));

            //落下
            if (GetInspectorParam().mover.IsFall())
            {
                manager.SetNextState((int)PlayerObject.State.FALL);
                return true;
            }
            //アンカーショット
            if (M_System.input.Down(SystemInput.Tag.WIRE) && ShotAnchor())
            {
                manager.SetNextState((int)PlayerObject.State.WIRE_DASH);
                return true;
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
            var vec = new Vector2(Func.FixXAxis(Input.GetAxis("RStickX")) * GetPlayerMoveSpeed(), 0.0f);
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

            float axis = Func.FixXAxis(Input.GetAxis("RStickX"));

            //着地
            if (GetParam().onGround)
            {
                manager.SetNextState((int)PlayerObject.State.IDLE);
                return true;
            }
            //アンカーショット
            if (M_System.input.Down(SystemInput.Tag.WIRE) && ShotAnchor())
            {
                manager.SetNextState((int)PlayerObject.State.WIRE_DASH);
                return true;
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
            var vec = new Vector2(Func.FixXAxis(Input.GetAxis("RStickX")) * GetPlayerMoveSpeed(), 0.0f);
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
            //ヒットエフェクト
            //var effectObj = Instantiate(this.anchorHitEffect);
            //effectObj.transform.position = this.anchorObject.transform.position;

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
        }
        //出た時の関数
        public override void Exit(ref StateManager manager)
        {
            GetParam().anchorTarget = null;

            if (manager.GetNextStateNum() != (int)PlayerObject.State.STRIKE_ASULT)
            {
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
                GetInspectorParam().mover.Jump(this.targetDirection.y * GetParam().currentDashSpead * 0.5f);
                manager.SetNextState((int)PlayerObject.State.JUMP);
                return true;
            }

            //ワイヤー中にやればストライクアサルト
            if (GetParam().anchorTarget != null && M_System.input.Down(SystemInput.Tag.ATTACK)){
                manager.SetNextState((int)PlayerObject.State.STRIKE_ASULT);
                return true;
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
            Effect.Get().DeleteEffect(this.effectID);
            GetInspectorParam().mover.SetActiveGravity(true, true);
            GetParam().myself.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            GetParam().myself.GetData().hitPoint.SetDamageShutout(false);
        }
        //遷移を行う
        public override bool Transition(ref StateManager manager)
        {

            if(GetTime() > 15)
            {
                GetInspectorParam().mover.Jump(this.direction.y * GetParam().currentDashSpead * 0.5f);
                manager.SetNextState((int)PlayerObject.State.FALL);
                return true;
            }
            return false;
        }
        //ステート処理
        public override void Update()
        {
            GetInspectorParam().attackCollisions[3].StartAttack();

            //速度をダッシュ中のそれにする
            this.GetParam().dashRatio = 1.0f;

            //攻撃方向へ移動
            GetParam().moveVector += this.direction * GetParam().currentDashSpead * 1.1f;

            //エフェクト追従
            Vector3 pos = GetParam().myself.transform.position + new Vector3(0.0f, 0.0f, 1.0f);
            this.effectObj.transform.position = pos;
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
            if(this.timeCnt == 20)
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
            if (M_System.input.Down(SystemInput.Tag.WIRE) && ShotAnchor() && this.timeCnt > 5)
                {
                manager.SetNextState((int)PlayerObject.State.WIRE_DASH);
                return true;
            }

            return false;
        }
        //ステート処理
        public override void Update()
        {
            var vec = new Vector2(Func.FixXAxis(Input.GetAxis("RStickX")) * GetPlayerMoveSpeed(), 0.0f);
            //硬直中は移動速度を落とす
            if (GetParam().onGround)
            {
                vec.x *= 0.2f;
            }
            else if(this.timeCnt <= 5)
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
            if (M_System.input.Down(SystemInput.Tag.WIRE) && ShotAnchor() && this.timeCnt > 5)
                {
                manager.SetNextState((int)PlayerObject.State.WIRE_DASH);
                return true;
            }

            return false;
        }
        //ステート処理
        public override void Update()
        {
            var vec = new Vector2(Func.FixXAxis(Input.GetAxis("RStickX")) * GetPlayerMoveSpeed(), 0.0f);
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
                GetInspectorParam().attackCollisions[2].StartAttack();
            }
            else
            {
                GetInspectorParam().playerAnim.Play("JumpSwordAttack3");
                GetInspectorParam().mover.Jump(6.0f);
                GetInspectorParam().attackCollisions[2].StartAttack();
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
            if (M_System.input.Down(SystemInput.Tag.WIRE) && ShotAnchor() && this.timeCnt > 14)
            {
                manager.SetNextState((int)PlayerObject.State.WIRE_DASH);
                return true;
            }

            return false;
        }
        //ステート処理
        public override void Update()
        {
            var vec = new Vector2(Func.FixXAxis(Input.GetAxis("RStickX")) * GetPlayerMoveSpeed(), 0.0f);
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