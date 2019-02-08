using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MadouEnemy
{
    //魔導弾を打つ敵-----------------------------------------------------------------------------------------------------
    public class MadouEnemy : CharacterObject
    {
        //行動ステート
        public enum State
        {
            //待機
            WAIT,
            //起動待ち
            ORBIT,
            //落下
            FALL,
            //攻撃開始
            ATTACK_BEGIN,
            //攻撃モーション
            ATTACK,
            //攻撃終了
            ATTACK_END,
            //死亡
            DEATH,
        }
        [System.Serializable]
        public class Parameter
        {
            //HP
            public int maxHP;
            //攻撃間隔
            public int attackInterval;
            //無敵時間
            public int damageInvincible;
            //重力
            public float gravity;
            //重力判定
            public bool enableGravity;
            //攻撃チャージ時間
            public int chargeBulletTime;
            //攻撃のジャンプ力
            public float bulletJumpPower;
            //攻撃弾の重力値
            public float bulletGravity;
            //攻撃開始アニメーションタイム
            public float attackBeginAnimTime;
            //攻撃終了アニメーションタイム
            public float attackEndAnimTime;
            //足元オブジェクト
            public Foot foot;
            //向き自動変更
            public Enemy.AutoScaleChange autoScaleChange;
            //攻撃判定
            public NarrowAttacker[] narrowAttackers;
        }
        //固有パラメータ
        public Parameter parameter;
        //生成攻撃データ
        public Bullet.BulletParameter bulletParameter;
        //プレイヤーの検索
        public TargetSearch targetSearch;
        //アニメーションデータ
        private Animator animator;
        //移動システム
        private CharacterMover mover;
        //移動力
        private Vector2 movePower;
        void Start()
        {
            //生成
            movePower = new Vector2();
            //取得
            animator = GetComponent<Animator>();
            mover = GetComponent<CharacterMover>();

            //初期化
            this.GetData().hitPoint.SetMaxHP(this.parameter.maxHP);
            this.GetData().hitPoint.Recover(this.parameter.maxHP);
            this.GetData().hitPoint.SetInvincible(this.parameter.damageInvincible);
            this.parameter.autoScaleChange.Init(this.gameObject);
            animator.Play("wait");

            //!-ステートデータを登録＆初期値の設定-!//
            base.AddState((int)State.WAIT, new WaitState(this));
            base.AddState((int)State.FALL, new FallState(this));
            base.AddState((int)State.ORBIT, new OrbitState(this));
            base.AddState((int)State.ATTACK_BEGIN, new AttackBegin(this));
            base.AddState((int)State.ATTACK, new AttackState(this));
            base.AddState((int)State.ATTACK_END, new AttackEnd(this));
            base.AddState((int)State.DEATH, new DeathState(this));
            base.ChangeState((int)State.ORBIT);
        }
        public override void ApplyDamage()
        {
            //HPなくなったら死亡へ
            if (this.GetData().hitPoint.GetHP() <= 0 && base.GetData().stateManager.GetNowStateNum() != (int)State.DEATH)
            {
                base.ChangeState((int)State.DEATH);
            }
            else
            {
                this.GetData().hitPoint.DamageUpdate();
            }
        }
        public override bool Damage(AttackData attackData)
        {
            return this.GetData().hitPoint.Damage(attackData.power, attackData.chain); ;
        }
        public override void MoveCharacter()
        {
            if (this.GetData().stateManager.GetNowStateNum() != (int)State.ORBIT)
            {
                mover.UpdateVelocity(this.movePower.x, this.movePower.y, this.parameter.enableGravity ? this.parameter.gravity : 0.0f, parameter.foot.CheckHit());
                this.movePower = Vector2.zero;
            }
            this.parameter.autoScaleChange.DirectionUpdate();
        }
        public override void UpdateCharacter()
        {
            //判定垂れ流し
            if (this.GetData().stateManager.GetNowStateNum() != (int)State.DEATH)
            {
                this.parameter.narrowAttackers[0].StartAttack();
            }
            this.UpdateState();
        }
        public void SetMovePower(Vector2 move)
        {
            this.movePower = move;
        }
        public Animator GetAnimator()
        {
            return this.animator;
        }
        public CharacterMover GetCharacterMover()
        {
            return this.mover;
        }
    }
    //元ステート-----------------------------------------------------------------------------------------------------
    public abstract class BaseState : StateParameter
    {
        public MadouEnemy enemy;
        public BaseState(MadouEnemy enemy)
        {
            this.enemy = enemy;
        }
    }
    //待機ステート-----------------------------------------------------------------------------------------------------
    public class WaitState : BaseState
    {
        public WaitState(MadouEnemy enemy) : base(enemy)
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
            if (!enemy.parameter.foot.CheckHit() && this.enemy.parameter.enableGravity)
            {
                manager.SetNextState((int)MadouEnemy.State.FALL);
                return true;
            }
            //一定count経過したら攻撃モーションへ移行
            //攻撃データの弾数が０の場合移行をしない
            if (base.enemy.bulletParameter.bulletNum != 0 && base.GetTime() > base.enemy.parameter.attackInterval)
            {
                manager.SetNextState((int)MadouEnemy.State.ATTACK_BEGIN);
                return true;
            }
            return false;
        }

        public override void Update()
        {
        }
    }
    //落下ステー-----------------------------------------------------------------------------------------------------
    public class FallState : BaseState
    {
        public FallState(MadouEnemy enemy) : base(enemy)
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
            if (enemy.parameter.foot.CheckHit())
            {
                manager.SetNextState((int)MadouEnemy.State.WAIT);
                return true;
            }
            return false;
        }

        public override void Update()
        {
        }
    }
    //起動待ちステート-----------------------------------------------------------------------------------------------------
    public class OrbitState : BaseState
    {
        public OrbitState(MadouEnemy enemy) : base(enemy)
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
            if(enemy.targetSearch.Search() != null && enemy.targetSearch.Search().tag == "Player")
            {
                enemy.bulletParameter.Search();
                manager.SetNextState((int)MadouEnemy.State.WAIT);
                return true;
            }
            return false;
        }

        public override void Update()
        {
        }
    }
    //攻撃開始ステート-----------------------------------------------------------------------------------------------------
    public class AttackBegin : BaseState
    {
        float deltaTime;
        public AttackBegin(MadouEnemy enemy) : base(enemy)
        {
        }

        public override void Enter(ref StateManager manager)
        {
            this.deltaTime = 0.0f;
            enemy.GetAnimator().Play("attackBegin");
        }

        public override void Exit(ref StateManager manager)
        {
        }

        public override bool Transition(ref StateManager manager)
        {
            if(deltaTime >= enemy.parameter.attackBeginAnimTime)
            {
                manager.SetNextState((int)MadouEnemy.State.ATTACK);
                return true;
            }
            return false;
        }

        public override void Update()
        {
            deltaTime += Time.deltaTime;
        }
    }
    //攻撃ステート-----------------------------------------------------------------------------------------------------
    public class AttackState : BaseState
    {
        public AttackState(MadouEnemy enemy) : base(enemy)
        {
        }

        public override void Enter(ref StateManager manager)
        {
            enemy.GetAnimator().Play("attack");
        }

        public override void Exit(ref StateManager manager)
        {
        }

        public override bool Transition(ref StateManager manager)
        {
            if (this.GetTime() == 0)
            {
                return false;
            }
            if (GetTime() % ((this.enemy.bulletParameter.interval + this.enemy.parameter.chargeBulletTime) * this.enemy.bulletParameter.bulletNum) == 0)
            {
                manager.SetNextState((int)MadouEnemy.State.ATTACK_END);
                return true;
            }
            return false;
        }

        public override void Update()
        {
            //初期フレームで１つ生成するために-1
            if ((GetTime() - 1) % (this.enemy.bulletParameter.interval + this.enemy.parameter.chargeBulletTime) == 0)
            {
                CreateBullet();
            }
        }
        private void CreateBullet()
        {
            for (int i = 0; i < this.enemy.bulletParameter.bulletData.Length; ++i)
            {
                Bullet.ChargeBullet.Create(this.enemy, this.enemy.bulletParameter.bulletData[i], this.enemy.transform.position, this.enemy.parameter.chargeBulletTime, this.enemy.parameter.bulletJumpPower,enemy.parameter.bulletGravity);
            }
        }
    }
    //攻撃終了ステート-----------------------------------------------------------------------------------------------------
    public class AttackEnd : BaseState
    {
        float deltaTime;
        public AttackEnd(MadouEnemy enemy) : base(enemy)
        {
        }

        public override void Enter(ref StateManager manager)
        {
            this.deltaTime = 0.0f;
            enemy.GetAnimator().Play("attackEnd");
        }

        public override void Exit(ref StateManager manager)
        {
            enemy.GetAnimator().Play("wait");
        }

        public override bool Transition(ref StateManager manager)
        {
            if (deltaTime >= enemy.parameter.attackEndAnimTime)
            {
                //待機か軌道待ちへ移動
                if (enemy.targetSearch.Search() == null || enemy.targetSearch.Search().tag != "Player")
                {
                    manager.SetNextState((int)MadouEnemy.State.ORBIT);
                }
                else
                {
                    manager.SetNextState((int)MadouEnemy.State.WAIT);
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
    //死亡ステート-----------------------------------------------------------------------------------------------------
    public class DeathState : BaseState
    {
        public DeathState(MadouEnemy enemy) : base(enemy)
        {
        }

        public override void Enter(ref StateManager manager)
        {
            this.enemy.GetAnimator().Play("death");
            this.enemy.GetCharacterMover().Jump(6.0f);
            if (this.enemy.transform.localScale.x == 1)
            {
                this.enemy.transform.Rotate(new Vector3(0, 0, -30));
            }
            else if (this.enemy.transform.localScale.x == -1)
            {
                this.enemy.transform.Rotate(new Vector3(0, 0, 30));
            }
            var magicteam = this.enemy.GetComponent<MagicTeam>();
            if (magicteam != null)
            {
                magicteam.NotActive();
            }
            Sound.PlaySE("slashFlash");
            Effect.Get().CreateEffect("defeat", this.enemy.transform.position - Vector3.forward * 2.0f, Quaternion.identity, Vector3.one);
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
            if (this.enemy.transform.localScale.x == 1)
            {
                this.enemy.SetMovePower(new Vector2(3, 0));
            }
            else if (this.enemy.transform.localScale.x == -1)
            {
                this.enemy.SetMovePower(new Vector2(-3, 0));
            }
            if (base.GetTime() >= 30)
            {
                base.enemy.KillMyself();
            }
        }
    }
}
