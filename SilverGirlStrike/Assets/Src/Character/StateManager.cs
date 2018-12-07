using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * file     StateManager.cs
 * brief    State関係
 * author   Shou Kaneko
 * date     2018/11/14
*/

    //編集履歴
    //2018/11/21 板倉　：　やっぱりステートの無限ループを防止するようなチェックを導入



/**
 * brief     Stateを生成する際に継承する元の型
 */
public abstract class StateParameter
{
    //! 経過カウント
    private int timeCnt;
    /**
     *  constructor
     */
    public StateParameter()
    {
        //タイムリセット
        this.ResetTime();
    }
    /**
     *  brief   更新処理
     */
    public abstract void Update();
    /**
     * brief    開始時
     */
    public abstract void Enter(ref StateManager manager);
    /**
     * brief    終了時
     */ 
    public abstract void Exit(ref StateManager manager);
    /**
     * brief    変更処理
     * param[in] ref StateManager manager 管理クラスの情報
     * return bool trueで変更処理を行う
     * この処理内に条件を記述する
     */ 
    public abstract bool Transition(ref StateManager manager);
    /**
     * brief    経過カウントを取得
     * return int 経過カウント
     */ 
    public int GetTime()
    {
        return this.timeCnt;
    }
    /**
     * brief    経過カウントの上昇値を指定
     * param[in]    int cnt 上昇値
     */ 
    public void CountUp(int cnt)
    {
        this.timeCnt += cnt;
    }
    /**
     * brief    経過カウントを初期化
     */ 
    public void ResetTime()
    {
        this.timeCnt = 0;
    }
}

/**
 * brief    State管理class
 */ 
public class StateManager
{
    //! 登録データ
    Dictionary<int, StateParameter> pairs;
    //! NextStateNumber
    int nextState;
    //! PreStateNumber
    int preState;
    //! NowStateNumber
    int nowState;
    /**
     * constructor
     */
    public StateManager()
    {
        pairs = new Dictionary<int, StateParameter>();
        this.nextState = -1;
        this.preState = -1;
        this.nowState = -1;
    }
    /**
     * brief    Stateを登録する
     * param[in] int stateNum StateNumber
     * param[in] StatePatameter parameter StateData
     */ 
    public void SetParameter(int stateNum,StateParameter parameter)
    {
        pairs.Add(stateNum, parameter);
    }
    /**
     * brief    Stateを強制変更する
     * param[in] int stateNum StateNumber
     */ 
    public void ChengeState(int stateNum)
    {
        this.SetNextState(stateNum);
        this.Transition();
    }
    /**
     * brief    更新処理
     */ 
    public void Update()
    {
        var tmp = this;
        //変化のおおもとを記録
        int tmpPrevState = this.nowState;
        //変化が収まるか、元の場所に戻る一歩手前で遷移終了
        while (this.pairs[this.nowState].Transition(ref tmp))
        {
            //出戻りを防止
            if(tmpPrevState == this.nextState)
            {
                break;
            }
            this.Transition();
        }
        this.pairs[this.nowState].CountUp(1);
        this.pairs[this.nowState].Update();
    }
    /**
     * brief    State変更処理
     */ 
    void Transition()
    {
        var tmp = this;
        //初期ステートが-1であることへの対応
        //現在のステートが-1の場合NextをいれてEnterだけを呼ぶ
        if (this.nowState == -1)
        {
            this.nowState = this.nextState;
        }
        else
        {
            //現在の終了関数を呼んで、State値を移動する
            this.pairs[this.nowState].Exit(ref tmp);
            this.preState = this.nowState;
            this.nowState = this.nextState;
        }
        this.pairs[this.nowState].Enter(ref tmp);
    }
    /**
     * brief    次Stateを指定する
     */ 
    public void SetNextState(int stateNum)
    {
        this.nextState = stateNum;
    }
    /**
     * brief    preStateのNumber取得
     * return int PreStateNumber
     */ 
     public int GetPreStateNum()
    {
        return this.preState;
    }
    /**
     * brief    nowStateのNumber取得
     * return int NowStateNumber
     */
    public int GetNowStateNum()
    {
        return this.nowState;
    }
    /**
     * brief    NextStateのNumber取得
     * return int NextStateNumber
     */
    public int GetNextStateNum()
    {
        return this.nextState;
    }
}


/*↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓サンプルSTATE処理↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓*/
/**
//!State1!//
public class TestState : StateParameter
{
    //! 自分の元のオブジェクトの情報
    GameObject gameObject;
    /**
     * brief    constructor
     * param[in] ref GameObject gameObject 元オブジェクト情報
    /*
    public TestState(ref GameObject gameObject)
    {
        this.gameObject = gameObject;
    }
    /**
     * brief    更新処理
     /* 
    public override void Update() 
    {
        //経過カウントを１増やす
        this.TimeUp(1);
        //親のScaleをいじる
        this.gameObject.transform.localScale += new Vector3(1, 0, 0);
    }
    /**
     * brief    開始処理
     /* 
    public override void Enter()
    {
        //経過カウントをリセットする
        this.ResetTime();
    }
    /**
     * brief    終了処理
     /* 
    public override void Exit()
    {

    }
    /**
     * brief    変更処理
     * param[in] ref StateManager manager 管理classの情報
     /*
    public override bool Transition(ref StateManager manager)
    {
        //Attackが押させたら次のStateへ移行
        if (M_System.input.Down(SystemInput.Tag.ATTACK))
        {
            //次Stateを指定※現在は関数
            manager.nextState = 1;
            M_System.input.SetEnableStop(SystemInput.Tag.ATTACK, true);
            return true;
        }
        else
        {
            M_System.input.SetEnableStop(SystemInput.Tag.ATTACK, false);
        }
        return false;
    }
}
//!State2!//
public class TestState2 : StateParameter
{
    GameObject gameObject;
    public TestState2(ref GameObject gameObject)
    {
        this.gameObject = gameObject;
    }
    public override void Update()
    {
        this.TimeUp(1);
    }

    public override void Enter()
    {
        this.ResetTime();
    }
    public override void Exit()
    {

    }
    public override bool Transition(ref StateManager manager)
    {
        if (M_System.input.Down(SystemInput.Tag.ATTACK))
        {
            manager.nextState = 0;
            M_System.input.SetEnableStop(SystemInput.Tag.ATTACK, true);
            return true;
        }
        else
        {
            M_System.input.SetEnableStop(SystemInput.Tag.ATTACK, false);
        }
        return false;
    }
}
//!適用例!//
public class Object : MonoBehaviour {

    StateManager manager;
	// Use this for initialization
	void Start () {
        //this.gameObjectを直接渡せないため一度別の場所へ置く
        var a = this.gameObject;
        //Managerを生成
        this.manager = new StateManager();
        //ManagerにStateを登録する
        this.manager.SetParameter(0, new TestState(ref a));
        this.manager.SetParameter(1, new TestState2(ref a));
	}
	
	// Update is called once per frame
	void Update () {
        //更新処理
        this.manager.Update();
	}
}
*/
