using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bullet
{
    public class MagicBullet : CharacterObject
    {
        public enum State
        {
            NORMAL,
        }
        AttackData data;
        public float movePower;
        public CharacterManager manager;
        private void Start()
        {
            manager.AddCharacter(this);
            base.AddState((int)State.NORMAL, new NormalState(this));
            base.ChangeState((int)State.NORMAL);
            this.movePower = 0.3f;
        }
        /**
         * brief    AttackData登録
         * param[in] AttackData data 攻撃データ
         */
        public void SetAttackData(AttackData data)
        {
            this.data = data;
        }
        /**
         * brief    AttackDataの取得
         * return AttackData 攻撃データ
         */
        public AttackData GetAttackData()
        {
            return this.data;
        }

        public override void UpdateCharacter()
        {
            GetData().manager.Update();
        }

        public override void Damage(AttackData attackData)
        {
        }

        public override void ApplyDamage()
        {
        }

        public override void MoveCharacter()
        {
            this.transform.position += new Vector3(this.data.direction.x * this.movePower, this.data.direction.y * this.movePower);
        }
    }
    /**
     * brief    元State
     */ 
    public abstract class BaseState : StateParameter
    {
        public MagicBullet bullet;
        public BaseState(MagicBullet bullet)
        {
            this.bullet = bullet;
        }
    }
    /**
     * brief    通常State
     */
    public class NormalState : BaseState
    {
        public NormalState(MagicBullet bullet) 
            : base(bullet)
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
            base.TimeUp(1);
            if(base.GetTime() > 10)
            {
                base.bullet.manager.DeleteCharacter(base.bullet.name);
            }
        }
    }
}