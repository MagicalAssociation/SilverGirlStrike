using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//変更履歴
//2018/10/26 板倉（作成）


//ボスキャラ、ただの岩に魔法によって知能と生命を与えられた姿
public class rockGolem : MonoBehaviour {
    //ステート
    struct StateParam
    {
        //過去と現在を参照できる
        public State currentState;
        public State prevState;
        public int timeCnt;

        public StateParam(State firstState)
        {
            //初期値
            this.currentState = firstState;
            this.prevState = State.None;
            timeCnt = 0;
        }

        public void ChangeState(State nextState)
        {
            this.prevState = this.currentState;
            this.currentState = nextState;
            timeCnt = 0;
        }

    }

    //ボスのステート
    enum State {
        None,
        Normal,
        Move,
        Tackle,
        Shooting,
        Press,
        Dead,
    }


    ////////
    //メンバー関数
    ////
    CharacterMover mover;
    StateParam state;

    float zyouge;


    //ステートが始まった瞬間の処理
    void StartState(State current, State prev)
    {
        //ステートごとの処理
        switch (current)
        {
            case State.Normal:
                break;
            case State.Move:
                break;
            case State.Tackle:
                break;
            case State.Shooting:
                break;
            case State.Press:
                break;
            case State.Dead:
                break;
            default:
                //Noneとかは何もしない
                break;
        }
    }

    //ステートが終わる瞬間の処理
    void EndState(State current, State prev, State next)
    {
        //ステートごとの処理
        switch (current)
        {
            case State.Normal:
                break;
            case State.Move:
                break;
            case State.Tackle:
                break;
            case State.Shooting:
                break;
            case State.Press:
                break;
            case State.Dead:
                break;
            default:
                //Noneとかは何もしない
                break;
        }
    }
    //ステートを変える処理
    void ChangeState()
    {
        //次のステートを設定する
        State nextState = State.None;

        //ステートごとの処理
        switch (this.state.currentState)
        {
            case State.Normal:
                break;
            case State.Move:
                break;
            case State.Tackle:
                break;
            case State.Shooting:
                break;
            case State.Press:
                break;
            case State.Dead:
                break;
            default:
                //Noneとかは何もしない
                break;
        }

        //設定があった場合はステート変更
        if (nextState != State.None)
        {
            EndState(this.state.currentState, this.state.prevState, nextState);
            this.state.ChangeState(nextState);
            StartState(this.state.currentState, this.state.prevState);
        }


    }

    //ステートそのものの行動処理
    void ActionState()
    {
        //ステートごとの処理
        switch (this.state.currentState)
        {
            case State.Normal:
                this.mover.UpdateVelocity(Mathf.Sin((this.state.timeCnt * 0.8f) * Mathf.Deg2Rad) * 0.1f, Mathf.Sin((this.state.timeCnt * 2.0f) * Mathf.Deg2Rad), 0.0f, false);

                break;
            case State.Move:
                break;
            case State.Tackle:
                break;
            case State.Shooting:
                break;
            case State.Press:
                break;
            case State.Dead:
                break;
            default:
                //Noneとかは何もしない
                break;
        }
    }


    // Use this for initialization
    void Start () {
        this.mover = GetComponent<CharacterMover>();
        this.mover.SetActiveGravity(false);
        this.state = new StateParam(State.Normal);
        this.zyouge = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
        ChangeState();
        ActionState();
        this.state.timeCnt += 1;
    }
}
