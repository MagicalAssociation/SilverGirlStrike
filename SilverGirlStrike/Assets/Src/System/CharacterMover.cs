using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//変更履歴
//2018/12/01 板倉：仕様変更。RigidBodyで動かすのをやめ、こっち側で制御しやすいCollisionのみの処理にした
//2018/12/04 板倉：速度面での不利が解消できないため、RigidBodyを利用した処理が復活
//2018/12/06 板倉：IsJump()で上昇中かどうかを取得できるようになった

public class CharacterMover : MonoBehaviour
{

    Rigidbody2D rigid;
    BoxCollider2D boxCollider2D;
    float fallVelocity;

    float gravityRatio;

    Vector2 normal;


    RaycastHit2D[] resultArray;

    Collider2D[] result;

    bool prevOnGround;

    //重力の影響を受けない場合に設定
    bool acvtiveGravity;


    // Use this for initialization
    void Start()
    {
        this.gravityRatio = 1.0f;

        this.rigid = this.GetComponent<Rigidbody2D>();
        this.rigid.sharedMaterial = new PhysicsMaterial2D();
        this.fallVelocity = 0.0f;

        resultArray = new RaycastHit2D[10];
        this.result = new Collider2D[10];
        this.boxCollider2D = GetComponent<BoxCollider2D>();

        this.acvtiveGravity = true;
        this.prevOnGround = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    /**
     * @brief   現在の落下速度を取得する
     * @return float 落下速度
     */

    public float GetFallVelocity()
    {
        return this.fallVelocity;
    }
    /**
     * @brief   現在落ちているかを返す
     * @return bool 落ちているならtrue
     */
    public bool IsFall()
    {
        return this.fallVelocity < 0.0f;
    }
    /**
    * @brief   現在上昇しているかを返す
    * @return bool 上昇しているならtrue
    */
    public bool IsJump()
    {
        return this.fallVelocity > 0.0f;
    }

    //（未実装）横移動とか縦移動をもっとスムーズに扱える関数
    public void MoveHorizontal(float movePower, bool onGround)
    {

    }
    //（未実装）横移動とか縦移動をもっとスムーズに扱える関数
    public void MoveVartical(float movePower, float gravity)
    {

    }

    //重力が効く割合を設定
    public void SetGravityRatio(float ratio)
    {
        this.gravityRatio = ratio;
    }

    //横移動をセットし、物理挙動に速度を設定、縦移動は落下速度に影響を及ぼさないので、それをやりたい場合はJump()にて落下速度を設定するといい
    public void UpdateVelocity(float movePowerX, float movePowerY, float gravity, bool onGround)
    {

        if (this.acvtiveGravity)
        {
            //落下
            this.fallVelocity -= gravity * this.gravityRatio;
            //接地しなくなった際には落下速度リセット
            if (this.prevOnGround && !onGround)
            {
                if (this.fallVelocity < 0.0f)
                {
                    this.fallVelocity = 0.0f;
                }
            }
        }

        //ボックスコリジョンから四角形の頂点を取得、レイの発生に使う
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

        //空中にいるときは、法線を使用しない
        if (this.fallVelocity > 0.0f || !this.acvtiveGravity)
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

        if (this.acvtiveGravity)
        {
            this.rigid.velocity = new Vector2(0.0f, this.fallVelocity + movePowerY) + velocityX;
        }
        else
        {
            this.rigid.velocity = new Vector2(0.0f, movePowerY) + velocityX;
        }

        //泊っているときはずり落ち防止で摩擦を十分につける
        if (this.rigid.velocity.x == 0 && onGround)
        {
            ChangeFriction(true);
        }
        else
        {
            ChangeFriction(false);
        }

        //接地情報を記録
        this.prevOnGround = onGround;
    }

    //ジャンプ（縦移動力を設定）
    public void Jump(float jumpPower)
    {
        this.fallVelocity = jumpPower;
    }
    //重力設定
    public void SetActiveGravity(bool active, bool isKeepFallVelocity)
    {
        if (!isKeepFallVelocity)
        {
            this.fallVelocity = 0.0f;
        }
        this.acvtiveGravity = active;
    }

    void ChangeFriction(bool isStop)
    {       //泊っているときはずり落ち防止で摩擦を十分につける
        if (isStop)
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