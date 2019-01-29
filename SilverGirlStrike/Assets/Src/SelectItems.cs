using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectItems : CursorSystem
{
    //必要なリスト2つ-------------------------------------------------
    //移動する項目の情報を格納する
    [System.Serializable]
    public struct Items
    {
        public string key;              //名前を設定(Scene遷移時に使用)
        public string location;         //ステージの場所を格納
        public int num;                 //番号を設定(上から順に0~)
        public GameObject gameobject;   //場所を移動させるゲームオブジェクト指定
        public Vector3 endposition;     //移動停止位置を指定
        public Vector3 startposition;   //現在の位置を取得
        public int stopnum;             //停止位置を格納しているPositionsから停止位置を持ってくるために使用
    }
    public Items[] itemlist;

    //移動してきた項目の停止位置指定
    [System.Serializable]
    public struct Positions
    {
        public Vector3 pos;     //選択項目が停止する位置を格納
        public int num;         //番号を指定(上から順に0~)
    }
    public Positions[] poslist;
    //---------------------------------------------------------------

    private bool canmove;       //現在移動できるかどうか
    private float time;         //移動にかかる時間
    private float moveCnt;      //移動にかかる秒数指定

    //変更する色を指定する変数へ数値を格納
    float red, green, blue;
    //移動のために必要な変数
    private float starttime;    //移動を開始した時間を格納
    //選択可能な中央の位置を求める
    int center;

    //表示textの変更用
    public Text stageNameText;
    public Text locationText;
    public Text clearTimeText;

    // Use this for initialization
    void Start()
    {
        this.moveCnt = 0;
        this.canmove = true;
        this.time = 0.2f;
        //変更する色を指定(選択できるもの以外を暗く)
        this.red = 125.0f;
        this.green = 125.0f;
        this.blue = 125.0f;
        //選択可能な中央の位置を求める
        this.center = itemlist.Length / 2;

        //項目を入れ物の中に格納
        if (itemlist.Length < 5)
        {
            for (int i = 0; i < itemlist.Length; ++i)
            {
                this.itemlist[i].stopnum = i + 1;
            }
        }
        else
        {
            for (int i = 0; i < itemlist.Length; ++i)
            {
                this.itemlist[i].stopnum = poslist[i].num;
            }
        }

        //移動のために必要な初期化--------------------------
        for (int i = 0; i < itemlist.Length; ++i)
        {
            if (time <= 0)             //時間切れであれば
            {
                itemlist[i].gameobject.transform.position = poslist[itemlist[i].stopnum].pos;
                //enabled = false;
                this.canmove = true;
                return;
            }
            this.starttime = Time.timeSinceLevelLoad;                                   //スタート時間を取得
            itemlist[i].startposition = itemlist[i].gameobject.transform.position;      //現在の位置を取得
        }
        //--------------------------------------------------
    }

    // Update is called once per frame
    void Update()
    {
        this.SetEndpos();
        this.Move();
        this.ChangeColor();
        this.ChangeText();
    }

    //public override void SystemUpdate(CursorSystemManager manager)
    //{
    //    this.SetEndpos();
    //    this.Move();
    //    this.ChangeColor();
    //    this.ChangeText();
    //}


    //-------------------------------------------------------------------------------------------------------
    //終了位置を指定
    //-------------------------------------------------------------------------------------------------------
    private void SetEndpos()
    {
        //下が押されたら
        //if (Input.GetKey(KeyCode.DownArrow))          //デバッグ用(方向キーで動く)
        if (M_System.input.Down(SystemInput.Tag.LSTICK_DOWN))
        {
            if (this.canmove)
            {
                for (int i = 0; i < itemlist.Length; ++i)
                {
                    if (itemlist[i].stopnum != poslist.Length - 1)                                   //一番最後の入れ物でなければ
                    {
                        itemlist[i].stopnum++;                                      //次の入れ物へ移動
                        itemlist[i].startposition = itemlist[i].gameobject.transform.position;      //現在の位置を取得
                        this.canmove = false;
                    }
                    else                                                            //一番最後の入れ物であれば
                    {
                        if (itemlist.Length < 5)                                    //停止位置より選択項目が少ない場合
                        {
                            itemlist[i].stopnum = 1;                                    //空きを無くすため1番の入れ物へ
                        }
                        else
                        {
                            itemlist[i].stopnum = 0;
                        }

                        itemlist[i].startposition = poslist[0].pos;      //上の外へ
                        this.canmove = false;
                    }


                    itemlist[i].endposition = poslist[itemlist[i].stopnum].pos;     //新しく入る入れ物の座標を取得
                    this.starttime = Time.timeSinceLevelLoad;                                   //スタート時間を取得
                }
            }
        }

        //上が押されたら
        //if (Input.GetKey(KeyCode.UpArrow))        //デバッグ用(方向キーで動く)
        if (M_System.input.Down(SystemInput.Tag.LSTICK_UP))
        {
            if (this.canmove)
            {
                for (int i = 0; i < itemlist.Length; ++i)
                {
                    if (itemlist[i].stopnum != 0)
                    {
                        itemlist[i].stopnum--;                                      //次の入れ物へ移動
                        itemlist[i].startposition = itemlist[i].gameobject.transform.position;      //現在の位置を取得
                        this.canmove = false;
                    }
                    else
                    {
                        if (itemlist.Length < 5)                                    //停止位置より選択項目が少ない場合
                        {
                            itemlist[i].stopnum = poslist.Length - 2;                                    //空きを無くすため1番の入れ物へ
                        }
                        else
                        {
                            itemlist[i].stopnum = poslist.Length - 1;
                        }
                        itemlist[i].startposition = poslist[poslist.Length - 1].pos;      //下の外へ

                        this.canmove = false;
                    }
                    itemlist[i].endposition = poslist[itemlist[i].stopnum].pos;     //新しく入る入れ物の座標を取得

                    this.starttime = Time.timeSinceLevelLoad;                                   //スタート時間を取得
                }
            }
        }
    }
    //-----------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------

    //-----------------------------------------------------------------------------------------
    //移動処理
    //-----------------------------------------------------------------------------------------
    private void Move()
    {
        for (int i = 0; i < itemlist.Length; ++i)
        {
            var diff = Time.timeSinceLevelLoad - starttime;     //経過時間カウント

            if (diff > time)     //時間切れなら
            {
                itemlist[i].gameobject.transform.position = poslist[itemlist[i].stopnum].pos;
                this.canmove = true;
            }

            var rate = diff / time;

            itemlist[i].gameobject.transform.position = Vector3.Lerp(itemlist[i].startposition, itemlist[i].endposition, rate);
        }
    }
    //-----------------------------------------------------------------------------
    //-----------------------------------------------------------------------------


    //-----------------------------------------------------------------------------
    //シーン遷移について(未実装)
    //Kaneko 追記：決定時にItemSelectのゲーム開始Selectにkeyの値を渡すs
    //-----------------------------------------------------------------------------
    void SetNextScene()
    {
        for (int i = 0; i < itemlist.Length; ++i)
        {
            if (itemlist[i].stopnum == center)           //中央で止まっていたら
            {
                switch (itemlist[i].key)
                {
                    case ("stage"):
                        //sceneを移動する処理を実装(未実装)
                        //SceneManager.LoadScene("GameScene");
                        break;
                    case ("stage1"):
                        //sceneを移動する処理を実装(未実装)
                        break;
                    case ("stage2"):
                        //sceneを移動する処理を実装(未実装)
                        break;
                    case ("stage3"):
                        //sceneを移動する処理を実装(未実装)
                        break;

                        //順次追加
                }
            }
        }
    }

    //-----------------------------------------------------------------------------
    //選択可能なアイコン以外暗くする
    void ChangeColor()
    {
        for (int i = 0; i < itemlist.Length; ++i)
        {
            if (itemlist[i].stopnum == center)          //選択できる項目なら
            {
                //通常色に
                itemlist[i].gameobject.GetComponent<SpriteRenderer>().color = new Color(255.0f, 255.0f, 255.0f);
            }
            else                               //選択できない項目なら
            {
                //少し暗くする
                itemlist[i].gameobject.GetComponent<SpriteRenderer>().color = new Color(red / 255.0f, green / 255.0f, blue / 255.0f);
            }
        }
    }
    //-------------------------------------------------------------------
    //textの変更(現在選択している項目の詳細を表示する)
    void ChangeText()
    {
        for (int i = 0; i < itemlist.Length; ++i)
        {
            if (itemlist[i].stopnum == center)
            {
                this.stageNameText.text = itemlist[i].key;
                this.locationText.text = itemlist[i].location;
                //クリアタイムのデータを取ってこられるように追加予定
                //CurrentData.CreateData();
                //this.clearTimeText.text = CurrentData.GetDataInstance().GetData().stageClearTime[i].ToString();
            }
        }
    }
    //--------------------------------------------------------------------
}