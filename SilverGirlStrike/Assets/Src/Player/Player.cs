using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public CharacterMover mover;
    public Foot foot;
    public float jumpPower;
    public int maxJumpNumber;
    public int nowJumpNumber;
    public float maxSpeed;
    public float nowSpeed;
    public int timeCnt;
    public Vector2 axis;
    public float gravity;
    public int hp;
    public AnchorSelector anchor;
    public GameObject anchorObject;
    public BoxCollider2D boxCollider;
    public Vector2 targetDistance;
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
        this.jumpPower = 12.0f;
        this.timeCnt = 0;
        this.direction = Direction.RIGHT;
        this.state = State.NORMAL;
        this.preState = State.NORMAL;
        this.axis = new Vector2();
        this.hp = 10;
        this.anchor = GetComponent<AnchorSelector>();
        this.anchorObject = null;
        this.boxCollider = GetComponent<BoxCollider2D>();

    }
	
	// Update is called once per frame
	void Update ()
    {
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

                    this.mover.SetActiveGravity(true);
                    if (!this.foot.CheckHit())
                    {
                        this.state = State.FALL;
                    }
                    else
                    {
                        this.nowJumpNumber = 0;
                    }

                    axis.x = Input.GetAxis("RStickX") * 5.0f;
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
                    axis.x = Input.GetAxis("RStickX") * 5.0f;
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
                    //アンカーとの距離が近くなったらここでFallに移行する
                    if(Physics2D.OverlapCircle(this.transform.position,this.boxCollider.size.x / 2.0f,(int)M_System.LayerName.ANCHOR) != null)
                    {
                        this.mover.Jump(this.jumpPower * 0.5f);
                        this.mover.SetActiveGravity(true);
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
                    //慣性を消すために書いてある
                    mover.UpdateVelocity(0.0f, 0.0f, 0.5f, this.foot.CheckHit());
                }
                break;
            case State.JUMP:
                {
                    if(this.timeCnt == 0 && this.preState != State.WIRE)
                    {
                        this.mover.Jump(this.jumpPower);
                    }
                    axis.x = Input.GetAxis("RStickX") * 5.0f;
                    mover.UpdateVelocity(axis.x, 0.0f,0.5f, this.foot.CheckHit());
                }
                break;
            case State.FALL:
                {
                    axis.x = Input.GetAxis("RStickX") * 5.0f;
                    mover.UpdateVelocity(axis.x, 0.0f, 0.5f, this.foot.CheckHit());
                }
                break;
            case State.WALK:
                {
                    mover.UpdateVelocity(axis.x, 0.0f, 0.5f, this.foot.CheckHit());
                }
                break;
            case State.WIRE:
                {
                    if (this.timeCnt == 0)
                    {
                        Vector2 dire = new Vector2(Input.GetAxis("RStickX"), Input.GetAxis("RStickY") * -1);
                        
                        if (dire == new Vector2(0,0))
                        {
                            dire = new Vector2(1, 0);
                        }
                        // Vector2 dire = new Vector2(1, 0);
                        //Debug.DrawLine(this.transform.position, new Vector2(dire.x + this.transform.position.x, dire.y + this.transform.position.y));
                        Debug.DrawRay(this.transform.position, new Vector3(dire.x, dire.y), Color.green, 1);
                        this.mover.SetActiveGravity(false);
                        this.anchor.FindAnchor(this.transform.position, new Vector2(this.transform.position.x + dire.x, this.transform.position.y + dire.y), out this.anchorObject);

                        Debug.Log(this.anchorObject);
                        if (this.anchorObject == null)
                        {
                            this.state = State.NORMAL;
                        }
                        else
                        {
                            this.targetDistance = new Vector2(this.anchorObject.transform.localPosition.x - this.transform.localPosition.x, this.anchorObject.transform.localPosition.y - this.transform.localPosition.y);
                            this.nowSpeed = 0.0f;
                        }
                    }
                    if (this.anchorObject != null)
                    {
                        
                        Vector2 v = targetDistance * this.nowSpeed;
                        mover.UpdateVelocity(v.x, v.y, 0.0f, this.foot.CheckHit());
                        if (this.maxSpeed > this.nowSpeed)
                        {
                            this.nowSpeed += 1.2f;
                        }
                    }
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
