using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * file     Enemy02.cs
 * brief    敵Class
 * author   Shou Kaneko
 * date     2018/11/30
 * 状態
 *      待ち、移動
*/
namespace Enemy02
{
    public class Enemy02 : CharacterObject
    {
        /**
         * enum State
         */
         public enum State
        {
            WAIT,
            MOVE,
        }
        /**
         * brief    エネミー02用パラメータデータ
         */
        [System.Serializable]
        public class Enemy02Parameter
        {
            public CharacterMover mover;
            public Animation animation;
        }
        [System.Serializable]
        public class Moves
        {
            public GameObject targets;
            public int waitTime;
        }
        public Enemy02Parameter parameter;
        public Moves[] moves;
        private int nowNum;
        public float speed;
        private Vector2 move;
        public Enemy02()
            :base(10)
        {
            base.AddState((int)State.MOVE, new MoveState(this));
            base.AddState((int)State.WAIT, new WaitState(this));
            base.ChangeState((int)State.WAIT);
            this.move = new Vector2();
        }
        private void Start()
        {
            parameter.mover = GetComponent<CharacterMover>();
        }

        public override void UpdateCharacter()
        {
            this.UpdateState();
        }

        public override void Damage(AttackData attackData)
        {

        }

        public override void ApplyDamage()
        {

        }

        public override void MoveCharacter()
        {
            parameter.mover.UpdateVelocity(move.x, move.y, 0.0f, false);   
        }
        public Enemy02Parameter GetParameter()
        {
            return this.parameter;
        }
        public int GetNowNum()
        {
            return this.nowNum;
        }
        public void SetNowNum(int num)
        {
            this.nowNum = num;
        }
    }
    /**
     * brief    元となるState
     */
    public abstract class BaseState : StateParameter
    {
        public Enemy02 enemy;
        public BaseState(Enemy02 enemy)
        {
            this.enemy = enemy;
        }
    }
    /**
     * brief    移動State
     */
    public class MoveState : BaseState
    {
        public MoveState(Enemy02 enemy) 
            : base(enemy)
        {
        }

        public override void Enter(ref StateManager manager)
        {
            base.enemy.SetNowNum(base.enemy.GetNowNum() + 1);
            if(base.enemy.moves.Length <= base.enemy.GetNowNum())
            {
                base.enemy.SetNowNum(0);
            }
            Vector3 vec = base.enemy.moves[base.enemy.GetNowNum()].targets.transform.position - this.enemy.transform.position;
        }

        public override void Exit(ref StateManager manager)
        {
            base.ResetTime();
        }

        public override bool Transition(ref StateManager manager)
        {
            return false;
        }

        public override void Update()
        {
            base.TimeUp(1);
            
        }
    }
    /**
     * brief    待機State
     */
    public class WaitState : BaseState
    {
        public WaitState(Enemy02 enemy) 
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
            if(base.enemy.moves[base.enemy.GetNowNum()].waitTime > base.GetTime())
            {
                base.enemy.GetData().manager.SetNextState((int)Enemy02.State.MOVE);
            }
            return false;
        }

        public override void Update()
        {
            base.TimeUp(1);
        }
    }
}