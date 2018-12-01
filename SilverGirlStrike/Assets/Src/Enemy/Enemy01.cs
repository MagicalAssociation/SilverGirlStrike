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
 * GameObject targetになにもいれないと固定方向に攻撃を飛ばします。
 * targetにオブジェクトを登録するとその方向に攻撃を飛ばします。
*/
/**
 * Inspectorの設定値の説明
 * Gravity このCharacterの重さ
 * Parameter.AttackObject 攻撃する際に出現させるGameObject
 *      これ今MagicBulletしか入らない設計しているので今後の実装によっては変化させます
 * Parameter.Radius このCharacterが攻撃を始める範囲
 * Parameter.AttackInterval 攻撃の感覚を指定します。
 * Parameter.Animation アニメーションデータ
 * Parameter.Mover 自動で入るので放置で構いません
 * Parameter.Foot このGameObjectの子にFoot.csこみのGameObjectをいれておいてください、入れるのは自動です
 * Parameter.Direction 自動で入るので放置で
 * BulletData.Angle 弾を飛ばす方向
 * BulletData.power ダメージ値
 * BulletData.Speed 弾の飛ぶ速度
 * BulletData.Life 生存するカウント数
 * BulletData.Target 指定オブジェクトの方向に攻撃を飛ばす、ここに何も入っていない時に指定角度で弾を飛ばします
 * EnableGravity このCharacterに重力を適用するかです。外れている場合落下もせず、FallStateにもなりません。
 */
namespace Enemy01
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
            //! 攻撃開始範囲の半径
            public float radius;
            //! 攻撃間隔
            public int attackInterval;
            //! Animation
            public Animation animation;
            //! 移動用class
            public CharacterMover mover;
            //! 地面判定
            public Foot foot;
            //! 向き
            public Direction direction;
        }
        [System.Serializable]
        public class BulletData
        {
            [Range(0.0f,360.0f)]
            public float angle;
            public int power;
            public float speed;
            public int life;
            public GameObject target;
        }
        //! 重力
        public float gravity;
        //! Parameter
        public Enemy01Parameter parameter;
        //! 生成するBulletのデータ
        public BulletData bulletData;
        //! 当たり判定
        BoxCollider2D collider;
        //! 攻撃範囲
        CircleCollider2D circleCollider;
        //! 重力の有効化設定
        public bool enableGravity;
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
        }
        // Update is called once per frame
        private void Start()
        {
            parameter.mover = GetComponent<CharacterMover>();
            parameter.foot = this.transform.GetComponentInChildren<Foot>();
            this.collider = GetComponent<BoxCollider2D>();
            this.circleCollider = GetComponent<CircleCollider2D>();
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
            parameter.mover.UpdateVelocity(0, 0, this.enableGravity ? this.gravity : 0.0f, parameter.foot.CheckHit());
        }
        /**
         * brief    Enemy01専用のパラメータデータ取得
         */
         public Enemy01Parameter GetParameter()
        {
            return this.parameter;
        }
        /**
         * brief    攻撃するターゲットが近くにいるか返す
         * return Collider2D 円の判定
         */
         public Collider2D TargetDistanceCheck()
        {
            return Physics2D.OverlapCircle(
                   this.circleCollider.transform.position,
                   this.circleCollider.radius,
                   (int)M_System.LayerName.PLAYER
                   );
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
            if(!enemy.GetParameter().foot.CheckHit() && this.enemy.enableGravity)
            {
                manager.SetNextState((int)Enemy01.State.FALL);
                return true;
            }
            //30count経過したら攻撃モーションへ移行
            if(base.GetTime() > base.enemy.GetParameter().attackInterval && base.enemy.TargetDistanceCheck() != null)
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
                this.CreateBullet();
            }
        }
        /**
         * brief    攻撃生成
         */ 
        private void CreateBullet()
        {
            Bullet.MagicBullet bullet = Object.Instantiate(base.enemy.GetParameter().attackObject, base.enemy.transform.position, Quaternion.identity) as Bullet.MagicBullet;
            bullet.SetAttackData(new AttackData(base.enemy));
            bullet.GetAttackData().power = base.enemy.bulletData.power;
            bullet.lifeCnt = base.enemy.bulletData.life;
            bullet.moveSpeed = base.enemy.bulletData.speed;
            if(this.enemy.bulletData.target != null)
            {
                bullet.SetShotTarget(this.enemy.bulletData.target);
            }
            else
            {
                bullet.SetShotAngle(this.enemy.bulletData.angle);
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