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
    public enum Tag //: int
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
        //! down判定を入れておく変数
        bool input_Down;
        //! up判定を入れておく変数
        bool input_Up;
        //! on判定を入れておく変数
        bool input_On;
        //! 強制入力判定
        bool enableForced;
        //! 軸の角度
        float axis;
        //! 強制判定の角度
        float axisForced;
        //! 角度値反転
        float axisFlip;
        //! 判定フラグ
        bool axisFlag;
        /**
         * brief    constructor
         * param[in]    string name 対応させたいInputManagerの使用する名前
         */
        public InputData(string name,bool flag = false,bool flip = false)
        {
            this.name = name;
            this.Reset();
            if(flip == true)
            {
                axisFlip = -1.0f;
            }
            else
            {
                axisFlip = 1.0f;
            }
            axisFlag = flag;
        }
        /**
         * brief    constructor
         */
        public InputData()
        {
            this.name = "";
            this.Reset();
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
        /**
         * brief    スティック系入力状況の更新
         * param[in]    float power 判定させる強さ[0.1~1.0]で指定する
         * ※注意※　スティック以外はUnityのInputに機能があるので省いています。
         * ここで更新するのはスティック関係のみです！
         */
        public void AxisUpdate(float power)
        {
            //this.axis = Input.GetAxis(this.name) != 0.0f ? Input.GetAxis(this.name) : this.axis;
            //this.axis = Input.GetAxis(this.name) != 0.0f ? Input.GetAxis(this.name) : 0.0f;
            this.axis = Input.GetAxis(this.name) * this.axisFlip != 0.0f ? Input.GetAxis(this.name) * this.axisFlip : 0.0f;
            if(this.GetForced())
            {
                this.axis = this.GetAxisForced();
            }
            bool flag = this.axis >= power;
            this.input_Down = !this.input_On && flag;
            this.input_Up = this.input_On && !flag;
            this.input_On = flag;
        }
        /**
         * brief    ボタン系入力状況の更新
         */
         public void ButtonUpdate()
        {
            bool flag = Input.GetButton(this.name) || this.GetForced();
            this.axis = flag ? 1.0f : 0.0f;
        }
        /**
         * brief    on判定取得
         * return   bool true InputON
         */
         public bool GetOn()
        {
            return (this.axisFlag) ? this.input_On : Input.GetButton(this.name) || this.GetForced();
            //return this.input_On || Input.GetButton(this.name) || this.GetForced();
        }
        /**
         * brief    up判定取得
         * return   bool true InputUP
         */
        public bool GetUp()
        {
            return (this.axisFlag) ? this.input_Up : Input.GetButtonUp(this.name) || this.GetForced();
            //return this.input_Up || Input.GetButtonUp(this.name) || this.GetForced();
        }
        /**
         * brief    down判定取得
         * return   bool true InputDOWN
         */
        public bool GetDown()
        {
            return (this.axisFlag) ? this.input_Down : Input.GetButtonDown(this.name) || this.GetForced();
            //return this.input_Down || Input.GetButtonDown(this.name) || this.GetForced();
        }
        /**
         * brief    倒してる角度を取得
         * ボタン系は0か1を返す
         */
         public float GetAxis()
        {
            return this.axis;
        }
        /**
         * brief    強制判定設定
         * param[in]    bool flag 判定設定
         */
         public void SetForced(bool flag)
        {
            this.enableForced = flag;
        }
        /**
         * brief    強制判定取得
         * return bool 判定
         */
         public bool GetForced()
        {
            return this.enableForced;
        }
        /**
         * brief    強制判定の時に得るAxis値の設定
         * param[in]    float axis 軸角度
         */
         public void SetAxisForced(float axis)
        {
            this.axisForced = axis;
        }
        /**
         * brief    強制判定Axis値を取得する
         * return float Axis値
         */
         public float GetAxisForced()
        {
            return this.axisForced;
        }
        /**
         * brief    入力状況のリセット
         */
        private void Reset()
        {
            this.input_Down = false;
            this.input_Up = false;
            this.input_On = false;
            this.enableStop = false;
            this.enableForced = false;
            this.axisForced = 0.0f;
            this.axis = 0.0f;
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
        this.inputData[(int)Tag.WIRE] = new InputData("R1");
        this.inputData[(int)Tag.TRANS_ASSAULT] = new InputData("L1");
        this.inputData[(int)Tag.ITEM_L] = new InputData("L");
        this.inputData[(int)Tag.ITEM_R] = new InputData("R");
        this.inputData[(int)Tag.ITEM_D] = new InputData("D");
        this.inputData[(int)Tag.ITEM_U] = new InputData("U");
        this.inputData[(int)Tag.PAUSE] = new InputData("START");
        //1
        this.inputData[(int)Tag.LSTICK_DOWN] = new InputData("RStickY",true);
        //-1
        this.inputData[(int)Tag.LSTICK_UP] = new InputData("RStickY", true,true);
        //1
        this.inputData[(int)Tag.LSTICK_RIGHT] = new InputData("RStickX",true);
        //-1
        this.inputData[(int)Tag.LSTICK_LEFT] = new InputData("RStickX", true,true);
        this.axis_Power = 1.0f;
    }
    /**
     * brief    更新処理
     * 入力状況の更新
     * M_System以外で呼ばないこと！
     */
     public void Update()
    {
        for(int i = 0;i < (int)Tag.TAG_NUM;++i)
        {

            this.inputData[i].ButtonUpdate();
            switch ((Tag)i)
            {
                case Tag.ITEM_D:
                case Tag.ITEM_U:
                case Tag.ITEM_L:
                case Tag.ITEM_R:
                case Tag.LSTICK_DOWN:
                case Tag.LSTICK_UP:
                case Tag.LSTICK_LEFT:
                case Tag.LSTICK_RIGHT:
                    this.inputData[i].AxisUpdate(this.axis_Power);
                    break;
                default:
                    break;
            }
        }
    }
    /**
     * brief    登録タグのON入力を取得
     * param[in]    SystemInput.Tag tag 登録タグ
     * return   bool 入力ON状態
     */
    public bool On(SystemInput.Tag tag)
    {
        //return !this.inputData[(int)tag].GetEnableStop() && (Input.GetButton(this.inputData[(int)tag].GetName()) || this.inputData[(int)tag].GetOn());
        return !this.inputData[(int)tag].GetEnableStop() && this.inputData[(int)tag].GetOn();
    }
    /**
    * brief    登録タグのDOWN入力を取得
    * param[in]    SystemInput.Tag tag 登録タグ
    * return   bool 入力DOWN状態
    */
    public bool Down(SystemInput.Tag tag)
    {
        //return !this.inputData[(int)tag].GetEnableStop() && (Input.GetButtonDown(this.inputData[(int)tag].GetName()) || this.inputData[(int)tag].GetDown());
        return !this.inputData[(int)tag].GetEnableStop() && this.inputData[(int)tag].GetDown();
    }
    /**
    * brief    登録タグのUP入力を取得
    * param[in]    SystemInput.Tag tag 登録タグ
    * return   bool 入力UP状態
    */
    public bool Up(SystemInput.Tag tag)
    {
        //return !this.inputData[(int)tag].GetEnableStop() && (Input.GetButtonUp(this.inputData[(int)tag].GetName()) || this.inputData[(int)tag].GetUp());
        return !this.inputData[(int)tag].GetEnableStop() && this.inputData[(int)tag].GetUp();
    }
    /**
     * brief    登録タグの軸角度を取得
     * param[in]    SystemInput.Tag tag 登録タグ
     * reeturn  float 現状角度
     */
     public float Axis(SystemInput.Tag tag)
    {
        return this.inputData[(int)tag].GetEnableStop() ? 0.0f : this.inputData[(int)tag].GetAxis();
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
    private bool GetAxisOn(SystemInput.Tag tag)
    {
        return Input.GetAxis(this.inputData[(int)tag].GetName()) >= this.axis_Power ? true : false;
    }
    /**
     * brief    スティックの判定させる押し倒しの強さを指定する
     * param[in]    float power 判定させる強さ[0.1~1.0]で指定する
     */
    public void SetAxisPower(float power)
    {
        this.axis_Power = power;
    }
    /**
     * brief    スティックの判定させる押し倒しの強さを取得する
     * return   判定の強さ
     */
     public float GetAxisPower()
    {
        return this.axis_Power;
    }
    /**
     * brief    強制判定設定
     * param[in]    SystemInput.Tag tag 指定タグ
     * param[in]    bool flag 強制判定する場合true
     */
     public void SetForced(SystemInput.Tag tag,bool flag)
    {
        this.inputData[(int)tag].SetForced(flag);
    }
    /**
     * brief    強制判定取得
     * param[in]    SystemInput.Tag tag 指定タグ
     * return   bool 強制判定設定
     */
    public bool GetForced(SystemInput.Tag tag)
    {
        return this.inputData[(int)tag].GetForced();
    }
    /**
     * brief    強制判定時Axisの値の設定
     * param[in]    SystemInput.Tag tag 指定タグ
     * param[in]    float axis 軸値
     */
     public void SetAxisForced(SystemInput.Tag tag,float axis)
    {
        this.inputData[(int)tag].SetAxisForced(axis);
    }
    /**
     * brief    強制判定時Axisの値の取得
     * param[in]    SystemInput.Tag tag 指定タグ
     * return   float Axis値
     */
     public float GetAxisForced(SystemInput.Tag tag)
    {
        return this.inputData[(int)tag].GetAxisForced();
    }
}