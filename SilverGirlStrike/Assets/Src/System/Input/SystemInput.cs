using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * file     SystemInput.cs
 * brief    ゲーム用にInputシステムを梱包したもの
 * author   Shou Kaneko
 * date     2018/10/05
 * Tagで指定して入力状況を取得できる
*/
public class SystemInput
{
    /**
     * enum Tag
     * 取得したいTag名
     * 入力状態の取得時に引数として使用する
     */
    public enum Tag : int
    {
        //! 決定
        DECISION,
        //! キャンセル
        CANCEL,
        //! 攻撃
        ATTACK,
        //! ジャンプ
        JUMP,
        //! ワイヤー
        WIRE,
        //! トランスアサルト
        TRANS_ASSAULT,
        //! アイテム１
        ITEM_L,
        //! アイテム２
        ITEM_R,
        //! アイテム３
        ITEM_D,
        //! アイテム４
        ITEM_U,
        //! ポーズ
        PAUSE,
        //! 左スティック上
        LSTICK_UP,
        //! 左スティック下
        LSTICK_DOWN,
        //! 左スティック右
        LSTICK_RIGHT,
        //! 左スティック左
        LSTICK_LEFT,
        //! Tag数
        TAG_NUM
    }
    /**
     * brief    入力対応stringと強制入力制限を行うclass
     */
    private class InputData
    {
        //! InputManagerName
        string name;
        //! InputStop
        bool enableStop;
        /**
         * brief    constructor
         * param[in]    string name 対応させたいInputManagerの使用する名前
         */
        public InputData(string name)
        {
            this.name = name;
            this.enableStop = false;
        }
        /**
         * brief    constructor
         */
        public InputData()
        {
            this.name = "";
            this.enableStop = false;
        }
        /**
         * brief    入力停止設定
         * param[in]    bool flag trueにすると入力を強制的にfalseを返すようにする
         */
        public void SetEnableStop(bool flag)
        {
            this.enableStop = flag;
        }
        /**
         * brief    入力停止状態を取得
         * return   bool 入力停止設定
         */
        public bool GetEnableStop()
        {
            return this.enableStop;
        }
        /**
         * brief    登録されているInputNameを取得する
         * return   string InputName
         */
        public string GetName()
        {
            return this.name;
        }
        /**
        * brief    Nameを登録する
        * param[in]    string name
        */
        public void SetName(string name)
        {
            this.name = name;
        }
    }

    //! 入力データを入れておく箱
    private InputData[] inputData = new InputData[(int)Tag.TAG_NUM];
    //! Axisの反応角度の指定
    float axis_Power;
    /**
     * constructor
     */
    public SystemInput()
    {
        this.inputData[(int)Tag.DECISION] = new InputData("B");
        this.inputData[(int)Tag.CANCEL] = new InputData("A");
        this.inputData[(int)Tag.ATTACK] = new InputData("X");
        this.inputData[(int)Tag.JUMP] = new InputData("A");
        this.inputData[(int)Tag.WIRE] = new InputData("Y");
        this.inputData[(int)Tag.TRANS_ASSAULT] = new InputData("L1");
        this.inputData[(int)Tag.ITEM_L] = new InputData("L");
        this.inputData[(int)Tag.ITEM_R] = new InputData("R");
        this.inputData[(int)Tag.ITEM_D] = new InputData("D");
        this.inputData[(int)Tag.ITEM_U] = new InputData("U");
        this.inputData[(int)Tag.PAUSE] = new InputData("START");
        this.inputData[(int)Tag.LSTICK_DOWN] = new InputData("RStickY");
        this.inputData[(int)Tag.LSTICK_UP] = new InputData("RStickY");
        this.inputData[(int)Tag.LSTICK_RIGHT] = new InputData("RStickX");
        this.inputData[(int)Tag.LSTICK_LEFT] = new InputData("RStickX");
        this.axis_Power = 1.0f;
    }
    /**
     * brief    登録タグのON入力を取得
     * param[in]    SystemInput.Tag tag 登録タグ
     * return   bool 入力ON状態
     */
    public bool On(SystemInput.Tag tag)
    {
        // アイテムの場合だけ判定を変える
        switch (tag)
        {
            case Tag.ITEM_D:
            case Tag.ITEM_U:
            case Tag.ITEM_L:
            case Tag.ITEM_R:
            case Tag.LSTICK_DOWN:
            case Tag.LSTICK_UP:
            case Tag.LSTICK_LEFT:
            case Tag.LSTICK_RIGHT:
                {
                    return !this.inputData[(int)tag].GetEnableStop() && Input.GetButton(this.inputData[(int)tag].GetName());
                }
        }

        return !this.inputData[(int)tag].GetEnableStop() && Input.GetButton(this.inputData[(int)tag].GetName());
    }
    /**
    * brief    登録タグのDOWN入力を取得
    * param[in]    SystemInput.Tag tag 登録タグ
    * return   bool 入力DOWN状態
    */
    public bool Down(SystemInput.Tag tag)
    {
        return !this.inputData[(int)tag].GetEnableStop() && Input.GetButtonDown(this.inputData[(int)tag].GetName());
    }
    /**
    * brief    登録タグのUP入力を取得
    * param[in]    SystemInput.Tag tag 登録タグ
    * return   bool 入力UP状態
    */
    public bool Up(SystemInput.Tag tag)
    {
        return !this.inputData[(int)tag].GetEnableStop() && Input.GetButtonUp(this.inputData[(int)tag].GetName());
    }
    /**
     * brief    強制入力制御設定
     * param[in]    SystemInput.Tag tag 変更するタグ
     * param[in]    bool flag 入力を止める場合true
     */
    public void SetEnableStop(SystemInput.Tag tag, bool flag)
    {
        this.inputData[(int)tag].SetEnableStop(flag);
    }
    /**
    * brief    強制入力制御を取得
    * param[in]     SystemInput.Tag tag 取得したいタグ
    * return   bool 入力制御 
    */
    public bool GetEnableStop(SystemInput.Tag tag)
    {
        return this.inputData[(int)tag].GetEnableStop();
    }
    /**
     * brief    スティックの入力値をboolにして返す
     * param[in]    SystemInput.Tag tag 取得したいタグ
     * return   bool 判定値
     */
     private bool GetAxis(SystemInput.Tag tag)
    {
        return Input.GetAxis(this.inputData[(int)tag].GetName()) >= this.axis_Power ? true : false;
    }
    /**
     * brief    スティックの
     */
}