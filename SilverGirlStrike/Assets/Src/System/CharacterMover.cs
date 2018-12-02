using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//変更履歴
//2018/12/01 板倉：仕様変更。RigidBodyで動かすのをやめ、こっち側で制御しやすいCollisionのみの処理にした

public class CharacterMover : MonoBehaviour {

    BoxCollider2D boxCollider2D;
    float fallVelocity;

    //地面の法線を格納
    Vector2 normal;

    //ヒット結果を格納する配列
    Collider2D[] result;

    bool prevOnGround;

    //重力の影響を受けない場合に設定
    bool acvtiveGravity;


    // Use this for initialization
    void Start () {
        this.fallVelocity = 0.0f;

        this.result = new Collider2D[10];
        this.boxCollider2D = GetComponent<BoxCollider2D>();

        this.acvtiveGravity = true;
        this.prevOnGround = false;
    }

    // Update is called once per frame
    void Update () {
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

    //（未実装）横移動とか縦移動をもっとスムーズに扱える関数
    public void MoveHorizontal(float movePower, bool onGround)
    {

    }
    //（未実装）横移動とか縦移動をもっとスムーズに扱える関数
    public void MoveVartical(float movePower, float gravity)
    {

    }


    //横移動をセットし、物理挙動に速度を設定、縦移動は落下速度に影響を及ぼさないので、それをやりたい場合はJump()にて落下速度を設定するといい
    public void UpdateVelocity(float movePowerX, float movePowerY, float gravity, bool onGround)
    {

        if (this.acvtiveGravity)
        {
            //落下
            this.fallVelocity -= gravity * (Time.deltaTime * 60.0f);
            //接地しなくなった際には落下速度リセット
            if (onGround)
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

        //向きによってレイの優先度変更(坂道の時に、より地面に近いほうを採用するため)
        if (movePowerX > 0)
        {
            //右移動
            br += Vector2.up * 0.1f + new Vector2(movePowerX, 0.0f).normalized * 0.01f;
            ChangeNormal(movePowerX, br, bl);
        }
        else
        {
            //左移動
            bl += Vector2.up * 0.1f + new Vector2(movePowerX, 0.0f).normalized * 0.01f;
            ChangeNormal(movePowerX, bl, br);
        }

        //空中にいるときは、法線を使用しない
        if (this.fallVelocity > 0.0f || !this.acvtiveGravity)
        {
            this.normal = Vector2.zero;
        }
        //法線の角度（坂道の傾き）によって坂扱いか壁扱いかを決める
        if(Mathf.Abs(Vector3.Angle(Vector3.up, this.normal)) < 45.0f * Mathf.Deg2Rad)
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

        //重力が利いているかどうかで移動処理が変わる
        Vector3 moveVectorX;
        Vector3 moveVectorY;
        if (this.acvtiveGravity)
        {
            moveVectorX = velocityX;
            moveVectorY = new Vector2(0.0f, this.fallVelocity + movePowerY);
        }
        else
        {
            moveVectorX = velocityX;
            moveVectorY = new Vector2(0.0f, movePowerY);
        }



        //実際の移動処理(縦横それぞれ)
        MoveCharacter((moveVectorX / 50.0f) * (Time.deltaTime * 60.0f));
        MoveCharacter((moveVectorY / 50.0f) * (Time.deltaTime * 60.0f));

        //接地情報を記録
        this.prevOnGround = onGround;


    }

    //少しづつ移動して判定
    void MoveCharacter(Vector3 moveVector)
    {
        if (moveVector.magnitude < 0.001f)
        {
            return;
        }
        LayerMask mask = new LayerMask();
        mask.value = (int)M_System.LayerName.GROUND;
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(mask);

        //Debug.Log(this.boxCollider2D.size);

        //判定付きで移動
        var a = new Vector2(this.transform.position.x, this.transform.position.y) + this.boxCollider2D.offset;
        var hitResult = Physics2D.BoxCast(a, this.boxCollider2D.size, 0.0f, moveVector.normalized, moveVector.magnitude, (int)M_System.LayerName.GROUND);
        if (hitResult.collider != null)
        {
            float fraction = hitResult.fraction;
            this.transform.position += moveVector * (fraction);
            Debug.Log("Fragtion:" + fraction.ToString());
        this.transform.position -= moveVector.normalized * 0.1f;
        }
        else
        {
            this.transform.position += moveVector;
            Debug.Log("nonFragtion");
        }

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
