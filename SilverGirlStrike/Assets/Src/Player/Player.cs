using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//編集履歴
//わからん　金子（作成）
//2018/10/27　板倉　コメントなど追加、余計なpublicをできるだけ減らしてみた

//プレイヤーの移動・行動・ステート管理処理
public class Player : MonoBehaviour
{
    public CharacterMover mover;
    public Foot foot;


    //アンカーの値
    public float anchorMaxMoveSpeed;
    public float anchorMoveAcceleration;
    public AnchorSelector anchor;
    GameObject anchorObject;
    Vector2 targetDirection;

    //移動関連のパラメータ値
    public float playerMoveSpeed;
    public float gravity;
    //ジャンプ関連
    public float jumpPower;
    public int maxJumpNumber;
    public int nowJumpNumber;

    public int hp;

    float anchorCurrentMoveSpeed;
    int timeCnt;
    Vector2 axis;
    BoxCollider2D boxCollider;


    Vector2 moveVector;

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

    }
	
	// Update is called once per frame
	void Update ()
    {

        this.moveVector = Vector2.zero;
        Mode();
        if(this.state != this.preState)
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

        mover.UpdateVelocity(this.moveVector.x, this.moveVector.y, this.gravity, this.foot.CheckHit());

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

                    if (!this.foot.CheckHit())
                    {
                        this.state = State.FALL;
                    }
                    else
                    {
                        this.nowJumpNumber = 0;
                    }

                    axis.x = Input.GetAxis("RStickX") * playerMoveSpeed;
                    if (axis.x != 0.0f)
                    {
                        this.state = State.WALK;
                    }
                    if(M_System.input.Down(SystemInput.Tag.WIRE))
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
                    axis.x = Input.GetAxis("RStickX") * playerMoveSpeed;
                    if (axis.x == 0.0f)
                    {
                        this.state = State.NORMAL;
                    }
                    if (M_System.input.Down(SystemInput.Tag.WIRE))
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
            case State.JUMP:
                {
                    //頭がオブジェクトに当たった時落ちる行動にモード変更
                    
                    if(this.mover.IsFall())
                    {
                        this.state = State.FALL;
                    }
                    if (M_System.input.Down(SystemInput.Tag.WIRE))
                    {
                        this.state = State.WIRE;
                    }
                }
                break;
            case State.FALL:
                {
                    if(this.foot.CheckHit())
                    {
                        //接地したときにジャンプ回数を回復
                        this.state = State.NORMAL;
                        this.nowJumpNumber = 0;
                    }
                    if (M_System.input.Down(SystemInput.Tag.WIRE))
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
                    //待機モーション中は、速度ゼロにしたいのでこう書く
                    this.moveVector = new Vector2(0.0f, 0.0f);
                }
                break;
            case State.JUMP:
                {
                    if(this.timeCnt == 0 && this.preState != State.WIRE)
                    {
                        this.mover.Jump(this.jumpPower);
                    }
                    axis.x = Input.GetAxis("RStickX") * playerMoveSpeed;
                    this.moveVector = new Vector2(axis.x, 0.0f);
                }
                break;
            case State.FALL:
                {
                    axis.x = Input.GetAxis("RStickX") * playerMoveSpeed;
                    this.moveVector = new Vector2(axis.x, 0.0f);
                }
                break;
            case State.WALK:
                {
                    this.moveVector = new Vector2(axis.x, 0.0f);
                }
                break;
            case State.WIRE:
                {
                    //ステートの初期化処理
                    if (this.timeCnt == 0)
                    {
                        Vector2 dire = new Vector2(Input.GetAxis("RStickX"), Input.GetAxis("RStickY") * -1);
                        
                        if (dire == new Vector2(0,0))
                        {
                            dire = new Vector2(1, 0);
                        }

                        //アンカーを見つけて、そこへ向かう
                        Debug.DrawRay(this.transform.position, new Vector3(dire.x, dire.y), Color.green, 1);
                        this.mover.SetActiveGravity(false, true);
                        this.anchor.FindAnchor(this.transform.position, new Vector2(this.transform.position.x + dire.x, this.transform.position.y + dire.y), out this.anchorObject);

                        Debug.Log(this.anchorObject);
                        //アンカーがなかったらステートをNORMALに戻す
                        if (this.anchorObject == null)
                        {
                            this.mover.SetActiveGravity(true, true);
                            this.state = State.NORMAL;
                        }
                        else
                        {
                            //現在地から目標のアンカーへ向かうベクトル
                            this.targetDirection = new Vector2(this.anchorObject.transform.localPosition.x - this.transform.localPosition.x, this.anchorObject.transform.localPosition.y - this.transform.localPosition.y);
                            this.targetDirection.Normalize();
                            this.anchorCurrentMoveSpeed = 0.0f;
                        }
                    }
                    //アンカーが見つかっている場合にのみ処理を行う
                    if (this.anchorObject != null)
                    {
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
}
