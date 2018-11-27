using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * file     Enemy01.cs
 * brief    敵Class
 * author   Shou Kaneko
 * date     2018/11/27
 * 状態
 *      立ち、攻撃、受け、やられ
*/
namespace Enemy
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
        }
        [System.Serializable]
        public class Enemy01Parameter
        {
            //! 生成する攻撃オブジェクト
            public GameObject attackObject;
        }
        //! 移動用class
        CharacterMover mover;
        //! 地面判定
        Foot foot;
        //! 重力
        public float gravity;
        //! Parameter
        public Enemy01Parameter parameter;
        /**
         * brief    constructor
         */
        public Enemy01()
            :base(10)
        {
            base.AddState((int)State.NORMAL, new NormalState(this));
            base.AddState((int)State.ATTACK, new AttackState(this));
            base.ChangeState((int)State.NORMAL);
            this.parameter = new Enemy01Parameter();
        }
        // Update is called once per frame
        private void Start()
        {
            mover = GetComponent<CharacterMover>();
            foot = transform.GetComponentInChildren<Foot>();
        }
        public override void UpdateCharacter()
        {
            this.UpdateState();
            Debug.Log(GetData().manager.GetNextStateNum());
            mover.UpdateVelocity(0, 0, this.gravity, foot.CheckHit());
        }
        public override void Damage(AttackData data)
        {
        }
        public override void ApplyDamage()
        {
        }
        public override void MoveCharacter()
        {
        }
        /**
         * brief    Enemy01専用のパラメータデータ取得
         */
         public Enemy01Parameter GetParameter()
        {
            return this.parameter;
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
        }

        public override void Exit(ref StateManager manager)
        {
            base.ResetTime();
        }

        public override bool Transition(ref StateManager manager)
        {
            //30count経過したら攻撃モーションへ移行
            if(base.GetTime() > 30)
            {
                manager.SetNextState((int)Enemy01.State.ATTACK);
                return true;
            }
            return false;
        }

        public override void Update()
        {
            base.TimeUp(1);
        }
    }
    /**
     * brief    攻撃モーション
     */
    public class AttackState : BaseState
    {
        Vector2 direction;
        public AttackState(Enemy01 enemy) 
            : base(enemy)
        {
        }

        public override void Enter(ref StateManager manager)
        {
        }

        public override void Exit(ref StateManager manager)
        {
            base.ResetTime();
        }

        public override bool Transition(ref StateManager manager)
        {
            if(base.GetTime() > 10)
            {
                manager.SetNextState((int)Enemy01.State.NORMAL);
                return true;
            }
            return false;
        }

        public override void Update()
        {
            base.TimeUp(1);
            if(base.GetTime() == 4)
            {
                //攻撃生成
                Object.Instantiate(base.enemy.GetParameter().attackObject, base.enemy.transform.position, Quaternion.identity);
            }
        }
    }
}