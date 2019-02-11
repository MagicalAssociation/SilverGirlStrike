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
 * 中ボス 八の字で動きながらBulletを打つ、落下してきて範囲攻撃を行う
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
         * brief    エネミー04用パラメータデータ
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
            public Enemy.AutoScaleChange autoScaleChange;

            //無敵時間
            public int damageInvincible;
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
            //! 停止カウント数の最小値
            public int stopCountMin;
            //! 停止カウント数の最大値
            public int stopCountMax;
        }
        /**
         * brief    周囲攻撃の情報をまとめたclass
         */
         [System.Serializable]
         public class AttackParameter
        {
            public int beginTime;
            public int attackTime;
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
        //! 前回の位置(向き変えるために作った)
        private Vector2 prePos;
        //! 現在位置
        private Vector2 pos;
        //! 攻撃データ
        private AttackData attackData;
        //! 自身のBoxの当たり判定
        private BoxCollider2D collider;
        //! 魔法弾
        public Bullet.BulletParameter bulletParameter;
        //! 周囲攻撃データ
        public AttackParameter attackParameter;
        //! プレイヤー検知用Circle
        private CircleCollider2D circleCollider;
        //! 停止カウント
        private int stopCount;
        //! 移動速度の最大値
        private float maxSpeed;
        private Easing easing;
        private float angle;
        public Enemy04()
            //! life
            : base(10)
        {
            this.originPos = new Vector2();
            this.attackData = new AttackData(this);
            this.easing = new Easing();
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
            this.GetData().hitPoint.Recover(this.parameter.maxHP);
            this.GetData().hitPoint.SetInvincible(this.parameter.damageInvincible);

            this.stopCount = Random.Range(move.stopCountMin, move.stopCountMax);
            this.maxSpeed = move.speed;
            this.parameter.autoScaleChange.Init(this.gameObject);
            //各ステートを登録&適用
            base.AddState((int)State.MOVE, new MoveState(this));
            base.AddState((int)State.WAIT, new WaitState(this));
            base.AddState((int)State.DESCENT, new DescentState(this));
            base.AddState((int)State.RISING, new RisingState(this));
            base.AddState((int)State.CIRCUMFERENCE, new CircumferenceState(this));
            base.AddState((int)State.ORBIT, new OrbitState(this));
            base.AddState((int)State.DEATH, new DeathState(this));
            base.ChangeState((int)State.ORBIT);
            this.easing.ResetTime();
            this.easing.Set(move.speed, 0, 5, new Easing.Linear());
        }

        public override void UpdateCharacter()
        {
            if (this.GetData().stateManager.GetNowStateNum() != (int)State.DEATH)
            {
                this.attackParameter.narrowAttackers[0].StartAttack();
            }
            this.UpdateState();
            this.DebugDrawPointer();
        }

        public override bool Damage(AttackData attackData)
        {
            return this.GetData().hitPoint.Damage(attackData.power, attackData.chain);
        }

        public override void ApplyDamage()
        {
            if (this.GetData().hitPoint.GetHP() <= 0 && this.GetData().stateManager.GetNowStateNum() != (int)State.DEATH)
            {
                base.ChangeState((int)State.DEATH);
            }
            else
            {
                this.GetData().hitPoint.DamageUpdate();
            }
        }

        public override void MoveCharacter()
        {
            this.transform.localPosition = new Vector3(pos.x, pos.y, this.transform.position.z);
            this.parameter.autoScaleChange.DirectionUpdate();
        }
        /**
         * brief    固有データを取得する
         * return Enemy04Parameter ThisParameter
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
        public float GetMaxSpeed()
        {
            return this.maxSpeed;
        }
        public void SetStopCount(int count)
        {
            this.stopCount = count;
        }
        public int GetStopCount()
        {
            return this.stopCount;
        }
        public void SetPrePos(Vector2 pos)
        {
            this.prePos = pos;
        }
        public Vector2 GetPrePos()
        {
            return this.prePos;
        }
        public void SetAngle(float a)
        {
            angle = a;
        }
        public float GetAngle()
        {
            return this.angle;
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
        private int count;
        Easing easing_Speed;
        bool attackFlag;
        int attackcount;
        public MoveState(Enemy04 enemy)
            : base(enemy)
        {
            count = 0;
            this.easing_Speed = new Easing();
        }

        public override void Enter(ref StateManager manager)
        {
            this.enemy.parameter.animation.Play("Move");
            this.easing_Speed.ResetTime();
            this.easing_Speed.Set(0, this.enemy.GetMaxSpeed(), 5, new Easing.Linear());
            this.attackFlag = false;
            this.attackcount = 0;
        }

        public override void Exit(ref StateManager manager)
        {
        }

        public override bool Transition(ref StateManager manager)
        {
            if(base.enemy.GetStopCount() < 0 && !this.easing_Speed.IsPlay() && attackFlag == false)
            {
                base.enemy.SetStopCount(Random.Range(enemy.move.stopCountMin, enemy.move.stopCountMax));
                manager.SetNextState((int)Enemy04.State.WAIT);
                return true;
            }
            return false;
        }

        public override void Update()
        {
            ++this.count;
        
            //速度をEasingで変化
            this.enemy.move.speed = this.easing_Speed.In();
            //速度で中心からの角度を指定
            this.enemy.SetAngle(this.enemy.GetAngle() + this.enemy.move.speed);
            //移動処理
            this.enemy.SetPos(new Vector2(this.enemy.GetOriginPos().x + (Mathf.Sin(this.ToRadius(enemy.GetAngle())) * this.enemy.move.radius * this.enemy.move.scale.x),
                this.enemy.GetOriginPos().y + (Mathf.Cos(this.ToRadius(enemy.GetAngle() * 2 + 90)) * this.enemy.move.radius) * this.enemy.move.scale.y));
            //攻撃処理
            if ((this.GetTime() + attackcount) % base.enemy.parameter.attackInterval == 0 && attackFlag != true)
            {
                attackFlag = true;
                this.attackcount = this.GetTime();
            }
            else
            {
                if (attackFlag == true)
                {
                    //時間が来たら攻撃
                    if((GetTime() - attackcount) % this.enemy.bulletParameter.interval == 0)
                    {
                        Sound.PlaySE("shot1");
                        this.CreateBullet();
                    }
                    if((GetTime() - attackcount) % (this.enemy.bulletParameter.interval* this.enemy.bulletParameter.bulletNum) == 0)
                    {
                        this.attackFlag = false;
                    }
                }
            }   
            //カウントのあれこれ
            if (attackFlag != true)
            {
                base.enemy.SetStopCount(base.enemy.GetStopCount() - 1);
            }
            //終了カウントで減速を開始
            if (base.enemy.GetStopCount() == 0)
            {
                this.easing_Speed.ResetTime();
                this.easing_Speed.Set(this.enemy.move.speed, 0, 5);
            }
            Debug.Log(base.enemy.GetStopCount());
            Debug.Log(attackFlag);
        }
        private float ToRadius(float angle)
        {
            return (angle * Mathf.PI) / 180.0f;
        }
        private void CreateBullet()
        {
            for (int i = 0; i < this.enemy.bulletParameter.bulletData.Length; ++i)
            {
                Bullet.MagicBullet.Create(this.enemy, this.enemy.bulletParameter.bulletData[i], this.enemy.transform.position);
            }
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
            if(base.GetTime() >= 0)
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
            move_y.Set(this.enemy.transform.position.y, this.enemy.GetFootPosition().y);
            this.enemy.SetPrePos(this.enemy.transform.position);
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
            base.enemy.SetPos(new Vector2(this.enemy.transform.position.x,
                this.move_y.quint.InOut(this.move_y.Time(10), this.move_y.GetStartValue(), this.move_y.GetEndValue() - this.move_y.GetStartValue(), 10)));
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
            move_y.Set(this.enemy.GetFootPosition().y, this.enemy.GetPrePos().y);
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
            base.enemy.SetPos(new Vector2(this.enemy.transform.position.x,
                this.move_y.quint.InOut(this.move_y.Time(10), this.move_y.GetStartValue(), this.move_y.GetEndValue() - this.move_y.GetStartValue(), 10)));
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
            if(base.GetTime() >= this.enemy.attackParameter.beginTime + this.enemy.attackParameter.attackTime)
            {
                if (base.enemy.CheckTargetDetection() != null)
                {
                    manager.SetNextState((int)Enemy04.State.RISING);
                }
                else
                {
                    manager.SetNextState((int)Enemy04.State.ORBIT);
                }
                return true;
            }
            return false;
        }

        public override void Update()
        {
            if(base.GetTime() == this.enemy.attackParameter.beginTime)
            {
                this.enemy.attackParameter.narrowAttackers[1].StartAttack();
                Effect.Get().CreateEffect("magicAttack1", base.enemy.transform.position - Vector3.forward, Quaternion.identity, Vector3.one);
                //Sound.PlaySE("");
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
                enemy.bulletParameter.Search();
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
            Sound.PlaySE("crash1");
            this.enemy.parameter.animation.Play("Idle");
            this.enemy.GetData().hitPoint.SetDamageShutout(true);


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
            if (base.GetTime() % 5 == 0 && base.GetTime() <= 20)
            {
                Sound.PlaySE("bombSmall");
                Vector3 randomMove = new Vector3(Random.Range(0.0f, 2.0f) - 1.0f, Random.Range(0.0f, 2.0f) - 1.0f, 0.0f);
                Effect.Get().CreateEffect("manyBombs", base.enemy.transform.position - Vector3.forward + randomMove, Quaternion.identity, Vector3.one * 2);
            }

            if (base.GetTime() == 30)
            {
                Sound.PlaySE("slashFlash");
                Effect.Get().CreateEffect("defeat", this.enemy.transform.position - Vector3.forward, Quaternion.identity, Vector3.one);
            }
            if (base.GetTime() >= 60)
            {
                this.enemy.KillMyself();
            }
        }
    }
}