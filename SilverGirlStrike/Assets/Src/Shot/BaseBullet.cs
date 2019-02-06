using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Bullet
{
    //弾一発の情報--------------------------------------------------------------------------------------------------
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
        public Vector3 offset;
        public GameObject offsetObject;
    }
    //生成する攻撃の全情報--------------------------------------------------------------------------------------------------
    [System.Serializable]
    public class BulletParameter
    {
        //! 生成弾数
        public int bulletNum;
        //! 攻撃間隔
        public int interval;
        //! 攻撃のターゲット検索機能
        public TargetSearch targetSearch;
        //! 攻撃データ
        public BulletData[] bulletData;
        //検索をかけその結果をBulletDataに渡してくれます
        public void Search()
        {
            if (targetSearch.enable == true)
            {
                for (int i = 0; i < bulletData.Length; ++i)
                {
                    bulletData[i].target = targetSearch.Search();
                }
            }
        }
        //弾を打つ角度に指定値をプラスする(全て同じ)
        public void AdditionAngle(float angle)
        {
            for (int i = 0; i < bulletData.Length; ++i)
            {
                bulletData[i].angle += angle;
            }
        }
    }
    //Bulletの元class--------------------------------------------------------------------------------------------------
    public abstract class BaseBullet : CharacterObject
    {

        public NarrowAttacker attacker;

        //! 移動速度
        public float moveSpeed;
        //! 寿命
        public int lifeCnt;

        public Vector2 move;
        AttackData attackData;
        public static void Create(CharacterObject characterObject, BulletData bulletData, Vector3 position)
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
            BaseBullet bullet = Object.Instantiate(bulletData.attackObject, pos, Quaternion.identity) as Bullet.BaseBullet;
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

        /**
         * brief    攻撃を飛ばす方向を指定する
         * param[in] float angle 角度
         */
        public void SetShotAngle(float angle)
        {
            move.x = Mathf.Cos(angle * (Mathf.PI / 180));
            move.y = Mathf.Sin(angle * (Mathf.PI / 180));

        }

        /**
         * brief    攻撃を飛ばす方向を指定オブジェクトにする
         * param[in] GameObject target ターゲット
         */
        public void SetShotTarget(GameObject target)
        {
            move = (target.transform.position - this.transform.position).normalized;
            float di = Mathf.Atan2(move.y, move.x);
            move.x = Mathf.Cos(di);
            move.y = Mathf.Sin(di);
        }
    }
}