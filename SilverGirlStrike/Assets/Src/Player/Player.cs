using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//編集履歴
//わからん　金子（作成）
//2018/10/27　板倉　コメントなど追加、余計なpublicをできるだけ減らしてみた
//2018/10/30 板倉　アニメーション追加、あとアンカーのヒットエフェクトを追加
//2018/10/30 板倉　効果音追加、超絶雑なのでシステム組んだら要修正

//プレイヤーの移動・行動・ステート管理処理
public class Player : MonoBehaviour
{
    public CharacterMover mover;
    public Foot foot;


    //アンカーの値
    public GameObject anchorHitEffect;
    public float anchorMaxMoveSpeed;
    public float anchorMoveAcceleration;
    public AnchorSelector anchor;
    GameObject anchorObject;
    Vector2 targetDirection;

    //移動関連のパラメータ値
    public float playerMoveSpeed;
    //高速移動時の値
    public float playerDashSpeed;
    //スピードの値を補完するための値
    float speedRatio;

    public float gravity;
    //ジャンプ関連
    public float jumpPower;
    public int maxJumpNumber;
    public int nowJumpNumber;
    bool onGround;

    public int hp;

    float anchorCurrentMoveSpeed;
    int timeCnt;
    Vector2 axis;
    BoxCollider2D boxCollider;


    Vector2 moveVector;

    Animator playerAnim;

    //ステートの定義
    public enum State
    {
        ATTACK1,
        ATTACK2,
        ATTACK3,
        JUMP_ATTACK1,
        JUMP_ATTACK2,
        JUMP_ATTACK3,
        NORMAL,
        WALK,
        JUMP,
        FALL,
        WIRE,
        TRANS_ASSAULT,
        JUMP_TRANS_ASSAULT,
    }
    public enum Direction
    {
        LEFT,
        RIGHT,
    }

    //ステート
    public State state;
    public State preState;
    public Direction direction;


    // Use this for initialization
    void Start () {
        this.mover = GetComponent<CharacterMover>();
        this.maxJumpNumber = 1;
        this.nowJumpNumber = 0;
        this.timeCnt = 0;
        this.direction = Direction.RIGHT;
        this.state = State.NORMAL;
        this.preState = State.NORMAL;
        this.axis = new Vector2();
        this.anchor = GetComponent<AnchorSelector>();
        this.anchorObject = null;
        this.boxCollider = GetComponent<BoxCollider2D>();
        this.moveVector = Vector2.zero;

        this.playerAnim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update ()
    {
        this.speedRatio += -0.02f;
        if(speedRatio < 0.0f)
        {
            this.speedRatio = 0.0f;
        }

        Mode();
        if (this.state != this.preState)
        {
            this.timeCnt = 0;
        }
        Move();
        //Debug.Log("State:" + this.state + "|Cnt:" + this.timeCnt);
        this.foot.LineDraw();
        this.timeCnt++;
        this.preState = this.state;
        Vector2 dire = new Vector2(Input.GetAxis("RStickX"), Input.GetAxis("RStickY") * -1);
        Debug.DrawRay(this.transform.position, new Vector3(dire.x, dire.y), Color.green, 0);
    }

    private void FixedUpdate()
    {
        //足元チェック
        this.onGround = this.foot.CheckHit();
        //物理関連の処理はFixedUpdateで呼ぶのがいいらしい
        mover.UpdateVelocity(this.moveVector.x, this.moveVector.y, this.gravity, this.onGround);
        this.moveVector = Vector2.zero;
    }

    /**
     * @brief   Stateの変更をメインに行う
     */
    void Mode()
    {
        switch(this.state)
        {
            case State.NORMAL:
                {

                    if (!this.onGround)
                    {
                        this.state = State.FALL;
                    }
                    else
                    {
                        this.nowJumpNumber = 0;
                    }

                    axis.x = FixXAxis(Input.GetAxis("RStickX")) * GetPlayerSpeed();
                    if (axis.x != 0.0f)
                    {
                        this.state = State.WALK;
                    }
                    if (M_System.input.Down(SystemInput.Tag.WIRE) && ShotAnchor())
                    {
                        this.state = State.WIRE;
                    }
                    if (M_System.input.Down(SystemInput.Tag.JUMP) && this.maxJumpNumber > this.nowJumpNumber)
                    {
                        this.nowJumpNumber++;
                        this.state = State.JUMP;
                    }
                }
                break;
            case State.WALK:
                {
                    axis.x = FixXAxis(Input.GetAxis("RStickX")) * GetPlayerSpeed();
                    if (axis.x == 0.0f)
                    {
                        this.state = State.NORMAL;
                    }
                    if (M_System.input.Down(SystemInput.Tag.WIRE) && ShotAnchor())
                        {
                            this.state = State.WIRE;
                    }
                    if (M_System.input.Down(SystemInput.Tag.JUMP) && this.maxJumpNumber > this.nowJumpNumber)
                    {
                        this.nowJumpNumber++;
                        this.state = State.JUMP;
                    }
                    if (!this.onGround)
                    {
                        this.state = State.FALL;
                    }
                }
                break;
            case State.JUMP:
                {
                    //頭がオブジェクトに当たった時落ちる行動にモード変更
                    
                    if(this.mover.IsFall())
                    {
                        this.state = State.FALL;
                    }
                    if (M_System.input.Down(SystemInput.Tag.WIRE) && ShotAnchor())
                        {
                        this.state = State.WIRE;
                    }
                }
                break;
            case State.FALL:
                {
                    if(this.onGround)
                    {
                        //接地したときにジャンプ回数を回復
                        this.state = State.NORMAL;
                        this.nowJumpNumber = 0;
                    }
                    if (M_System.input.Down(SystemInput.Tag.WIRE) && ShotAnchor())
                    {
                        this.state = State.WIRE;
                    }
                    if (M_System.input.Down(SystemInput.Tag.JUMP) && this.maxJumpNumber > this.nowJumpNumber)
                    {
                        this.nowJumpNumber++;
                        this.state = State.JUMP;
                    }
                }
                break;
            case State.WIRE:
                {
                    //進行方向と、現在のアンカーへ向かうベクトルを比較し、後ろにあったらJUMPに移行
                    float dot = Vector2.Dot(this.targetDirection, this.anchorObject.transform.position - this.transform.position);

                    if (this.anchorObject != null && dot < 0.0f)
                    {
                        //移行時、少しだけ滞空時間を延ばすためにジャンプのような挙動を行う
                        this.mover.SetActiveGravity(true, true);
                        this.mover.Jump(this.targetDirection.y * this.anchorCurrentMoveSpeed * 0.5f);
                        this.state = State.JUMP;
                        this.anchorObject = null;


                        this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                    }
                }
                break;
            case State.ATTACK1:
                {

                }
                break;
            case State.ATTACK2:
                {

                }
                break;
            case State.ATTACK3:
                {

                }
                break;
            case State.TRANS_ASSAULT:
                {

                }
                break;
            default:
                break;
        }
    }


    /**
     * @brief   Stateに合わせて移動を行う
     */ 
    void Move()
    {
        switch(this.state)
        {
            case State.NORMAL:
                {
                    //アニメ
                    if (this.timeCnt == 0)
                    {
                        this.playerAnim.Play("Idle");
                        if (this.preState == State.WALK)
                        {
                            this.playerAnim.Play("DashStop");
                        }
                        if(this.preState == State.JUMP || this.preState == State.FALL)
                        {
                            //着地
                            this.playerAnim.Play("onGround");
                            Sound.PlaySE("onGround");
                        }
                    }
                    //待機モーション中は、速度ゼロにしたいのでこう書く
                    this.moveVector = new Vector2(0.0f, 0.0f);
                }
                break;
            case State.JUMP:
                {
                    if(this.timeCnt == 0)
                    {
                        if (this.preState == State.WALK || this.preState == State.NORMAL || this.preState == State.FALL || this.preState == State.JUMP)
                        {
                            Sound.PlaySE("jump");
                        }
                        this.playerAnim.Play("JumpUp");

                        //ワイヤーからは独自ジャンプ処理があるのでここでは処理しない
                        if (preState != State.WIRE)
                        {
                            this.mover.Jump(this.jumpPower);
                        }

                        if (preState != State.WIRE)
                        {
                            //ヒットエフェクト
                            var effectObj = Instantiate(this.anchorHitEffect);
                            effectObj.transform.position = this.transform.position;
                            effectObj.transform.rotation *= Quaternion.AngleAxis(65.0f, Vector3.right);
                        }
                    }
                    axis.x = FixXAxis(Input.GetAxis("RStickX")) * GetPlayerSpeed();
                    this.moveVector = new Vector2(axis.x, 0.0f);
                    //移動方向にて向きを変える
                    ChangeDirectionFromMoveX(axis.x);
                }
                break;
            case State.FALL:
                {
                    if (this.timeCnt == 0)
                    {
                        this.playerAnim.Play("JumpDownStart");
                    }

                    axis.x = FixXAxis(Input.GetAxis("RStickX")) * GetPlayerSpeed();
                    this.moveVector = new Vector2(axis.x, 0.0f);
                    //移動方向にて向きを変える
                    ChangeDirectionFromMoveX(axis.x);
                }
                break;
            case State.WALK:
                {
                    //アニメ
                    if (this.timeCnt == 0)
                    {
                        this.playerAnim.Play("DashStart");
                    }
                    axis.x = FixXAxis(Input.GetAxis("RStickX")) * GetPlayerSpeed();
                    this.moveVector = new Vector2(axis.x, 0.0f);
                    //移動方向にて向きを変える
                    ChangeDirectionFromMoveX(axis.x);
                }
                break;
            case State.WIRE:
                {
                    //ステートの初期化処理
                    if (this.timeCnt == 0)
                    {

                        //アンカーショットの処理を以下で行う

                        this.mover.SetActiveGravity(false, true);

                        this.playerAnim.Play("anchorShot");

                        //ｶｲｰﾝ
                        Sound.PlaySE("wireHit");
                        //ヒットエフェクト
                        var effectObj = Instantiate(this.anchorHitEffect);
                        effectObj.transform.position = this.anchorObject.transform.position;

                        //現在地から目標のアンカーへ向かうベクトル
                        this.targetDirection = new Vector2(this.anchorObject.transform.localPosition.x - this.transform.localPosition.x, this.anchorObject.transform.localPosition.y - this.transform.localPosition.y);
                        this.targetDirection.Normalize();
                        this.anchorCurrentMoveSpeed = 0.0f;


                        //移動方向にて向きを変える
                        ChangeDirectionFromMoveX(this.targetDirection.x);

                        //頭をアンカーに向ける
                        float angle = Vector2.Angle(new Vector2(0.0f, 1.0f), this.targetDirection);
                        if (this.direction == Direction.RIGHT)
                        {
                            angle *= -1.0f;
                        }

                        this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                    }
                    if(this.timeCnt == 2)
                    {
                        //ﾊﾞｼｭ
                        Sound.PlaySE("wireShot");
                    }

                    //アンカーが見つかっている場合にのみ処理を行う
                    if (this.anchorObject != null && this.timeCnt > 2)
                    {
                        //速度をダッシュ中のそれにする
                        this.speedRatio = 1.0f;
                        //アンカーに向かっての移動
                        this.moveVector = targetDirection * this.anchorCurrentMoveSpeed;


                        //最高値まで加速
                        if (this.anchorMaxMoveSpeed > this.anchorCurrentMoveSpeed)
                        {
                            this.anchorCurrentMoveSpeed += anchorMoveAcceleration;
                            if(this.anchorMaxMoveSpeed > this.anchorCurrentMoveSpeed)
                            {
                                //最大値にクランプ
                                this.anchorCurrentMoveSpeed = this.anchorMaxMoveSpeed;
                            }
                        }
                    }
                    //いつまでたってもステートが変化しない場合は時間で強制的に変化させる
                    if(this.timeCnt > 500)
                    {
                        this.state = State.NORMAL;
                    }
                }
                break;
            case State.ATTACK1:
                {

                }
                break;
            case State.ATTACK2:
                {

                }
                break;
            case State.ATTACK3:
                {

                }
                break;
            case State.TRANS_ASSAULT:
                {

                }
                break;
            default:
                break;
        }
    }


    //移動の値から方向を振り分ける関数
    void ChangeDirectionFromMoveX(float xMove)
    {
        if (xMove > 0.0f)
        {
            ChangeDirection(Direction.RIGHT);
        }
        else if (xMove < 0.0f)
        {
            ChangeDirection(Direction.LEFT);
        }
    }

    //向き変更関数
    void ChangeDirection(Direction direction)
    {
        if(this.direction != direction)
        {
            var scale = this.transform.localScale;
            scale.x *= -1;
            this.transform.localScale = scale;
        }

        this.direction = direction;
    }

    //デジタルな操作を実現する入力補正関数
    float FixXAxis(float axisX)
    {
        if(axisX > 0.2)
        {
            return 1.0f;
        }
        if (axisX < -0.2)
        {
            return -1.0f;
        }
        return 0.0f;
    }

    //値を計算して返す
    float GetPlayerSpeed()
    {
        return this.playerDashSpeed * this.speedRatio + this.playerMoveSpeed * (1.0f - this.speedRatio);
    }

    bool ShotAnchor()
    {
        Vector2 direction = new Vector2(Input.GetAxis("RStickX"), Input.GetAxis("RStickY") * -1);

        //スティックがニュートラルの際は、まっすぐX軸にそったレイを設定する
        if (direction.magnitude == 0.0f)
        {
            //向きによって違う
            if (this.direction == Direction.RIGHT)
            {
                direction = new Vector2(1, 0);
            }
            else
            {
                direction = new Vector2(-1, 0.0f);
            }

        }

        //アンカーを見つけて、そこへ向かう
        Debug.DrawRay(this.transform.position, new Vector3(direction.x, direction.y), Color.green, 1);
        this.anchor.FindAnchor(this.transform.position, new Vector2(this.transform.position.x + direction.x, this.transform.position.y + direction.y), out this.anchorObject);

        Debug.Log(this.anchorObject);
        //アンカーがなかったらfalse
        if (this.anchorObject == null)
        {
            return false;
        }
        return true;
    }
}
