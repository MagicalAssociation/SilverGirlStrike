using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PhysicsObject : MonoBehaviour
{
    //! 移動値
    public Vector2 move;
    //! 重力の値(テスト値)
    public float GRAVITY = 0.03f;
    //! 摩擦のための値(テスト値)
    public float FINSPEED = 0.03f;
    //! ジャンプ力(テスト値)
    public float JUMPPOWER = 10.0f;
    //! 足元の判定をいれておく変数(関数にしたからいらないかも)
    public bool footHit;
    //! 現在のジャンプ可能回数
    public int jumpCnt;
    //! 移動ベクトル
    public Vector3 moveDirection;
    //! 最大ジャンプ数
    public int MAXJUMPCOUNT = 1;
    void Start()
    {
        //全値の初期化
        this.move = new Vector2();
        this.moveDirection = Vector3.zero;
        this.footHit = false;
        this.jumpCnt = 0;
    }
    // Update is called once per frame
    void Update()
    {
        //足元判定(ジャンプ回数を復活させる)
        if(this.FootCheck())
        {
            this.jumpCnt = 0;
        }
        //横移動入力
        this.move.x = Input.GetAxis("RStickX");
        //ジャンプ処理(足元があるときのみ行う)
        if(M_System.input.Down(SystemInput.Tag.JUMP) && this.jumpCnt < this.MAXJUMPCOUNT)
        {
            //=にすることで空中でも地上でも同じ強さでジャンプする
            this.move.y = this.JUMPPOWER;
            this.jumpCnt++;
        }
        //重力処理
        this.GravityUpdate();
        //移動処理
        this.MoveUpdate();
        //実際の移動
        this.MoveObject();
    }

    /**
     * 移動のmove値に重力とか摩擦の計算を行う
     */
    void MoveUpdate()
    {
        //!横移動は少しだけ摩擦を残す(1f~3fくらい)ための処理!
        if(this.move.x > 0.0f)
        {
            //右移動の処理
            this.move.x = Mathf.Max(this.move.x - this.FINSPEED, 0.0f);
            this.moveDirection = Vector3.right;
        }
        else
        {
            //左移動の処理
            this.move.x = Mathf.Min(this.move.x + this.FINSPEED, 0.0f);
            this.moveDirection = Vector3.left;
        }
        if (this.move.y > 0.0f || !this.footHit)
        {
            //前まで移動処理があった
        }
        else
        {
            //地面に足がついているなら移動はしない
            this.move.y = 0.0f;
        }
        
    }
    /*
     * 重力の値を移動値に加算する
     */ 
    void GravityUpdate()
    {
        //移動値の重力を渡す
        this.move.y -= GRAVITY;
    }
    /*
     * Rayとか飛ばして移動を行う
     */ 
    void MoveObject()
    {
        //足元判定で当たっているCollisionのデータを取得する
        Collider2D hitobject;
        this.FootCheck(out hitobject);

        //確認用線描画
        Debug.DrawRay(this.transform.position - new Vector3(0.0f, 1.1f / 2.0f, 0.0f), new Vector2(0.0f, -0.03f), Color.red, 0.03f);
        Debug.DrawRay(this.transform.position - new Vector3(0.3f / 2.0f * -1.0f - 0.01f, 1.1f / 2.0f, 0.0f), new Vector2(0.0f, -0.03f), Color.blue, 0.03f);
        Debug.DrawRay(this.transform.position - new Vector3(0.3f / 2.0f + 0.01f, 1.1f / 2.0f, 0.0f), new Vector2(0.0f, -0.03f), Color.green, 0.03f);

        //縦移動の移動処理
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position - new Vector3(0.0f, 1.1f / 2.0f, 0.0f), new Vector2(0.0f, this.move.y), this.move.y, (int)M_System.LayerName.GROUND);
        if (hit.collider != null)
        {
            this.move.y = 0.0f;
        }
        else
        {
            this.GetComponent<Rigidbody2D>().velocity = new Vector3(this.GetComponent<Rigidbody2D>().velocity.x, this.move.y * 15.0f, 0.0f);
        }


        //坂道なら移動ベクトルを変更する
        //相手のオブジェクトの角度を取得

        


        //横移動の移動処理
        hit = Physics2D.Raycast(this.transform.position, new Vector2(this.move.x, 0.0f), this.move.x, (int)M_System.LayerName.GROUND);
        if (hit.collider != null)
        {
            this.move.x = 0.0f;
        }
        else
        {
            //空中の時は縦の移動処理がすでにあるので処理をifで分ける
            if (hitobject && this.move.y == 0.0f)
            {
                //
                //ここに坂道の判定とそのベクトルの変化を書く
                //壁ずりベクトル = 進行ベクトル-Dot(進行ベクトル,法線ベクトルnormal) * 法線ベクトルnormal
                //
                Vector3 dir = this.moveDirection - Vector3.Dot(this.moveDirection, hitobject.transform.eulerAngles.normalized) * hitobject.transform.eulerAngles.normalized;
                Debug.Log(dir);

                //ここのxとyに壁ずりベクトルをかければいけるはず
                this.GetComponent<Rigidbody2D>().velocity = new Vector2(this.move.x * 5.0f, this.move.x * 5.0f * dir.y);
            }
            else
            {
                //こっちは問題ない
                this.GetComponent<Rigidbody2D>().velocity = new Vector2(this.move.x * 5.0f, this.GetComponent<Rigidbody2D>().velocity.y);
            }
        }

    }
    /*
     * 足元の判定
     */ 
    bool FootCheck(out Collider2D hit2D)
    {
        //足元の判定をする
        RaycastHit2D foothitcheck = Physics2D.Raycast(this.transform.position - new Vector3(0.0f, 1.1f / 2.0f, 0.0f), new Vector2(0.0f, -0.03f), 0.03f, (int)M_System.LayerName.GROUND);
        RaycastHit2D foothitcheck1 = Physics2D.Raycast(this.transform.position - new Vector3(0.3f / 2.0f * -1.0f - 0.01f, 1.1f / 2.0f, 0.0f), new Vector2(0.0f, -0.03f), 0.03f, (int)M_System.LayerName.GROUND);
        RaycastHit2D foothitcheck2 = Physics2D.Raycast(this.transform.position - new Vector3(0.3f / 2.0f + 0.01f, 1.1f / 2.0f, 0.0f), new Vector2(0.0f, -0.03f), 0.03f, (int)M_System.LayerName.GROUND);

        if (foothitcheck.collider != null ||
            foothitcheck1.collider != null ||
            foothitcheck2.collider != null)
        {
            this.footHit = true;
            //足元にあるColliderの情報をいれる
            if (foothitcheck.collider)
            {
                hit2D = foothitcheck.collider;
            }
            else if (foothitcheck1.collider)
            {
                hit2D = foothitcheck1.collider;
            }
            else
            {
                hit2D = foothitcheck2.collider;
            }
        }
        else
        {
            this.footHit = false;
            //足元にColliderはないのでnullをいれる
            hit2D = null;
        }
        //変数かReturnいらない
        return this.footHit;
    }
    bool FootCheck()
    {
        //足元の判定をする
        RaycastHit2D foothitcheck = Physics2D.Raycast(this.transform.position - new Vector3(0.0f, 1.1f / 2.0f, 0.0f), new Vector2(0.0f, -0.03f), 0.03f, (int)M_System.LayerName.GROUND);
        RaycastHit2D foothitcheck1 = Physics2D.Raycast(this.transform.position - new Vector3(0.3f / 2.0f * -1.0f - 0.01f, 1.1f / 2.0f, 0.0f), new Vector2(0.0f, -0.03f), 0.03f, (int)M_System.LayerName.GROUND);
        RaycastHit2D foothitcheck2 = Physics2D.Raycast(this.transform.position - new Vector3(0.3f / 2.0f + 0.01f, 1.1f / 2.0f, 0.0f), new Vector2(0.0f, -0.03f), 0.03f, (int)M_System.LayerName.GROUND);

        if (foothitcheck.collider != null ||
            foothitcheck1.collider != null ||
            foothitcheck2.collider != null)
        {
            this.footHit = true;
            return true;
        }
        else
        {
            this.footHit = false;
            return false;
        }
    }
}