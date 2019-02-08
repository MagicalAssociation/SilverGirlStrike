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
        public float gravity;
        public bool isCharge;
        public float distance;
        Easing easing;
        public enum State
        {
            NORMAL,
            CHARGE,
        }
        //自分の登録番号
        int myselfID;
        CharacterMover mover;
        Animator animator;
        private void Start()
        {
            mover = GetComponent<CharacterMover>();
            animator = GetComponent<Animator>();
            this.myselfID = base.FindManager().AddCharacter(this);
            base.AddState((int)State.NORMAL, new Charge.NormalState(this));
            base.AddState((int)State.CHARGE, new Charge.ChargeState(this));
            base.ChangeState((int)State.CHARGE);
            easing = new Easing();
        }

        public override void UpdateCharacter()
        {
            GetData().stateManager.Update();
        }

        public override bool Damage(AttackData attackData)
        {
            return this.GetData().hitPoint.Damage(attackData.power, attackData.chain);
        }

        public override void ApplyDamage()
        {
        }

        public override void MoveCharacter()
        {
            if (isCharge == false)
            {
                //float gra = (distance / 10.0f) + (jumpPower / 4.0f);
                //this.mover.UpdateVelocity(move.x * this.moveSpeed, move.y * this.moveSpeed, gra / gravity, true);
                this.mover.UpdateVelocity(move.x * this.moveSpeed, move.y * this.moveSpeed, isCharge ?  0.0f : gravity, true);
                //弾の向き
                this.transform.rotation = Quaternion.FromToRotation(new Vector2(1.0f, 0.0f), move);
            }
        }
        public CharacterMover GetCharacterMover()
        {
            return this.mover;
        }
        public Animator GetAnimator()
        {
            return this.animator;
        }
        /**
         * brief    自分を消す命令をManagerに行う処理
         */
        public void Delete()
        {
            base.KillMyself();
        }
        public static void Create(CharacterObject characterObject, BulletData bulletData, Vector3 position,int chargeTime,float jumpPower,float gravity)
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
            //if (bulletData.target != null)
            //{
            //    bullet.SetShotTarget(bulletData.target);
            //    //Test処理
            //    bullet.distance = bulletData.target.transform.position.x - pos.x;
            //    if(bullet.distance < 0.0f)
            //    {
            //        bullet.distance *= -1.0f;
            //    }
            //}
            //else
            {
                bullet.SetShotAngle(bulletData.angle);
            }
            bullet.chargeTime = chargeTime;
            bullet.jumpPower = jumpPower;
            bullet.gravity = gravity;
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
                bullet.isCharge = false;
                bullet.GetCharacterMover().Jump(bullet.jumpPower);
                bullet.GetAnimator().Play("normal");

                Sound.PlaySE("press1");

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
        /**
         * brief   弾のチャーーージ
         */
        public class ChargeState : BaseState
        {
            public ChargeState(ChargeBullet bullet) : base(bullet)
            {
            }

            public override void Enter(ref StateManager manager)
            {
                bullet.isCharge = true;
                bullet.GetAnimator().Play("charge");
            }

            public override void Exit(ref StateManager manager)
            {
            }

            public override bool Transition(ref StateManager manager)
            {
                if(GetTime() > bullet.chargeTime)
                {
                    manager.SetNextState((int)ChargeBullet.State.NORMAL);
                    return true;
                }
                return false;
            }

            public override void Update()
            {
                //チャージ時間の半分で弾を大きくする
                if(GetTime() == bullet.chargeTime / 2)
                {
                    bullet.GetAnimator().Play("normal");
                }
                base.bullet.attacker.StartAttack();
                //自分がプレイヤーと当たっていた時、プレイヤーにダメージを与え自分は消滅する
                base.bullet.attacker.AttackJudge((int)M_System.LayerName.PLAYER);
                if (base.bullet.attacker.IsHit())
                {
                    base.bullet.Delete();
                    return;
                }
                //地面に当たったら消す
                base.bullet.attacker.AttackJudge((int)M_System.LayerName.GROUND);
                if (base.bullet.attacker.IsHit())
                {
                    base.bullet.Delete();
                    return;
                }
            }
        }
    }
}