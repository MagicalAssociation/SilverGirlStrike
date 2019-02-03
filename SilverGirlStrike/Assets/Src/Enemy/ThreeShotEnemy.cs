using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//壁にくっついて弾を3連続で撃つEnemy

/*
 * 起動待ち状態からプレイヤーを検知したら攻撃へ移行、
 * その後近くにまだプレイヤーがいるなら待機して攻撃
 * いなければ起動待ちへ移行
*/
namespace ThreeShot
{
    public class ThreeShotEnemy : CharacterObject
    {
        public enum State
        {
            WAIT,
            ORBIT,
            ATTACK1,
            ATTACK2,
            ATTACK3,
            DEATH
        }
        //プレイヤーが起動範囲内にいるかを検索するためのサーチ
        public TargetSearch targetSearch;
        [System.Serializable]
        public class Parameter
        {
            //反転
            public bool flip;
            //攻撃間隔
            public int attackInterval;
            //HP
            public int maxHP;
            //無敵時間
            public int damageInvincible;
            //攻撃判定
            public NarrowAttacker[] narrowAttackers;
        }
        //生成する弾の情報
        public Bullet.BulletParameter bulletParameter;
        //個別パラメータ
        public Parameter parameter;
        //アニメーション
        Animator animator;
        //攻撃の順番
        private State[] attackSequence;
        //攻撃準
        private int attackNumber;
        void Start()
        {
            //初期化
            attackNumber = 0;
            this.SetAttackSuquence();
            //取得
            this.animator = GetComponent<Animator>();
            this.GetData().hitPoint.SetMaxHP(parameter.maxHP);
            this.GetData().hitPoint.Recover(parameter.maxHP);
            this.GetData().hitPoint.SetInvincible(this.parameter.damageInvincible);
            //!-ステートデータを登録＆初期値の設定-!//
            base.AddState((int)State.ORBIT, new OrbitState(this));
            base.AddState((int)State.WAIT, new WaitState(this));
            base.AddState((int)State.ATTACK1, new Attack1(this));
            base.AddState((int)State.ATTACK2, new Attack2(this));
            base.AddState((int)State.ATTACK3, new Attack3(this));
            base.ChangeState((int)State.ORBIT);
        }
        public override void ApplyDamage()
        {
            //HPなくなったら死亡へ
            this.GetData().hitPoint.DamageUpdate();
            if (this.GetData().hitPoint.GetHP() <= 0 && base.GetData().stateManager.GetNowStateNum() != (int)State.DEATH)
            {
                base.ChangeState((int)State.DEATH);
            }
        }

        public override bool Damage(AttackData attackData)
        {
            return this.GetData().hitPoint.Damage(attackData.power, attackData.chain);
        }

        public override void MoveCharacter()
        {
        }

        public override void UpdateCharacter()
        {
            if(GetData().stateManager.GetNowStateNum() != (int)State.DEATH)
            {
                this.parameter.narrowAttackers[0].StartAttack();
            }
            this.UpdateState();
        }
        private void SetAttackSuquence()
        {
            attackSequence = new State[3];
            if (this.parameter.flip)
            {
                attackSequence[0] = State.ATTACK3;
                attackSequence[1] = State.ATTACK2;
                attackSequence[2] = State.ATTACK1;
            }
            else
            {
                attackSequence[0] = State.ATTACK1;
                attackSequence[1] = State.ATTACK2;
                attackSequence[2] = State.ATTACK3;
            }
        }
        //アニメーターの取得
        public Animator GetAnimator()
        {
            return this.animator;
        }
        //攻撃順の取得
        public State[] GetAttackSequence()
        {
            return this.attackSequence;
        }
    }
    //元ステート
    public abstract class BaseState : StateParameter
    {
        public ThreeShotEnemy enemy;
        public BaseState(ThreeShotEnemy enemy)
        {
            this.enemy = enemy;
        }
    }
    //待機ステート
    public class WaitState : BaseState
    {
        public WaitState(ThreeShotEnemy enemy)
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
            if(GetTime() > enemy.parameter.attackInterval)
            {
                manager.SetNextState((int)enemy.GetAttackSequence()[0]);
            }
            return false;
        }

        public override void Update()
        {
        }
    }
    //攻撃1ステート
    public class Attack1 : BaseState
    {
        public Attack1(ThreeShotEnemy enemy)
            :base(enemy)
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
            return false;
        }

        public override void Update()
        {
        }
    }
    //攻撃2ステート
    public class Attack2 : BaseState
    {
        public Attack2(ThreeShotEnemy enemy)
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
            return false;
        }

        public override void Update()
        {
        }
    }
    //攻撃１ステート
    public class Attack3 : BaseState
    {
        public Attack3(ThreeShotEnemy enemy)
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
            return false;
        }

        public override void Update()
        {
        }
    }
    //起動待ち
    public class OrbitState : BaseState
    {
        public OrbitState(ThreeShotEnemy enemy)
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
            if(base.enemy.targetSearch.Search() != null)
            {
                manager.SetNextState((int)ThreeShotEnemy.State.WAIT);
                return true;
            }
            return false;
        }

        public override void Update()
        {
        }
    }
}
