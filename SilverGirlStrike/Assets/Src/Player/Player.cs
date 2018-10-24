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
    public int timeCnt;
    public Vector2 axis;
    public float gravity;
    public int hp;
    public AnchorSelector anchor;
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
        this.jumpPower = 9.5f;
        this.timeCnt = 0;
        this.direction = Direction.RIGHT;
        this.state = State.NORMAL;
        this.preState = State.NORMAL;
        this.axis = new Vector2();
        this.hp = 10;
        this.anchor = GetComponent<AnchorSelector>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        Mode();
        if(this.state != this.preState)
        {
            this.preState = this.state;
            this.timeCnt = 0;
        }
        Move();
        this.timeCnt++;
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

                    axis.x = Input.GetAxis("RStickX") * 5.0f;
                    if (axis.x != 0.0f)
                    {
                        this.state = State.WALK;
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
                }
                break;
            case State.FALL:
                {
                    if(this.foot.CheckHit())
                    {
                        this.state = State.NORMAL;
                        this.nowJumpNumber = 0;
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
                    this.state = State.FALL;
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
                    mover.UpdateVelocity(0.0f, 0.0f, 0.3f, this.foot.CheckHit());
                }
                break;
            case State.JUMP:
                {
                    if(this.timeCnt == 0)
                    {
                        this.mover.Jump(this.jumpPower);
                    }
                    axis.x = Input.GetAxis("RStickX") * 5.0f;
                    mover.UpdateVelocity(axis.x, 0.0f,0.3f, this.foot.CheckHit());
                }
                break;
            case State.FALL:
                {
                    axis.x = Input.GetAxis("RStickX") * 5.0f;
                    mover.UpdateVelocity(axis.x, 0.0f, 0.3f, this.foot.CheckHit());
                }
                break;
            case State.WALK:
                {
                    mover.UpdateVelocity(axis.x, 0.0f, 0.3f, this.foot.CheckHit());
                }
                break;
            case State.WIRE:
                {
                    
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
