using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * file     Enemy04.cs
 * brief    敵Class
 * author   Shou Kaneko
 * date     2018/12/07
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
namespace Enemy04
{
    public class Enemy04 : CharacterObject
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
            //! 降下
            DESCENT,
            //! 上昇
            RISING,
            //! 周囲攻撃
            CIRCUMFERENCE,
            //! 軌道待ち
            ORBIT,
            //! 死亡
            DEATH,
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
        public class Enemy04Parameter
        {
            //! 最大HP
            public int maxHP;
            //! アニメーション用class
            public Animator animation;
            //! ダメージ量
            public int power;
            //! 向き情報
            public Direction direction;
            //! 攻撃間隔
            public int attackInterval;
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
            //! 足元の位置
            public Transform footPosition;
        }
        /**
         * brief    周囲攻撃の情報をまとめたclass
         */
         [System.Serializable]
         public class AttackParameter
        {
            public NarrowAttacker[] narrowAttackers;
        }
        //! 固有パラメータデータ
        public Enemy04Parameter parameter;
        //! 移動用データの配列
        public Move move;
        //! 移動の中心位置
        private Vector2 originPos;
        //! 足元位置
        private Vector2 footPos;
        //! 前回の位置
        private Vector2 prePos;
        //! 現在位置
        private Vector2 pos;
        //! 攻撃データ
        private AttackData attackData;
        //! 自身のBoxの当たり判定
        private BoxCollider2D collider;
        //! 魔法弾
        public Bullet.BulletData[] bulletData;
        //! 周囲攻撃データ
        public AttackParameter attackParameter;
        //! プレイヤー検知用Circle
        private CircleCollider2D circleCollider;
        public Enemy04()
            //! life
            : base(10)
        {
            this.originPos = new Vector2();
            this.attackData = new AttackData(this);
        }
        private void Start()
        {
            //今の位置をいれておく
            this.originPos = this.transform.localPosition;
            this.prePos = this.originPos;
            this.pos = this.originPos;
            this.footPos = this.move.footPosition.position;
            this.parameter.direction = Direction.LEFT;
            this.collider = GetComponent<BoxCollider2D>();
            this.circleCollider = GetComponent<CircleCollider2D>();
            this.attackData.power = this.parameter.power;
            this.parameter.animation = GetComponent<Animator>();
            this.GetData().hitPoint.SetMaxHP(this.parameter.maxHP);
            //各ステートを登録&適用
            base.AddState((int)State.MOVE, new MoveState(this));
            base.AddState((int)State.WAIT, new WaitState(this));
            base.AddState((int)State.DESCENT, new DescentState(this));
            base.AddState((int)State.RISING, new RisingState(this));
            base.AddState((int)State.CIRCUMFERENCE, new CircumferenceState(this));
            base.AddState((int)State.ORBIT, new OrbitState(this));
            base.AddState((int)State.DEATH, new DeathState(this));
            base.ChangeState((int)State.ORBIT);
        }

        public override void UpdateCharacter()
        {
            this.UpdateState();
            //プレイヤーと当たったらダメージ処理
            Collider2D hit = this.Hit();
            if (hit != null)
            {
                if (hit.GetComponent<CharacterObject>() != null)
                {
                    hit.GetComponent<CharacterObject>().Damage(this.attackData);
                }
            }
            this.DebugDrawPointer();
        }

        public override void Damage(AttackData attackData)
        {
            this.GetData().hitPoint.Damage(attackData.power, attackData.chain);
        }

        public override void ApplyDamage()
        {
            this.GetData().hitPoint.DamageUpdate();
            if (this.GetData().hitPoint.GetHP() <= 0 && this.GetData().stateManager.GetNowStateNum() != (int)State.DEATH)
            {
                base.ChangeState((int)State.DEATH);
            }
        }

        public override void MoveCharacter()
        {
            this.prePos = this.transform.localPosition;
            this.transform.localPosition = new Vector3(pos.x, pos.y, this.transform.position.z);
            if (this.transform.localPosition.x > this.prePos.x)
            {
                this.parameter.direction = Direction.RIGHT;
            }
            else if (this.transform.localPosition.x < this.prePos.x)
            {
                this.parameter.direction = Direction.LEFT;
            }
        }
        /**
         * brief    固有データを取得する
         * return Enemy03Parameter ThisParameter
         */
        public Enemy04Parameter GetParameter()
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
         * brief    プレイヤー検知
         */
         public Collider2D CheckTargetDetection()
        {
            return Physics2D.OverlapCircle(
                this.circleCollider.transform.position,
                this.circleCollider.radius,
                (int)M_System.LayerName.PLAYER);
        }
        /**
         * brief    Sceneに自分の通った道に点を置くDebug処理
         */
        public void DebugDrawPointer()
        {
            Debug.DrawRay(this.transform.localPosition, new Vector3(0.1f, 0), Color.red, 10.0f);
        }

        public Vector2 GetFootPosition()
        {
            return this.footPos;
        }
        public Vector2 GetOriginPosition()
        {
            return this.originPos;
        }
    }
    /**
     * brief    元となるState
     */
    public abstract class BaseState : StateParameter
    {
        public Enemy04 enemy;
        public BaseState(Enemy04 enemy)
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
        Enemy04.Move moveData;
        public MoveState(Enemy04 enemy)
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
            if (base.GetTime() != 0)
            {
                if ((int)Mathf.Sin(this.ToRadius(base.GetTime()) * this.enemy.move.speed) == 0 && (int)Mathf.Cos(this.ToRadius(base.GetTime()) * this.enemy.move.speed) == 1)
                {
                    manager.SetNextState((int)Enemy04.State.WAIT);
                    return true;
                }
            }
            return false;
        }

        public override void Update()
        {
            this.enemy.SetPos(new Vector2(this.enemy.GetOriginPos().x + (Mathf.Sin(this.ToRadius(base.GetTime() * this.enemy.move.speed)) * this.enemy.move.radius * this.enemy.move.scale.x),
                this.enemy.GetOriginPos().y + (Mathf.Cos(this.ToRadius(base.GetTime() * this.enemy.move.speed * 2 + 90)) * this.enemy.move.radius) * this.enemy.move.scale.y));
            if(base.GetTime() % base.enemy.parameter.attackInterval == 0)
            {
                for (int i = 0; i < this.enemy.bulletData.Length; ++i)
                {
                    Bullet.MagicBullet.Create(this.enemy, this.enemy.bulletData[i], this.enemy.transform.position);
                }
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
        public WaitState(Enemy04 enemy)
            : base(enemy)
        {
        }

        public override void Enter(ref StateManager manager)
        {
            //this.enemy.parameter.animation.Play("");
        }

        public override void Exit(ref StateManager manager)
        {
        }

        public override bool Transition(ref StateManager manager)
        {
            if(base.GetTime() >= 30)
            {
                switch (manager.GetPreStateNum())
                {
                    case (int)Enemy04.State.RISING:
                        manager.SetNextState((int)Enemy04.State.MOVE);
                        break;
                    case (int)Enemy04.State.MOVE:
                        manager.SetNextState((int)Enemy04.State.DESCENT);
                        break;
                    default:
                        break;
                }
                return true;
            }
            return false;
        }

        public override void Update()
        {
        }
    }
    /**
     * brief    降下State
     */
    public class DescentState : BaseState
    {
        Easing move_y;
        public DescentState(Enemy04 enemy) 
            : base(enemy)
        {
            move_y = new Easing();
        }

        public override void Enter(ref StateManager manager)
        {
            this.enemy.parameter.animation.Play("Idle");
            move_y.Set(this.enemy.GetOriginPos().y, this.enemy.GetFootPosition().y - this.enemy.GetOriginPos().y);
        }

        public override void Exit(ref StateManager manager)
        {
            move_y.ResetTime();
        }

        public override bool Transition(ref StateManager manager)
        {
            if (!this.move_y.IsPlay())
            {
                manager.SetNextState((int)Enemy04.State.CIRCUMFERENCE);
                return true;
            }
            return false;
        }

        public override void Update()
        {
            base.enemy.SetPos(new Vector2(this.enemy.GetOriginPos().x,
                this.move_y.linear.None(this.move_y.Time(3), this.move_y.GetStartValue(), this.move_y.GetEndValue(), 3)));
        }
    }
    /**
     * brief   上昇State
     */
    public class RisingState : BaseState
    {
        Easing move_y;
        public RisingState(Enemy04 enemy) : base(enemy)
        {
            this.move_y = new Easing();
        }

        public override void Enter(ref StateManager manager)
        {
            this.enemy.parameter.animation.Play("Idle");
            move_y.Set(this.enemy.GetFootPosition().y, this.enemy.GetOriginPos().y - this.enemy.GetFootPosition().y);
        }

        public override void Exit(ref StateManager manager)
        {
            move_y.ResetTime();
        }

        public override bool Transition(ref StateManager manager)
        {
            if(!this.move_y.IsPlay())
            {
                manager.SetNextState((int)Enemy04.State.WAIT);
                return true;
            }
            return false;
        }

        public override void Update()
        {
            base.enemy.SetPos(new Vector2(this.enemy.GetOriginPos().x,
                this.move_y.linear.None(this.move_y.Time(3), this.move_y.GetStartValue(), this.move_y.GetEndValue(), 3)));
        }
    }
    /**
     * brief    周囲攻撃State
     */ 
    public class CircumferenceState : BaseState
    {
        public CircumferenceState(Enemy04 enemy) : base(enemy)
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
            if(base.GetTime() >= 30)
            {
                manager.SetNextState((int)Enemy04.State.RISING);
                return true;
            }
            return false;
        }

        public override void Update()
        {
            if(base.GetTime() == 4)
            {
                this.enemy.attackParameter.narrowAttackers[0].StartAttack();
            }
        }
    }
    /**
     * brief    軌道確認
     */
    public class OrbitState : BaseState
    {
        public OrbitState(Enemy04 enemy) : base(enemy)
        {
        }

        public override void Enter(ref StateManager manager)
        {
            this.enemy.parameter.animation.Play("Idle");
            this.enemy.SetPos(this.enemy.GetFootPosition());
        }

        public override void Exit(ref StateManager manager)
        {
        }

        public override bool Transition(ref StateManager manager)
        {
            if (base.enemy.CheckTargetDetection() != null)
            {
                manager.SetNextState((int)Enemy04.State.RISING);
                return true;
            }
            return false;
        }

        public override void Update()
        {
        }
    }
    /**
     * brief    死亡State
     */
    public class DeathState : BaseState
    {
        public DeathState(Enemy04 enemy) : base(enemy)
        {
        }

        public override void Enter(ref StateManager manager)
        {
            this.enemy.parameter.animation.Play("Idle");
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
                this.enemy.gameObject.SetActive(false);
            }
        }
    }
}