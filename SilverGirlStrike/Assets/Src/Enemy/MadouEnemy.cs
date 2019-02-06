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
        //アニメーションデータ
        private Animator animator;
        //移動システム
        private CharacterMover mover;
        void Start()
        {
            //取得
            animator = GetComponent<Animator>();
            mover = GetComponent<CharacterMover>();

            //初期化
            this.GetData().hitPoint.SetMaxHP(this.parameter.maxHP);
            this.GetData().hitPoint.Recover(this.parameter.maxHP);
            this.GetData().hitPoint.SetInvincible(this.parameter.damageInvincible);
            this.parameter.autoScaleChange.Init(this.gameObject);

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
        }
        public override bool Damage(AttackData attackData)
        {
            return false;
        }
        public override void MoveCharacter()
        {
        }
        public override void UpdateCharacter()
        {
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
            throw new System.NotImplementedException();
        }

        public override void Exit(ref StateManager manager)
        {
            throw new System.NotImplementedException();
        }

        public override bool Transition(ref StateManager manager)
        {
            throw new System.NotImplementedException();
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }

        public override void Exit(ref StateManager manager)
        {
            throw new System.NotImplementedException();
        }

        public override bool Transition(ref StateManager manager)
        {
            throw new System.NotImplementedException();
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }

        public override void Exit(ref StateManager manager)
        {
            throw new System.NotImplementedException();
        }

        public override bool Transition(ref StateManager manager)
        {
            throw new System.NotImplementedException();
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
        }
    }
    //攻撃開始ステート-----------------------------------------------------------------------------------------------------
    public class AttackBegin : BaseState
    {
        public AttackBegin(MadouEnemy enemy) : base(enemy)
        {
        }

        public override void Enter(ref StateManager manager)
        {
            throw new System.NotImplementedException();
        }

        public override void Exit(ref StateManager manager)
        {
            throw new System.NotImplementedException();
        }

        public override bool Transition(ref StateManager manager)
        {
            throw new System.NotImplementedException();
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }

        public override void Exit(ref StateManager manager)
        {
            throw new System.NotImplementedException();
        }

        public override bool Transition(ref StateManager manager)
        {
            throw new System.NotImplementedException();
        }

        public override void Update()
        {

        }
        private void CreateBullet()
        {
            Bullet.ChargeBullet.Create(this.enemy, this.enemy.bulletParameter.bulletData[0], this.enemy.transform.position, this.enemy.parameter.chargeBulletTime, this.enemy.parameter.bulletJumpPower);
        }
    }
    //攻撃終了ステート-----------------------------------------------------------------------------------------------------
    public class AttackEnd : BaseState
    {
        public AttackEnd(MadouEnemy enemy) : base(enemy)
        {
        }

        public override void Enter(ref StateManager manager)
        {
            throw new System.NotImplementedException();
        }

        public override void Exit(ref StateManager manager)
        {
            throw new System.NotImplementedException();
        }

        public override bool Transition(ref StateManager manager)
        {
            throw new System.NotImplementedException();
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }

        public override void Exit(ref StateManager manager)
        {
            throw new System.NotImplementedException();
        }

        public override bool Transition(ref StateManager manager)
        {
            throw new System.NotImplementedException();
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
        }
    }
}
