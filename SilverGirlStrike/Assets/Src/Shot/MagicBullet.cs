﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//変更履歴
//2018/11/30 板倉：削除処理をIDを用いたものに変更、そのために登録時のIDを取得している
//2018/12/04 板倉：基底クラスのCharacterData data とフィールド名が被っていることでエラーが出ていたので名前を変えておいた（動いてはいたが何かあるとメンドイので）

namespace Bullet
{

    [System.Serializable]
    public class BulletData
    {
        [Range(0.0f, 360.0f)]
        public float angle;
        public int power;
        public float speed;
        public int life;
        public GameObject target;
        public BaseBullet attackObject;
    }
    [System.Serializable]
    public class BulletParameter
    {
        //! 生成弾数
        public int bulletNum;
        //! 攻撃間隔
        public int interval;
        //! 攻撃データ
        public BulletData[] bulletData;
    }
    public abstract class BaseBullet : CharacterObject
    {
    }

    public class MagicBullet : BaseBullet
    {
        public enum State
        {
            NORMAL,
        }
        public enum Mode
        {
            //! 直線に飛ぶやつ
            LINE,
            //! ターゲットに向かって飛ぶやつ
            TARGET,
        }
        AttackData attackData;
        //! 移動速度
        public float moveSpeed;
        //! 寿命
        public int lifeCnt;
        //自分の登録番号
        int myselfID;
        BoxCollider2D collider;
        Vector2 move;
        CharacterMover mover;
        public Mode mode;
        
        private void Start()
        {
            collider = GetComponent<BoxCollider2D>();
            mover = GetComponent<CharacterMover>();
            this.myselfID = base.FindManager().AddCharacter(this);
            base.AddState((int)State.NORMAL, new NormalState(this));
            base.ChangeState((int)State.NORMAL);

        }
        /**
         * brief    AttackData登録
         * param[in] AttackData data 攻撃データ
         */
        public void SetAttackData(AttackData data)
        {
            this.attackData = data;
        }
        /**
         * brief    AttackDataの取得
         * return AttackData 攻撃データ
         */
        public AttackData GetAttackData()
        {
            return this.attackData;
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
        }
        /**
         * brief    自分を消す命令をManagerに行う処理
         */
        public void Delete()
        {
            base.KillMyself();
        }
        /**
         * brief    当たり判定
         */
        public Collider2D HitCheck()
        {
            if (this.collider)
            {
                Collider2D hit = Physics2D.OverlapBox(
                    this.collider.transform.position,
                    this.collider.size,
                    this.transform.eulerAngles.z,
                    (int)M_System.LayerName.PLAYER
                    );
                return hit;
            }
            return null;
        }
        public Collider2D HitCheck(int layer)
        {
            if (this.collider)
            {
                Collider2D hit = Physics2D.OverlapBox(
                    this.collider.transform.position,
                    this.collider.size,
                    this.transform.eulerAngles.z,
                    layer
                    );
                return hit;
            }
            return null;
        }
        public Collider2D HitCheck(int layer,Vector2 size)
        {
            if (this.collider)
            {
                Collider2D hit = Physics2D.OverlapBox(
                    this.collider.transform.position,
                    size,
                    this.transform.eulerAngles.z,
                    layer
                    );
                return hit;
            }
            return null;
        }
        /**
         * brief    攻撃を飛ばす方向を指定する
         * param[in] float angle 角度
         */
        public void SetShotAngle(float angle)
        {
            this.mode = Mode.LINE;
            move.x = Mathf.Cos(angle * (Mathf.PI / 180));
            move.y = Mathf.Sin(angle * (Mathf.PI / 180));
        }
        /**
         * brief    攻撃を飛ばす方向を指定オブジェクトにする
         * param[in] GameObject target ターゲット
         */
        public void SetShotTarget(GameObject target)
        {
            this.mode = Mode.TARGET;
            move = (target.transform.position - this.transform.position);
            float di = Mathf.Atan2(move.y, move.x);
            float rad = Mathf.Rad2Deg * di;
            rad *= (Mathf.PI / 180);
            move.x = Mathf.Cos(rad);
            move.y = Mathf.Sin(rad);
        }
        /**
         * brief    攻撃生成
         */
         public static void Create(CharacterObject characterObject,BulletData bulletData,Vector3 position)
        {
            MagicBullet bullet = Object.Instantiate(bulletData.attackObject, position, Quaternion.identity) as Bullet.MagicBullet;
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
        }
    }
    /**
     * brief    元State
     */ 
    public abstract class BaseState : StateParameter
    {
        public MagicBullet bullet;
        public BaseState(MagicBullet bullet)
        {
            this.bullet = bullet;
        }
    }
    /**
     * brief    通常State
     */
    public class NormalState : BaseState
    {
        public NormalState(MagicBullet bullet) 
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
            //自分がプレイヤーと当たっていた時、プレイヤーにダメージを与え自分は消滅する
            Collider2D hit = base.bullet.HitCheck((int)M_System.LayerName.PLAYER);
            if (hit != null)
            {
                if (hit.tag == "Player")
                {
                    hit.GetComponent<CharacterObject>().Damage(this.bullet.GetAttackData());
                    base.bullet.Delete();
                    return;
                }
            }
            //地面に当たったら消す
            hit = base.bullet.HitCheck((int)M_System.LayerName.GROUND,Vector2.zero);
            if(hit != null)
            {
                base.bullet.Delete();
                return;
            }
            //時間による削除処理
            if(base.GetTime() > base.bullet.lifeCnt)
            {
                base.bullet.Delete();
                return;
            }
        }
    }
}