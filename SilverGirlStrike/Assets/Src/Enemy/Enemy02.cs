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
/**
 * Inspectorの設定値の説明
 * Parameter.MaxHP 最大HP
 * Parameter.Animation アニメーションデータ
 * Parameter.Power ダメージ値
 * Moves.Target 移動先GameObject
 * Moves.WaitTime ↑へ移動する前に待機している時間
 * Moves.MoveTime ↑へ移動する時間を指定
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
            //! 待機
            WAIT,
            //! 移動
            MOVE,
            //! 死亡
            DEATH,
        }
        /**
         * enum Direction 向き
         */
         public enum Direction
        {
            LEFT = 1,
            RIGHT = -1
        }
        /**
         * brief    エネミー02用パラメータデータ
         */
        [System.Serializable]
        public class Enemy02Parameter
        {
            //! 最大HP
            public int maxHP;
            //! アニメーション用class
            public Animator animation;
            //! ダメージ量
            public int power;
            //! 向き情報
            public Direction direction;
            //! 移動用class
            public CharacterMover charactermover;
        }
        /**
         * brief    移動用変数をまとめたclass
         */ 
        [System.Serializable]
        public class Moves
        {
            //! 移動先ターゲット
            public GameObject targets;
            //! 待ち時間
            public int waitTime;
            //! ターゲットまでの移動時間
            public float moveTime;
        }
        //! 固有パラメータデータ
        public Enemy02Parameter parameter;
        //! 移動用データの配列
        public Moves[] moves;
        //! 現在の移動データ番号
        private int nowNum;
        //! 位置情報 !移動はこの値を書き変え、そのままGameObjectに渡します! ※Easingを使って移動しようとしたらこの形が計算少なく済んだ
        private Vector2 pos;
        //! 前回の位置データ
        private Vector2 prePos;
        //! 攻撃データ
        private AttackData attackData;
        //! 自身のBoxの当たり判定
        private BoxCollider2D collider;
        public Enemy02()
            //! life
            :base(10)
        {
            this.pos = new Vector2();
            this.nowNum = 0;
            this.attackData = new AttackData(this);
        }
        private void Start()
        {
            //今の位置をいれておく
            this.pos = this.transform.localPosition;
            this.prePos = this.pos;
            this.collider =  GetComponent<BoxCollider2D>();
            this.attackData.power = this.parameter.power;
            this.parameter.animation = GetComponent<Animator>();
            this.parameter.charactermover = GetComponent<CharacterMover>();
            this.GetData().hitPoint.SetMaxHP(this.parameter.maxHP);
            //各ステートを登録&適用
            base.AddState((int)State.MOVE, new MoveState(this));
            base.AddState((int)State.WAIT, new WaitState(this));
            base.AddState((int)State.DEATH, new DeathState(this));
            base.ChangeState((int)State.WAIT);
        }

        public override void UpdateCharacter()
        {
            this.UpdateState();
            //プレイヤーと当たったらダメージ処理
            Collider2D hit = this.Hit();
            if(hit != null)
            {
                if (hit.GetComponent<CharacterObject>() != null)
                {
                    hit.GetComponent<CharacterObject>().Damage(this.attackData);
                }
            }
        }

        public override void Damage(AttackData attackData)
        {
            this.GetData().hitPoint.Damage(attackData.power, attackData.chain);
        }

        public override void ApplyDamage()
        {
            this.GetData().hitPoint.DamageUpdate();
            if (this.GetData().hitPoint.GetHP() <= 0 && base.GetData().stateManager.GetNowStateNum() != (int)State.DEATH)
            {
                base.ChangeState((int)State.DEATH);
            }
        }

        public override void MoveCharacter()
        {
            this.prePos = this.transform.localPosition;
            this.transform.localPosition = new Vector3(pos.x, pos.y, this.transform.position.z);
            //this.parameter.charactermover.UpdateVelocity(pos.x - this.transform.localPosition.x, pos.y - this.transform.localPosition.y, 0.0f, true);
            //if (this.transform.localPosition.x > this.prePos.x)
            //{
            //    this.parameter.direction = Direction.RIGHT;
            //}
            //else if (this.transform.localPosition.x < this.prePos.x)
            //{
            //    this.parameter.direction = Direction.LEFT;
            //}
            //this.transform.localScale = new Vector3((int)this.parameter.direction, 1, 1);
        }
        /**
         * brief    固有データを取得する
         * return Enemy02Parameter ThisParameter
         */ 
        public Enemy02Parameter GetParameter()
        {
            return this.parameter;
        }
        /**
         * brief    現在の配列番号
         * return int 配列番号
         */ 
        public int GetNowNum()
        {
            return this.nowNum;
        }
        /**
         * brief    配列番号を指定
         * param[in] int num 指定値
         */ 
        public void SetNowNum(int num)
        {
            this.nowNum = num;
        }
        /**
         * brief    位置を指定
         * param[in] Vector2 move 移動後位置
         */ 
        public void SetPos(Vector2 move)
        {
            this.pos = move;
        }
        /**
         * brief    当たり判定
         * return Collider2D 当たったオブジェクト
         */ 
        private Collider2D Hit()
        {
            return Physics2D.OverlapBox(
                    this.collider.transform.position,
                    this.collider.size,
                    this.transform.eulerAngles.z,
                    (int)M_System.LayerName.PLAYER
                    );
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
        //! x座標Easing
        Easing move_x;
        //! y座標Easing
        Easing move_y;
        //! 移動量とかターゲット位置とかの取得用
        Enemy02.Moves moveData;
        public MoveState(Enemy02 enemy) 
            : base(enemy)
        {
            this.move_x = new Easing();
            this.move_y = new Easing();
        }

        public override void Enter(ref StateManager manager)
        {
            //次自分が向かうMovesのデータを取得
            this.moveData = base.enemy.moves[base.enemy.GetNowNum()];
            //Easingを登録
            move_x.Set(this.enemy.transform.position.x, this.moveData.targets.transform.position.x - this.enemy.transform.position.x);
            move_y.Set(this.enemy.transform.position.y, this.moveData.targets.transform.position.y - this.enemy.transform.position.y);
            this.enemy.parameter.animation.Play("Move");
        }

        public override void Exit(ref StateManager manager)
        {
            //各値リセット
            this.move_x.ResetTime();
            this.move_y.ResetTime();
            //次の移動配列へ、配列の最後だったら0に戻す
            base.enemy.SetNowNum(base.enemy.GetNowNum() + 1);
            if (base.enemy.moves.Length <= base.enemy.GetNowNum())
            {
                base.enemy.SetNowNum(0);
            }
        }

        public override bool Transition(ref StateManager manager)
        {
            //移動が終わったらWAITへ移行
            if (!this.move_x.IsPlay() && !this.move_y.IsPlay())
            {
                manager.SetNextState((int)Enemy02.State.WAIT);
                return true;
            }
            return false;
        }

        public override void Update()
        {
            //移動値を登録
            base.enemy.SetPos(new Vector2(this.move_x.linear.None(this.move_x.Time(this.moveData.moveTime), this.move_x.GetStartValue(), this.move_x.GetEndValue(), this.moveData.moveTime),
                this.move_y.linear.None(this.move_y.Time(this.moveData.moveTime), this.move_y.GetStartValue(), this.move_y.GetEndValue(), this.moveData.moveTime)));
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
            this.enemy.parameter.animation.Play("Normal");
        }

        public override void Exit(ref StateManager manager)
        {
        }

        public override bool Transition(ref StateManager manager)
        {
            if(base.enemy.moves.Length == 0)
            {
                return false;
            }
            if(base.enemy.moves[base.enemy.GetNowNum()].waitTime <= base.GetTime())
            {
               manager.SetNextState((int)Enemy02.State.MOVE);
                return true;
            }
            return false;
        }

        public override void Update()
        {
        }
    }
    public class DeathState : BaseState
    {
        public DeathState(Enemy02 enemy) : base(enemy)
        {
        }

        public override void Enter(ref StateManager manager)
        {
            this.enemy.parameter.animation.Play("Death");
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
            if (base.GetTime() >= 60)
            {
                base.enemy.KillMyself();
            }
        }
    }
}