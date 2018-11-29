using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * file     Enemy01.cs
 * brief    敵Class
 * author   Shou Kaneko
 * date     2018/11/27
 * 状態
 *      立ち、攻撃、受け、やられ、落下
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
            FALL,
        }
        /**
         * enum Direction 向き
         */
        public enum Direction
        {
            LEFT,
            RIGHT,
        }
        [System.Serializable]
        public class Enemy01Parameter
        {
            //! 生成する攻撃オブジェクト
            public Bullet.MagicBullet attackObject;
            //! 移動用class
            public CharacterMover mover;
            //! 地面判定
            public Foot foot;
            //! Animation
            public Animation animation;
            //! 向き
            public Direction direction;
        }
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
            //!-ステートデータを登録＆初期値の設定-!//
            base.AddState((int)State.NORMAL, new NormalState(this));
            base.AddState((int)State.ATTACK, new AttackState(this));
            base.AddState((int)State.FALL, new FallState(this));
            base.ChangeState((int)State.NORMAL);

            this.parameter = new Enemy01Parameter();
            this.parameter.direction = Direction.LEFT;
        }
        // Update is called once per frame
        private void Start()
        {
            parameter.mover = GetComponent<CharacterMover>();
            parameter.foot = transform.GetComponentInChildren<Foot>();
        }
        public override void UpdateCharacter()
        {
            this.UpdateState();
        }
        public override void Damage(AttackData data)
        {
        }
        public override void ApplyDamage()
        {
        }
        public override void MoveCharacter()
        {
            parameter.mover.UpdateVelocity(0, 0, this.gravity, parameter.foot.CheckHit());
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
            if(!enemy.GetParameter().foot.CheckHit())
            {
                manager.SetNextState((int)Enemy01.State.FALL);
                return true;
            }
            //30count経過したら攻撃モーションへ移行
            if(base.GetTime() > 200)
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
                Bullet.MagicBullet bullet = Object.Instantiate(base.enemy.GetParameter().attackObject, base.enemy.transform.position, Quaternion.identity) as Bullet.MagicBullet;
                bullet.SetAttackData(new AttackData(base.enemy));
                bullet.GetAttackData().power = 3;
                Vector2 dire = new Vector2(1, 0);
                if (this.enemy.GetParameter().direction == Enemy01.Direction.LEFT)
                {
                    dire.x *= -1.0f;
                }
                bullet.GetAttackData().direction = dire;
            }
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
            base.ResetTime();
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
            base.TimeUp(1);
        }
    }
}