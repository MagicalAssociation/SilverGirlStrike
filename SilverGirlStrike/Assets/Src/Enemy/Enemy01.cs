using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//変更履歴
//2018/12/04 板倉：ダメージ関数の変更を反映





/**
 * file     Enemy01.cs
 * brief    敵Class
 * author   Shou Kaneko
 * date     2018/11/27
 * 状態
 *      立ち、攻撃、受け、やられ、落下
 * GameObject targetになにもいれないと固定方向に攻撃を飛ばします。
 * targetにオブジェクトを登録するとその方向に攻撃を飛ばします。
*/
/**
 * Inspectorの設定値の説明
 * Gravity このCharacterの重さ
 * Parameter.MaxHP 最大HP
 * Parameter.Radius このCharacterが攻撃を始める範囲
 * Parameter.AttackInterval 攻撃の感覚を指定します。
 * Parameter.Animation アニメーションデータ
 * Parameter.Mover 自動で入るので放置で構いません
 * Parameter.Foot このGameObjectの子にFoot.csこみのGameObjectをいれておいてください、入れるのは自動です
 * Parameter.Direction 自動で入るので放置で
 * BulletData.Angle 弾を飛ばす方向
 * BulletData.power ダメージ値
 * BulletData.Speed 弾の飛ぶ速度
 * BulletData.Life 生存するカウント数
 * BulletData.Target 指定オブジェクトの方向に攻撃を飛ばす、ここに何も入っていない時に指定角度で弾を飛ばします
 * EnableGravity このCharacterに重力を適用するかです。外れている場合落下もせず、FallStateにもなりません。
 */
namespace Enemy01
{
    /**
     * brief    エネミー01
     * 固定砲台　指定向きに飛ばす
     */ 
    public class Enemy01 : CharacterObject
    {
        /**
         * enum State
         */
         public enum State
        {
            NORMAL,
            ATTACK,
            FALL,
            DEATH,
            //! 軌道待ち
            ORBIT,
            STARTATTACK,
            ENDATTACK,
        }
        [System.Serializable]
        public class Enemy01Parameter
        {
            //! 最大HP
            public int maxHP;
            //! 攻撃開始範囲の半径
            public float radius;
            //! 攻撃間隔
            public int attackInterval;
            //! Animation
            public Animator animation;
            //! 移動用class
            public CharacterMover mover;
            //! 地面判定
            public Foot foot;
            //攻撃判定オブジェクト
            public NarrowAttacker[] narrowAttacker;
            // 向きの自動変更
            public Enemy.AutoScaleChange autoScaleChange;

            //無敵時間
            public int damageInvincible;
        }
        //! 重力
        public float gravity;
        //! Parameter
        public Enemy01Parameter parameter;
        //! 生成するBulletのデータ
        public Bullet.BulletParameter bulletParameter;
        //! 当たり判定
        BoxCollider2D boxCollider;
        //! 攻撃範囲
        CircleCollider2D circleCollider;
        //! 移動値
        Vector2 movePower;
        //! 重力の有効化設定
        public bool enableGravity;
        /**
         * brief    constructor
         */
        public Enemy01()
            :base(10)
        {
            this.movePower = new Vector2();
        }
        // Update is called once per frame
        private void Start()
        {
            parameter.mover = GetComponent<CharacterMover>();
            parameter.foot = this.transform.GetComponentInChildren<Foot>();
            this.boxCollider = GetComponent<BoxCollider2D>();
            this.circleCollider = GetComponent<CircleCollider2D>();

            this.parameter.animation = GetComponent<Animator>();
            this.GetData().hitPoint.SetMaxHP(this.parameter.maxHP);
            this.GetData().hitPoint.Recover(this.parameter.maxHP);
            this.GetData().hitPoint.SetInvincible(this.parameter.damageInvincible);

            this.parameter.autoScaleChange.Init(this.gameObject);
            //!-ステートデータを登録＆初期値の設定-!//
            base.AddState((int)State.NORMAL, new NormalState(this));
            base.AddState((int)State.ATTACK, new AttackState(this));
            base.AddState((int)State.FALL, new FallState(this));
            base.AddState((int)State.DEATH, new DeathState(this));
            base.AddState((int)State.ORBIT, new OrbitState(this));
            base.AddState((int)State.STARTATTACK, new AttackStartState(this));
            base.AddState((int)State.ENDATTACK, new AttackEndState(this));
            base.ChangeState((int)State.ORBIT);
        }
        public override void UpdateCharacter()
        {
            //判定垂れ流し
            if (this.GetData().stateManager.GetNowStateNum() != (int)State.DEATH)
            {
                this.parameter.narrowAttacker[0].StartAttack();
            }
            this.UpdateState();
        }
        public override bool Damage(AttackData data)
        {
            return this.GetData().hitPoint.Damage(data.power, data.chain);
        }
        public override void ApplyDamage()
        {
            //HPなくなったら死亡へ
            if(this.GetData().hitPoint.GetHP() <= 0 && base.GetData().stateManager.GetNowStateNum() != (int)State.DEATH)
            {
                base.ChangeState((int)State.DEATH);
            }
            else
            {
                this.GetData().hitPoint.DamageUpdate();
            }
        }
        public override void MoveCharacter()
        {
            if (this.GetData().stateManager.GetNowStateNum() != (int)State.ORBIT)
            {
                parameter.mover.UpdateVelocity(this.movePower.x, this.movePower.y, this.enableGravity ? this.gravity : 0.0f, parameter.foot.CheckHit());
                this.movePower = Vector2.zero;
            }
            this.parameter.autoScaleChange.DirectionUpdate();
        }
        /**
         * brief    Enemy01専用のパラメータデータ取得
         */
         public Enemy01Parameter GetParameter()
        {
            return this.parameter;
        }
        /**
         * brief    攻撃するターゲットが近くにいるか返す
         * return Collider2D 円の判定
         */
         public Collider2D TargetDistanceCheck()
        {
            return Physics2D.OverlapCircle(
                   this.circleCollider.transform.position,
                   this.circleCollider.radius,
                   (int)M_System.LayerName.PLAYER
                   );
        }
        public void SetMovePower(Vector2 move)
        {
            this.movePower = move;
        }
    }
    /**
     * brief    元となるState
     */ 
    public abstract class BaseState : StateParameter
    {
        public Enemy01 enemy;
        public BaseState(Enemy01 enemy)
        {
            this.enemy = enemy;
        }
    }
    /**
     * brief    立ちモーション
     */
    public class NormalState : BaseState
    {
        public NormalState(Enemy01 enemy) 
            : base(enemy)
        {

        }

        public override void Enter(ref StateManager manager)
        {
            this.enemy.parameter.animation.Play("Normal");
        }

        public override void Exit(ref StateManager manager)
        {
        }

        public override bool Transition(ref StateManager manager)
        {
            if(!enemy.GetParameter().foot.CheckHit() && this.enemy.enableGravity)
            {
                manager.SetNextState((int)Enemy01.State.FALL);
                return true;
            }
            //一定count経過したら攻撃モーションへ移行
            //攻撃データの弾数が０の場合移行をしない
            if(base.enemy.bulletParameter.bulletNum != 0 && base.GetTime() > base.enemy.GetParameter().attackInterval)
            {
                manager.SetNextState((int)Enemy01.State.STARTATTACK);
                return true;
            }
            return false;
        }

        public override void Update()
        {
        }
    }
    /**
     * brief    攻撃モーション
     */
    public class AttackState : BaseState
    {
        float preDeltaTime;
        public float animationTime;
        bool shot;
        public AttackState(Enemy01 enemy) 
            : base(enemy)
        {
        }

        public override void Enter(ref StateManager manager)
        {
            this.enemy.parameter.animation.Play("Attack");
            this.animationTime = this.enemy.parameter.animation.GetCurrentAnimatorStateInfo(0).length;
            this.preDeltaTime = Time.deltaTime;
            this.shot = false;
        }

        public override void Exit(ref StateManager manager)
        {
        }

        public override bool Transition(ref StateManager manager)
        {
            if(this.GetTime() == 0)
            {
                return false;
            }
            if (GetTime() % (this.enemy.bulletParameter.interval * this.enemy.bulletParameter.bulletNum) == 0)
            {
                manager.SetNextState((int)Enemy01.State.ENDATTACK);
                return true;
            }
            return false;
        }

        public override void Update()
        {
            if (GetTime() % this.enemy.bulletParameter.interval == 0)
            {
                Sound.PlaySE("shot1");
                this.CreateBullet();
            }
        }
        /**
         * brief    攻撃生成
         */ 
        private void CreateBullet()
        {
            for (int i = 0; i < this.enemy.bulletParameter.bulletData.Length; ++i)
            {
                Bullet.MagicBullet.Create(this.enemy, this.enemy.bulletParameter.bulletData[i], this.enemy.transform.position);
            }
        }
    }
    /**
     * brief    攻撃開始モーション
     */
    public class AttackStartState : BaseState
    {
        float preDeltaTime;
        float deltaTime;
        public float animationTime;
        public AttackStartState(Enemy01 enemy) : base(enemy)
        {
        }

        public override void Enter(ref StateManager manager)
        {
            this.enemy.parameter.animation.Play("AttackStart");
            this.animationTime = 0.283f;
            this.deltaTime = 0.0f;
        }

        public override void Exit(ref StateManager manager)
        {
        }

        public override bool Transition(ref StateManager manager)
        {
            if (deltaTime >= this.animationTime)
            {
                manager.SetNextState((int)Enemy01.State.ATTACK);
                return true;
            }
            return false;
        }

        public override void Update()
        {
            this.deltaTime += Time.deltaTime;
        }
    }
    /**
     * brief   攻撃終了モーション
     */
    public class AttackEndState : BaseState
    {
        float preDeltaTime;
        public float animationTime;
        float deltaTime;
        public AttackEndState(Enemy01 enemy) : base(enemy)
        {
        }

        public override void Enter(ref StateManager manager)
        {
            
            this.enemy.parameter.animation.Play("AttackEnd");
            this.animationTime = 0.283f;
            this.preDeltaTime = Time.deltaTime;
            deltaTime = 0.0f;
        }

        public override void Exit(ref StateManager manager)
        {
        }

        public override bool Transition(ref StateManager manager)
        {
            if(deltaTime >= this.animationTime)
            {
                if (base.enemy.TargetDistanceCheck() != null)
                {
                    manager.SetNextState((int)Enemy01.State.NORMAL);
                }
                else
                {
                    manager.SetNextState((int)Enemy01.State.ORBIT);
                }
                return true;
            }
            return false;
        }

        public override void Update()
        {
            deltaTime += Time.deltaTime;
        }
    }
    /**
     * brief    落下モーション
     */
    public class FallState : BaseState
    {
        public FallState(Enemy01 enemy) 
            : base(enemy)
        {
        }

        public override void Enter(ref StateManager manager)
        {
        }

        public override void Exit(ref StateManager manager)
        {
        }

        public override bool Transition(ref StateManager manager)
        {
            if(enemy.GetParameter().foot.CheckHit())
            {
                manager.SetNextState((int)Enemy01.State.NORMAL);
                return true;
            }
            return false;
        }

        public override void Update()
        {
        }
    }
    /**
     * brief    死亡モーション
     */
    public class DeathState : BaseState
    {
        public DeathState(Enemy01 enemy) : base(enemy)
        {
        }

        public override void Enter(ref StateManager manager)
        {
            this.enemy.parameter.animation.Play("Death");
            this.enemy.parameter.mover.Jump(6.0f);
            if (this.enemy.transform.localScale.x == 1)
            {
                this.enemy.transform.Rotate(new Vector3(0, 0, -30));
            }
            else if (this.enemy.transform.localScale.x == -1)
            {
                this.enemy.transform.Rotate(new Vector3(0, 0, 30));
            }
            var magicteam = this.enemy.GetComponent<MagicTeam>();
            if(magicteam != null)
            {
                magicteam.NotActive();
            }
            Sound.PlaySE("slashFlash");
            Effect.Get().CreateEffect("defeat", this.enemy.transform.position - Vector3.forward, Quaternion.identity, Vector3.one );
            this.enemy.GetData().hitPoint.SetDamageShutout(true);
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
            if(this.enemy.transform.localScale.x == 1)
            {
                this.enemy.SetMovePower(new Vector2(3, 0));
            }
            else if(this.enemy.transform.localScale.x == -1)
            {
                this.enemy.SetMovePower(new Vector2(-3, 0));
            }
            if(base.GetTime() >= 30)
            {
                base.enemy.KillMyself();
            }
        }
    }
    /**
    * brief    軌道確認
    */
    public class OrbitState : BaseState
    {
        public OrbitState(Enemy01 enemy) : base(enemy)
        {
        }

        public override void Enter(ref StateManager manager)
        {
            //this.enemy.parameter.animation.Play("Idle");
        }

        public override void Exit(ref StateManager manager)
        {
        }

        public override bool Transition(ref StateManager manager)
        {
            if (base.enemy.TargetDistanceCheck() != null)
            {
                enemy.bulletParameter.Search();
                manager.SetNextState((int)Enemy01.State.NORMAL);
                return true;
            }
            return false;
        }

        public override void Update()
        {
        }
    }
}