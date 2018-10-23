using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public CharacterMover mover;
    public foot foot;
    public float jumpPower;
    public int maxJumpNumber;
    public int nowJumpNumber;
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
    public State state;
	// Use this for initialization
	void Start () {
        this.mover = GetComponent<CharacterMover>();
        this.maxJumpNumber = 1;
        this.nowJumpNumber = 0;
        this.jumpPower = 9.5f;
	}
	
	// Update is called once per frame
	void Update () {
        
        if(M_System.input.Down(SystemInput.Tag.JUMP) && this.maxJumpNumber > this.nowJumpNumber)
        {
            this.mover.Jump(this.jumpPower);
            this.nowJumpNumber++;
        }
        float axis = Input.GetAxis("RStickX") * 5.0f;

        //横移動力、重力、接地フラグを渡す
        mover.UpdateVelocity(axis, 0.3f, this.foot.isFoot);
    }

    void Mode()
    {
        switch(this.state)
        {
            case State.NORMAL:
                {
                    if (!this.foot.isFoot)
                    {
                        this.state = State.FALL;
                    }
                    if(Input.GetAxis("RStickX") != 0.0f)
                    {
                        this.state = State.WALK;
                    }
                }
                break;
            case State.WALK:
                {

                }
                break;
            default:
                break;
        }
    }
}
