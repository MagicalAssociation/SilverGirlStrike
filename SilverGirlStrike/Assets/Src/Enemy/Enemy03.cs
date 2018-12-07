using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * file     Enemy03.cs
 * brief    敵Class
 * author   Shou Kaneko
 * date     2018/12/01
 * 状態
 *      待ち、移動
*/
/**
 * Inspectorの設定値の説明
 * Parameter.MaxHP 最大HP
 * Parameter.Animation アニメーションデータ
 * Parameter.Power ダメージ値
 * Move.Speed 移動速度 1で1週に360フレームかかる感じ
 * Move.Radius 半径
 * Move.Scale 半径倍率
 *      基本値1でこの値を増やしたり減らしたりして楕円等の移動を作る
 * StopDatas 停止タイミングと停止時間の設定
 *      停止タイミングは0に近い順にいれてください。1週にかかる時間以上を設定している場合無視されます。
 * StopDatas.WaitTime 停止時間、カウント数です。
 * StopDatas.StopTime 停止を行うカウント数。1なら移動処理1カウント目に停止します。
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
         * enum Direction
         */
         public enum Direction
        {
            LEFT = 1,
            RIGHT = -1
        }
        /**
         * brief    エネミー03用パラメータデータ
         */
        [System.Serializable]
        public class Enemy03Parameter
        {
            //! 最大HP
            public int maxHP;
            //! アニメーション用class
            public Animator animation;
            //! ダメージ量
            public int power;
            //! 向き情報
            public Direction direction;
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
            public Vector2 scale;
            //! 停止時間とそのタイミング
            public StopData[] stopDatas;
        }
        //! 固有パラメータデータ
        public Enemy03Parameter parameter;
        //! 移動用データの配列
        public Move move;
        //! 移動の中心位置
        private Vector2 originPos;
        //! 前回の位置
        private Vector2 prePos;
        //! 現在位置
        private Vector2 pos;
        //! 攻撃データ
        private AttackData attackData;
        //! 自身のBoxの当たり判定
        private BoxCollider2D collider;
        //! 停止データの配列位置番号
        private int nowNum;
        public Enemy03()
            //! life
            : base(10)
        {
            this.originPos = new Vector2();
            this.attackData = new AttackData(this);
            this.nowNum = 0;
        }
        private void Start()
        {
            //今の位置をいれておく
            this.originPos = this.transform.localPosition;
            this.prePos = this.originPos;
            this.collider = GetComponent<BoxCollider2D>();
            this.attackData.power = this.parameter.power;
            this.parameter.animation = GetComponent<Animator>();
            this.GetData().hitPoint.SetMaxHP(this.parameter.maxHP);
            //各ステートを登録&適用
            base.AddState((int)State.MOVE, new MoveState(this));
            base.AddState((int)State.WAIT, new WaitState(this));
            base.ChangeState((int)State.MOVE);
        }

        public override void UpdateCharacter()
        {
            this.UpdateState();
            //プレイヤーと当たったらダメージ処理
            Collider2D hit = this.Hit();
            if (hit != null)
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
            this.prePos = this.transform.localPosition;
            this.transform.localPosition = new Vector3(pos.x, pos.y, this.transform.position.z);
            if(this.transform.localPosition.x > this.prePos.x)
            {
                this.parameter.direction = Direction.RIGHT;
            }
            else if(this.transform.localPosition.x < this.prePos.x)
            {
                this.parameter.direction = Direction.LEFT;
            }
            this.transform.localScale = new Vector3((int)this.parameter.direction, 1, 1);
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
        /**
         * brief    現在の停止位置の配列番号のデータを返す
         * return StopData 位置と時間の入ったclass
         */
         public StopData GetStopData()
        {
            return this.move.stopDatas[this.nowNum];
        }
        /**
         * brief    停止位置を次に移行する
         * 次がない場合は最初に戻る
         */
         public void NextStopData()
        {
            this.nowNum++;
            if(this.move.stopDatas.Length <= this.nowNum)
            {
                this.nowNum = 0;
            }
        }
        /**
         * brief    停止配列の位置を最初に戻す
         */
         public void StartStopData()
        {
            this.nowNum = 0;
        }
        /**
         * brief    Sceneに自分の通った道に点を置くDebug処理
         */
         public void DebugDrawPointer()
        {
            Debug.DrawRay(this.transform.localPosition, new Vector3(0.1f, 0), Color.red, 10.0f);
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
            this.enemy.parameter.animation.Play("Move");
        }

        public override void Exit(ref StateManager manager)
        {
        }

        public override bool Transition(ref StateManager manager)
        {
            if(this.enemy.GetStopData().stopTime == base.GetTime())
            {
                manager.SetNextState((int)Enemy03.State.WAIT);
                return true;
            }
            return false;
        }

        public override void Update()
        {
            this.enemy.SetPos(new Vector2(this.enemy.GetOriginPos().x + (Mathf.Sin(this.ToRadius(base.GetTime() * this.enemy.move.speed)) * this.enemy.move.radius * this.enemy.move.scale.x),
                this.enemy.GetOriginPos().y + (Mathf.Cos(this.ToRadius(base.GetTime() * this.enemy.move.speed)) * this.enemy.move.radius) * this.enemy.move.scale.y));
            //1週判定
            if ((int)Mathf.Sin(this.ToRadius(base.GetTime()) * this.enemy.move.speed) == 0 && (int)Mathf.Cos(this.ToRadius(base.GetTime()) * this.enemy.move.speed) == 1)
            {
                base.ResetTime();
                this.enemy.StartStopData();
            }
        }
        private float ToRadius(float angle)
        {
            return (angle * Mathf.PI) / 180.0f;
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
            this.enemy.parameter.animation.Play("Normal");
        }

        public override void Exit(ref StateManager manager)
        {
            base.ResetTime();
            this.enemy.NextStopData();
        }

        public override bool Transition(ref StateManager manager)
        {
            if (this.enemy.GetStopData().waitTime == this.GetTime())
            {
                manager.SetNextState((int)Enemy03.State.MOVE);
                return true;
            }
            return false;
        }

        public override void Update()
        {
        }
    }
}