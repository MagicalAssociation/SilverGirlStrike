using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCreate05 : CharacterObject/*MonoBehaviour*/
{

    private bool HitCheck;              //playerか地面に当たったかどうか
    private float gravity;              //球が落下する重力
    private bool isCreate;              //生成されているかどうか
    private GameObject attackbullet;
    private Vector3 enemyPos;           //敵の位置を格納
    private bool canMove;               //現在球が移動可能かどうか

    //テスト追加
    private float animCnt;                //消えるアニメーションが終わるまでの時間をカウント

    public NarrowAttacker narrowAttacker;

    // Use this for initialization
    void Start () {
        this.HitCheck = false;
        this.gravity = 0.0f;
        this.isCreate = false;
        canMove = true;

        //テスト追加
        this.animCnt = 0.0f;
    }

    // Update is called once per frame
    void Update () {
        if(this.canMove)
        {
            isCreate = true;
        }
        if (this.isCreate)
        {
            if (!HitCheck)
            {
                Move();
            }
        }

        if(HitCheck)
        {
            ExplosionBullet();
        }


        //テスト追加
        narrowAttacker.StartAttack();

        //自分がプレイヤーと当たっていた時、プレイヤーにダメージを与え自分は消滅する
        narrowAttacker.AttackJudge((int)M_System.LayerName.PLAYER);
        if (narrowAttacker.IsHit())
        {
            //テスト追加
            this.animCnt += Time.deltaTime ;

            //自身が消滅する処理の追加
            //if (animCnt > 100.0f)
            {
                Destroy(this.gameObject);
            }
            return;
        }
        //地面に当たったら消す
        var hit = narrowAttacker.AttackJudge((int)M_System.LayerName.GROUND);
        if (hit)
        {
            //テスト追加
            this.animCnt += Time.deltaTime;

            //自身を消す処理を追加
            //if (this.animCnt > 100.0f)
            {
                Destroy(this.gameObject);
            }
            return;
        }
    }

    //球の動き(下に直線落下)
    private void Move()
    {
        //transformPositionを変更して移動させる
        this.gravity -= 0.01f;
        Vector3 movey;
        movey.x = this.transform.position.x;
        movey.y = this.transform.position.y + gravity;

        movey.z = this.transform.position.z;

        this.transform.position = movey;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        ////プレイヤと当たった時爆発
        //if(other.tag=="Player")
        //{
        //    HitCheck = true;
        //    this.narrowAttacker.StartAttack();
        //}
        ////または、地面に接触した時爆発、爆風に当たってもダメージ
        //string layerName = LayerMask.LayerToName(other.gameObject.layer);

        //if (layerName == "Ground")
        //{
        //    HitCheck = true;
        //    this.narrowAttacker.StartAttack();
        //}
    }

    //弾の生成、Enemy05内で呼び出す
    public GameObject Create(Vector3 pos,GameObject bullet)         //場所とgameObjectの情報を渡してもらう。
    {
        //もはやattackbulletっていらない??
        //生成はEnemy05で行ってる。ポジションも指定

        //テスト追加
        attackbullet = bullet;
        attackbullet.transform.position = pos;

        ////this.isCreate = true;
        ////テスト追加
        this.canMove = false;

        return attackbullet;
    }

    //弾の爆発で当たり判定を拡大する処理
    public void ExplosionBullet()
    {
        //デバッグ用

        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        collider.radius += 0.01f;
    }


    //テスト追加(MagicBulletの真似)
    public override void UpdateCharacter()
    {
        throw new System.NotImplementedException();
    }

    public override bool Damage(AttackData attackData)
    {
        return false;
    }

    public override void ApplyDamage()
    {
    }

    //球を動かす処理をここに?
    public override void MoveCharacter()
    {
        throw new System.NotImplementedException();
    }
}
