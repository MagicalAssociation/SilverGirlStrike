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
 * Parameter.Animation アニメーションデータ
 * Parameter.Power ダメージ値
 * Moves.Target 移動先GameObject
 * Moves.WaitTime ↑へ移動する前に待機している時間
 * Moves.MoveTime ↑へ移動する時間を指定
 */
namespace Enemy03
{
    public class Enemy03 : CharacterObject
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
        }
        /**
         * brief    エネミー03用パラメータデータ
         */
        [System.Serializable]
        public class Enemy03Parameter
        {
            //! アニメーション用class
            public Animation animation;
            //! ダメージ量
            public int power;
        }
        /**
         * brief    停止用データ
         */
        [System.Serializable]
        public class StopData
        {
            public int waitTime;
            public int stopTime;
        }
        /**
         * brief    移動用変数をまとめたclass
         */
        [System.Serializable]
        public class Move
        {
            //! 移動速度
            public float speed;
            //! 移動半径
            public float radius;
            //! 回転軸の移動倍率
            public Vector2 magnification;
            //! 停止時間とそのタイミング
            public StopData[] stopDatas;
        }
        //! 固有パラメータデータ
        public Enemy03Parameter parameter;
        //! 移動用データの配列
        public Move move;
        //! 移動の中心位置
        private Vector2 originPos;
        //! 現在位置
        private Vector2 pos;
        //! 攻撃データ
        private AttackData attackData;
        //! 自身のBoxの当たり判定
        private BoxCollider2D collider;
        public Enemy03()
            //! life
            : base(10)
        {
            //各ステートを登録&適用
            base.AddState((int)State.MOVE, new MoveState(this));
            base.AddState((int)State.WAIT, new WaitState(this));
            base.ChangeState((int)State.MOVE);
            this.originPos = new Vector2();
            this.attackData = new AttackData(this);
        }
        private void Start()
        {
            //今の位置をいれておく
            this.originPos = this.transform.localPosition;
            this.collider = GetComponent<BoxCollider2D>();
            this.attackData.power = this.parameter.power;
        }

        public override void UpdateCharacter()
        {
            this.UpdateState();
            //プレイヤーと当たったらダメージ処理
            Collider2D hit = this.Hit();
            if (hit)
            {
                hit.GetComponent<CharacterObject>().Damage(this.attackData);
            }
        }

        public override void Damage(AttackData attackData)
        {

        }

        public override void ApplyDamage()
        {
            this.GetData().hitPoint.DamageUpdate();
            if (this.GetData().hitPoint.GetHP() <= 0)
            {

            }
        }

        public override void MoveCharacter()
        {
            this.transform.localPosition = new Vector3(pos.x, pos.y, this.transform.position.z);
        }
        /**
         * brief    固有データを取得する
         * return Enemy03Parameter ThisParameter
         */
        public Enemy03Parameter GetParameter()
        {
            return this.parameter;
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
         * brief    原点を取得
         * return Vector2 原点
         */
         public Vector2 GetOriginPos()
        {
            return this.originPos;
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
        public Enemy03 enemy;
        public BaseState(Enemy03 enemy)
        {
            this.enemy = enemy;
        }
    }
    /**
     * brief    移動State
     */
    public class MoveState : BaseState
    {
        //! 移動量とかターゲット位置とかの取得用
        Enemy03.Move moveData;
        public MoveState(Enemy03 enemy)
            : base(enemy)
        {
        }

        public override void Enter(ref StateManager manager)
        {
            //次自分が向かうMovesのデータを取得
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
            this.enemy.SetPos(new Vector2(this.enemy.GetOriginPos().x + (Mathf.Sin(base.GetTime() * this.enemy.move.speed) * this.enemy.move.radius * this.enemy.move.magnification.x),
                this.enemy.GetOriginPos().y + (Mathf.Cos(base.GetTime() * this.enemy.move.speed) * this.enemy.move.radius) * this.enemy.move.magnification.y));
        }
    }
    /**
     * brief    待機State
     */
    public class WaitState : BaseState
    {
        public WaitState(Enemy03 enemy)
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
            return false;
        }

        public override void Update()
        {
            base.TimeUp(1);

        }
    }
}