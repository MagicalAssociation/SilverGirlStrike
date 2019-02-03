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
            ATTACK_TOP,
            ATTACK_CENTER,
            ATTACK_BOTTOM,
            DEATH
        }
        public enum AttackPosition : int
        {
            TOP = 0,
            CENTER = 1,
            BOTTOM = 2,
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
            this.bulletParameter.AdditionAngle(this.transform.localEulerAngles.z);
            //取得
            this.animator = GetComponent<Animator>();
            this.GetData().hitPoint.SetMaxHP(parameter.maxHP);
            this.GetData().hitPoint.Recover(parameter.maxHP);
            this.GetData().hitPoint.SetInvincible(this.parameter.damageInvincible);
            //!-ステートデータを登録＆初期値の設定-!//
            base.AddState((int)State.ORBIT, new OrbitState(this));
            base.AddState((int)State.WAIT, new WaitState(this));
            base.AddState((int)State.ATTACK_TOP, new AttackTop(this));
            base.AddState((int)State.ATTACK_CENTER, new AttackCenter(this));
            base.AddState((int)State.ATTACK_BOTTOM, new AttackBottom(this));
            base.AddState((int)State.DEATH, new DeathState(this));
            base.ChangeState((int)State.WAIT);
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
            Debug.Log(this.transform.eulerAngles);
        }
        private void SetAttackSuquence()
        {
            attackSequence = new State[3];
            if (this.parameter.flip)
            {
                attackSequence[0] = State.ATTACK_BOTTOM;
                attackSequence[1] = State.ATTACK_CENTER;
                attackSequence[2] = State.ATTACK_TOP;
            }
            else
            {
                attackSequence[0] = State.ATTACK_TOP;
                attackSequence[1] = State.ATTACK_CENTER;
                attackSequence[2] = State.ATTACK_BOTTOM;
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
        public bool NextAttackNumber()
        {
            this.attackNumber++;
            if (attackSequence.Length <= attackNumber)
            {
                this.attackNumber = 0;
                return false;
            }
            return true;
        }
        public int GetAttackNumber()
        {
            return this.attackNumber;
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
                manager.SetNextState((int)enemy.GetAttackSequence()[enemy.GetAttackNumber()]);
                return true;
            }
            return false;
        }

        public override void Update()
        {
        }
    }
    //攻撃1ステート
    public class AttackTop : BaseState
    {
        public AttackTop(ThreeShotEnemy enemy)
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
            if (this.GetTime() == 0)
            {
                return false;
            }
            //次の攻撃または待機へ移行
            if (GetTime() % (this.enemy.bulletParameter.interval * 1) == 0)
            {
                if(enemy.NextAttackNumber())
                {
                    manager.SetNextState((int)enemy.GetAttackSequence()[enemy.GetAttackNumber()]);
                }
                else
                {
                    manager.SetNextState((int)ThreeShotEnemy.State.WAIT);
                }
                return true;
            }
            return false;
        }

        public override void Update()
        {
            if(GetTime() % this.enemy.bulletParameter.interval == 0)
            {
                //攻撃生成
                Bullet.MagicBullet.Create(this.enemy, this.enemy.bulletParameter.bulletData[(int)ThreeShotEnemy.AttackPosition.TOP], this.enemy.transform.position);
            }
        }
    }
    //攻撃2ステート
    public class AttackCenter : BaseState
    {
        public AttackCenter(ThreeShotEnemy enemy)
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
            if (this.GetTime() == 0)
            {
                return false;
            }
            //次の攻撃または待機へ移行
            if (GetTime() % (this.enemy.bulletParameter.interval * 1) == 0)
            {
                if (enemy.NextAttackNumber())
                {
                    manager.SetNextState((int)enemy.GetAttackSequence()[enemy.GetAttackNumber()]);
                }
                else
                {
                    manager.SetNextState((int)ThreeShotEnemy.State.WAIT);
                }
                return true;
            }
            return false;
        }

        public override void Update()
        {
            if (GetTime() % this.enemy.bulletParameter.interval == 0)
            {
                //攻撃生成
                Bullet.MagicBullet.Create(this.enemy, this.enemy.bulletParameter.bulletData[(int)ThreeShotEnemy.AttackPosition.CENTER], this.enemy.transform.position);
            }
        }
    }
    //攻撃１ステート
    public class AttackBottom : BaseState
    {
        public AttackBottom(ThreeShotEnemy enemy)
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
            if (this.GetTime() == 0)
            {
                return false;
            }
            //次の攻撃または待機へ移行
            if (GetTime() % (this.enemy.bulletParameter.interval * 1) == 0)
            {
                if (enemy.NextAttackNumber())
                {
                    manager.SetNextState((int)enemy.GetAttackSequence()[enemy.GetAttackNumber()]);
                }
                else
                {
                    manager.SetNextState((int)ThreeShotEnemy.State.WAIT);
                }
                return true;
            }
            return false;
        }

        public override void Update()
        {
            if (GetTime() % this.enemy.bulletParameter.interval == 0)
            {
                //攻撃生成
                Bullet.MagicBullet.Create(this.enemy, this.enemy.bulletParameter.bulletData[(int)ThreeShotEnemy.AttackPosition.BOTTOM], this.enemy.transform.position);
            }
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
    //死亡モーション
    public class DeathState : BaseState
    {
        public DeathState(ThreeShotEnemy enemy) : base(enemy)
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
            this.enemy.KillMyself();
        }
    }
}
