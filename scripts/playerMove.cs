using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMove : MonoBehaviour {

    Rigidbody2D rigid;
    public float gravity;
    float fallVelocity;

    Vector2 normal;


    RaycastHit2D[] resultArray;
    public foot foot;

    Collider2D[] result;


    // Use this for initialization
    void Start () {
        this.rigid = this.GetComponent<Rigidbody2D>();
        this.fallVelocity = 0.0f;

        resultArray = new RaycastHit2D[10];
        this.result = new Collider2D[10];
    }

    // Update is called once per frame
    void Update () {

        if (this.foot.isFoot)
        {
            if (this.fallVelocity < 0.0f)
            {
                this.fallVelocity = 0.0f;
            }
        }

        this.fallVelocity -= this.gravity;




        float axis = Input.GetAxis("Horizontal") * 5.0f;

        var a = GetComponent<BoxCollider2D>();
        Vector2 bl = (Vector2)this.transform.position + a.offset + Vector2.left * a.size.x * 0.5f + Vector2.down * a.size.y * 0.5f;
        Vector2 br = (Vector2)this.transform.position + a.offset + Vector2.right * a.size.x * 0.5f + Vector2.down * a.size.y * 0.5f;

        if (axis > 0)
        {
            br += Vector2.up * 0.1f + new Vector2(axis, 0.0f).normalized * 0.01f;
            ChangeNormal(axis, br, bl);
        }
        else
        {
            bl += Vector2.up * 0.1f + new Vector2(axis, 0.0f).normalized * 0.01f;
            ChangeNormal(axis, bl, br);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            this.fallVelocity = 5.0f;
            this.normal = Vector2.zero;
        }


        Vector2 velocityX = new Vector2(axis, 0.0f);
        //かべずり
        velocityX = velocityX - Vector2.Dot(velocityX, this.normal) * this.normal;
        if (velocityX.x != 0.0f)
        {
            velocityX *= axis / velocityX.x;
        }


        this.rigid.velocity = new Vector2(0.0f, this.fallVelocity) + velocityX;

        if(this.rigid.velocity.x == 0)
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
        else if(this.rigid.sharedMaterial.friction != 0)
        {
            this.rigid.sharedMaterial.friction = 0;
            int num = this.rigid.GetAttachedColliders(result);
            for (int i = 0; i < num; ++i)
            {
                result[i].enabled = false;
                result[i].enabled = true;
            }
        }

        Debug.DrawRay(this.transform.position, new Vector3(this.rigid.velocity.x, this.rigid.velocity.y, 0.0f) * 100, Color.green, 0, false);
    }


    void ChangeNormal(float moveVectorX, Vector2 frontFoot, Vector2 backFoot)
    {
        //レイキャスト2本でどうにかしたい
        {
            var result = Physics2D.Raycast(frontFoot, Vector2.down, 0.4f, 2);

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
            var result = Physics2D.Raycast(backFoot, Vector2.down, 0.4f, 2);

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
