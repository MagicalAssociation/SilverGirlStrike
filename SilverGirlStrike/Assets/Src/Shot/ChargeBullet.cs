using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Bullet
{
    //MagicBullet(直線に飛ばす)--------------------------------------------------------------------------------------------------
    public class ChargeBullet : BaseBullet
    {
        public int chargeTime;
        public float jumpPower;
        public enum State
        {
            NORMAL,
        }
        //自分の登録番号
        int myselfID;
        CharacterMover mover;

        private void Start()
        {
            mover = GetComponent<CharacterMover>();
            this.myselfID = base.FindManager().AddCharacter(this);
            base.AddState((int)State.NORMAL, new Charge.NormalState(this));
            base.ChangeState((int)State.NORMAL);
        }

        public override void UpdateCharacter()
        {
            GetData().stateManager.Update();
        }

        public override bool Damage(AttackData attackData)
        {
            return false;
        }

        public override void ApplyDamage()
        {
        }

        public override void MoveCharacter()
        {
            this.mover.UpdateVelocity(move.x * this.moveSpeed, move.y * this.moveSpeed, 0.0f, true);

            //弾の向き
            this.transform.rotation = Quaternion.FromToRotation(new Vector2(1.0f, 0.0f), move);
        }
        /**
         * brief    自分を消す命令をManagerに行う処理
         */
        public void Delete()
        {
            base.KillMyself();
        }
        public static void Create(CharacterObject characterObject, BulletData bulletData, Vector3 position,int chargeTime,float jumpPower)
        {
            Vector3 pos = new Vector3();
            if (bulletData.offsetObject == null)
            {
                pos = bulletData.offset + position;
            }
            else
            {
                pos = bulletData.offsetObject.transform.position;
            }
            ChargeBullet bullet = Object.Instantiate(bulletData.attackObject, pos, Quaternion.identity) as Bullet.ChargeBullet;
            bullet.SetAttackData(new AttackData(characterObject));
            bullet.GetAttackData().power = bulletData.power;
            bullet.lifeCnt = bulletData.life;
            bullet.moveSpeed = bulletData.speed * 10.0f;
            if (bulletData.target != null)
            {
                bullet.SetShotTarget(bulletData.target);
            }
            else
            {
                bullet.SetShotAngle(bulletData.angle);
            }
            bullet.chargeTime = chargeTime;
            bullet.jumpPower = jumpPower;
        }
    }
    namespace Charge
    {
        /**
         * brief    元State
         */
        public abstract class BaseState : StateParameter
        {
            public ChargeBullet bullet;
            public BaseState(ChargeBullet bullet)
            {
                this.bullet = bullet;
            }
        }
        /**
         * brief    通常State
         */
        public class NormalState : BaseState
        {
            public NormalState(ChargeBullet bullet)
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
                base.bullet.attacker.StartAttack();
                //自分がプレイヤーと当たっていた時、プレイヤーにダメージを与え自分は消滅する
                base.bullet.attacker.AttackJudge((int)M_System.LayerName.PLAYER);
                if (base.bullet.attacker.IsHit())
                {
                    base.bullet.Delete();
                    return;
                }
                //地面に当たったら消す
                var hit = base.bullet.attacker.AttackJudge((int)M_System.LayerName.GROUND);
                if (hit)
                {
                    base.bullet.Delete();
                    return;
                }
                //時間による削除処理
                if (base.GetTime() > base.bullet.lifeCnt)
                {
                    base.bullet.Delete();
                    return;
                }
            }
        }
    }
}