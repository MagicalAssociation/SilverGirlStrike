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
public class SystemInput {
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
        ITEM_1,
        //! アイテム２
        ITEM_2,
        //! アイテム３
        ITEM_3,
        //! アイテム４
        ITEM_4,
        //! ポーズ
        PAUSE,
        //! Tag数
        TAG_NUM
    }
    /**
     * brief    入力対応stringと強制入力制限を行うclass
     */
    class InputData
    {
        //! InputManagerName
        string name;
        //! InputStop
        bool enableStop;
        /**
         * brief    constructor
         * param    string name 対応させたいInputManagerの使用する名前
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
         * param    bool flag trueにすると入力を強制的にfalseを返すようにする
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
        * param    string name
        */
        public void SetName(string name)
        {
            this.name = name;
        }
    }

    //! 入力データを入れておく箱
    InputData[] inputData = new InputData[11];
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
        this.inputData[(int)Tag.ITEM_1] = new InputData("U");
        this.inputData[(int)Tag.ITEM_2] = new InputData("L");
        this.inputData[(int)Tag.ITEM_3] = new InputData("R");
        this.inputData[(int)Tag.ITEM_4] = new InputData("D");
        this.inputData[(int)Tag.PAUSE] = new InputData("START");
        //this.inputData[(int)Tag.DECISION].SetName("B");
        //this.inputData[(int)Tag.CANCEL].SetName("A");
        //this.inputData[(int)Tag.ATTACK].SetName("X");
        //this.inputData[(int)Tag.JUMP].SetName("A");
        //this.inputData[(int)Tag.WIRE].SetName("Y");
        //this.inputData[(int)Tag.TRANS_ASSAULT].SetName("L1");
        //this.inputData[(int)Tag.ITEM_1].SetName("U");
        //this.inputData[(int)Tag.ITEM_2].SetName("L");
        //this.inputData[(int)Tag.ITEM_3].SetName("R");
        //this.inputData[(int)Tag.ITEM_4].SetName("D");
        //this.inputData[(int)Tag.PAUSE].SetName("START");
    }
    /**
     * brief    登録タグのON入力を取得
     * param    SystemInput.Tag tag 登録タグ
     * return   bool 入力ON状態
     */
    public bool On(SystemInput.Tag tag)
    {
        if(this.inputData[(int)tag].GetEnableStop())
        {
            return false;
        }
        return Input.GetButton(this.inputData[(int)tag].GetName());
    }
    /**
    * brief    登録タグのDOWN入力を取得
    * param    SystemInput.Tag tag 登録タグ
    * return   bool 入力DOWN状態
    */
    public bool Down(SystemInput.Tag tag)
    {
        if (this.inputData[(int)tag].GetEnableStop())
        {
            return false;
        }
        return Input.GetButtonDown(this.inputData[(int)tag].GetName());
    }
    /**
    * brief    登録タグのUP入力を取得
    * param    SystemInput.Tag tag 登録タグ
    * return   bool 入力UP状態
    */
    public bool Up(SystemInput.Tag tag)
    {
        if (this.inputData[(int)tag].GetEnableStop())
        {
            return false;
        }
        return Input.GetButtonUp(this.inputData[(int)tag].GetName());
    }
    /**
     * brief    強制入力制御設定
     * param    SystemInput.Tag tag 変更するタグ
     * param    bool flag 入力を止める場合true
     */
     public void SetEnableStop(SystemInput.Tag tag,bool flag)
    {
        this.inputData[(int)tag].SetEnableStop(flag);
    }
    /**
    * brief    強制入力制御を取得
    * param    SystemInput.Tag tag 取得したいタグ
    * return   bool 入力制御 
    */
    public bool GetEnableStop(SystemInput.Tag tag)
    {
        return this.inputData[(int)tag].GetEnableStop();
    }
}
