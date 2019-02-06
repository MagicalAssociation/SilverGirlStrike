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
            ATTACK_SHOT,
            ATTACK_BEGIN,
            ATTACK_END,
            DEATH
        }
        public enum AttackPosition : int
        {
            TOP = 0,
            CENTER = 1,
            BOTTOM = 2,
        }
        public class AnimationName
        {
            public string beginName;
            public string shotName;
            public string endName;
            public AnimationName(string begin,string shot,string end)
            {
                beginName = begin;
                shotName = shot;
                endName = end;
            }
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
            //攻撃退場間隔
            public int exitInterval;
            //退場アニメーション時間
            public float endAnimTime;
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
        private AttackPosition[] attackSequence;
        public Dictionary<AttackPosition, AnimationName> animationName;
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
            //アニメーションの登録
            animationName = new Dictionary<AttackPosition, AnimationName>();
            animationName[AttackPosition.TOP] = new AnimationName("topBegin", "topShot", "topEnd");
            animationName[AttackPosition.CENTER] = new AnimationName("centerBegin", "centerShot", "centerEnd");
            animationName[AttackPosition.BOTTOM] = new AnimationName("bottomBegin", "bottomShot", "bottomEnd");
            //!-ステートデータを登録＆初期値の設定-!//
            base.AddState((int)State.ORBIT, new OrbitState(this));
            base.AddState((int)State.WAIT, new WaitState(this));
            base.AddState((int)State.ATTACK_BEGIN, new AttackBegin(this));
            base.AddState((int)State.ATTACK_SHOT, new AttackShot(this));
            base.AddState((int)State.ATTACK_END, new AttackEnd(this));
            base.AddState((int)State.DEATH, new DeathState(this));
            base.ChangeState((int)State.ATTACK_BEGIN);
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
            attackSequence = new AttackPosition[3];
            if (this.parameter.flip)
            {
                attackSequence[0] = AttackPosition.BOTTOM;
                attackSequence[1] = AttackPosition.CENTER;
                attackSequence[2] = AttackPosition.TOP;
            }
            else
            {
                attackSequence[0] = AttackPosition.TOP;
                attackSequence[1] = AttackPosition.CENTER;
                attackSequence[2] = AttackPosition.BOTTOM;
            }
        }
        //アニメーターの取得
        public Animator GetAnimator()
        {
            return this.animator;
        }
        //攻撃順の取得
        public AttackPosition[] GetAttackSequence()
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
                manager.SetNextState((int)ThreeShotEnemy.State.ATTACK_BEGIN);
                return true;
            }
            return false;
        }

        public override void Update()
        {
        }
    }
    //攻撃開始
    public class AttackBegin : BaseState
    {
        ThreeShotEnemy.AttackPosition now;
        public AttackBegin(ThreeShotEnemy enemy) : base(enemy)
        {

        }

        public override void Enter(ref StateManager manager)
        {
            now = enemy.GetAttackSequence()[enemy.GetAttackNumber()];
            enemy.GetAnimator().Play(enemy.animationName[now].beginName);
        }

        public override void Exit(ref StateManager manager)
        {
        }

        public override bool Transition(ref StateManager manager)
        {
            if (GetTime() > enemy.parameter.attackInterval)
            {
                manager.SetNextState((int)ThreeShotEnemy.State.ATTACK_SHOT);
                return true;
            }
            return false;
        }

        public override void Update()
        {
        }
    }
    //攻撃
    public class AttackShot : BaseState
    {
        ThreeShotEnemy.AttackPosition now;
        public AttackShot(ThreeShotEnemy enemy) : base(enemy)
        {
        }

        public override void Enter(ref StateManager manager)
        {
            now = enemy.GetAttackSequence()[enemy.GetAttackNumber()];
            enemy.GetAnimator().Play(enemy.animationName[now].shotName);
        }

        public override void Exit(ref StateManager manager)
        {
        }

        public override bool Transition(ref StateManager manager)
        {
            //次の攻撃または待機へ移行
            if (GetTime() > enemy.parameter.exitInterval)
            {
                manager.SetNextState((int)ThreeShotEnemy.State.ATTACK_END);
                return true;
            }
            return false;
        }

        public override void Update()
        {
            if (GetTime() == 1)
            {
                //攻撃生成
                Bullet.MagicBullet.Create(this.enemy, this.enemy.bulletParameter.bulletData[(int)now], this.enemy.transform.position);
            }
        }
    }
    //攻撃終了
    public class AttackEnd : BaseState
    {
        ThreeShotEnemy.AttackPosition now;
        float time;
        float nowTime;
        public AttackEnd(ThreeShotEnemy enemy) : base(enemy)
        {
            time = enemy.parameter.endAnimTime;
        }

        public override void Enter(ref StateManager manager)
        {
            now = enemy.GetAttackSequence()[enemy.GetAttackNumber()];
            enemy.GetAnimator().Play(enemy.animationName[now].endName);
            nowTime = 0.0f;
        }
        public override void Exit(ref StateManager manager)
        {
        }
        public override bool Transition(ref StateManager manager)
        {
            if (time < nowTime)
            {
                enemy.NextAttackNumber();
                manager.SetNextState((int)ThreeShotEnemy.State.ATTACK_BEGIN);
                return true;
            }
            return false;
        }
        public override void Update()
        {
            nowTime += Time.deltaTime;
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
