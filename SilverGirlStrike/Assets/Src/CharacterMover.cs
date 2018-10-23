using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMover : MonoBehaviour {
    
    Rigidbody2D rigid;
    BoxCollider2D boxCollider2D;
    float fallVelocity;

    Vector2 normal;


    RaycastHit2D[] resultArray;

    Collider2D[] result;


    // Use this for initialization
    void Start () {
        this.rigid = this.GetComponent<Rigidbody2D>();
        this.fallVelocity = 0.0f;

        resultArray = new RaycastHit2D[10];
        this.result = new Collider2D[10];
        this.boxCollider2D = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update () {
        ////ここが主要な部分
        //float axis = Input.GetAxis("RStickX") * 5.0f;

        //if (M_System.input.Down(SystemInput.Tag.JUMP)){
        //    //ジャンプ時にジャンプ力を渡す
        //    Jump(3.0f);
        //}

        ////横移動力、重力、接地フラグを渡す
        //UpdateVelocity(axis, 0.3f, this.foot.isFoot);
    }


    //横移動をセットし、物理挙動に速度を設定
    public void UpdateVelocity(float movePowerX, float gravity, bool onGround)
    {
        //接地時は落下速度リセット
        if (onGround)
        {
            if (this.fallVelocity < 0.0f)
            {
                this.fallVelocity = 0.0f;
            }
        }
        //落下
        this.fallVelocity -= gravity;

        Vector2 bl = (Vector2)this.transform.position;
        bl += boxCollider2D.offset + Vector2.left * boxCollider2D.size.x * 0.5f + Vector2.down * boxCollider2D.size.y * 0.5f;
        Vector2 br = (Vector2)this.transform.position;
        br += boxCollider2D.offset + Vector2.right * boxCollider2D.size.x * 0.5f + Vector2.down * boxCollider2D.size.y * 0.5f;

        //向きによってレイの優先度変更
        if (movePowerX > 0)
        {
            br += Vector2.up * 0.1f + new Vector2(movePowerX, 0.0f).normalized * 0.01f;
            ChangeNormal(movePowerX, br, bl);
        }
        else
        {
            bl += Vector2.up * 0.1f + new Vector2(movePowerX, 0.0f).normalized * 0.01f;
            ChangeNormal(movePowerX, bl, br);
        }


        if (this.fallVelocity > 0.0f)
        {
            this.normal = Vector2.zero;
        }

        Vector2 velocityX = new Vector2(movePowerX, 0.0f);
        //かべずり
        velocityX = velocityX - Vector2.Dot(velocityX, this.normal) * this.normal;
        if (velocityX.x != 0.0f)
        {
            velocityX *= movePowerX / velocityX.x;
        }

        this.rigid.velocity = new Vector2(0.0f, this.fallVelocity) + velocityX;

        ChangeFriction();
    }

    //ジャンプ（縦移動力を設定）
    public void Jump(float jumpPower)
    {
          this.fallVelocity = jumpPower;
    }

    void ChangeFriction()
    {
        //泊っているときはずり落ち防止で摩擦を十分につける
        if (this.rigid.velocity.x == 0)
        {
            if (this.rigid.sharedMaterial.friction != 1000)
            {
                this.rigid.sharedMaterial.friction = 1000;
                Collider2D[] result = new Collider2D[10];
                int num = this.rigid.GetAttachedColliders(result);
                for (int i = 0; i < num; ++i)
                {
                    result[i].enabled = false;
                    result[i].enabled = true;
                }
            }
        }
        else if (this.rigid.sharedMaterial.friction != 0)
        {
            this.rigid.sharedMaterial.friction = 0;
            int num = this.rigid.GetAttachedColliders(result);
            for (int i = 0; i < num; ++i)
            {
                result[i].enabled = false;
                result[i].enabled = true;
            }
        }
    }


    void ChangeNormal(float moveVectorX, Vector2 frontFoot, Vector2 backFoot)
    {
        //レイキャスト2本でどうにかしたい
        {
            var result = Physics2D.Raycast(frontFoot, Vector2.down, 0.4f, (int)M_System.LayerName.GROUND);

            //地面の法線獲得
            if (result.collider != null)
            {
                this.normal = result.normal;
            }
            else
            {
                this.normal = Vector2.zero;
            }

        }

        //下り坂の場合は進行方向の後ろ側の足元のレイを優先
        if (Vector2.Angle(this.normal, new Vector2(moveVectorX, 0.0f)) <= 90)
        {
            var result = Physics2D.Raycast(backFoot, Vector2.down, 0.4f, (int)M_System.LayerName.GROUND);

            //地面の法線獲得
            if (result.collider != null)
            {
                this.normal = result.normal;
            }
            else
            {
                this.normal = Vector2.zero;
            }
        }
    }
}
