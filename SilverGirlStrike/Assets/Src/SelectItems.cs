using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectItems : MonoBehaviour
{
    //移動する項目の情報を格納する
    [System.Serializable]
    public struct Items
    {
        public string key;
        public int num;
        public GameObject gameobject;
        public Vector3 endposition;
        public Vector3 startposition;
        public int stopnum;
    }
    public Items[] itemlist;

    //移動してきた項目の停止位置指定
    [System.Serializable]
    public struct Positions
    {
        public Vector3 pos;
        public int num;
    }
    public Positions[] poslist;
    private bool canmove;       //現在移動できるかどうか
    private float time;

    private float moveCnt;      //移動にかかる秒数指定

    //移動のために必要な変数
    private float starttime;

    // Use this for initialization
    void Start()
    {
        this.moveCnt = 0;
        this.canmove = true;
        this.time = 0.5f;

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
    }

    //-------------------------------------------------------------------------------------------------------
    //終了位置を指定
    //-------------------------------------------------------------------------------------------------------
    private void SetEndpos()
    {
        //下が押されたら
        if (Input.GetKey(KeyCode.DownArrow))
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

                    //テスト実装
                    //itemlist[i].startposition = itemlist[i].gameobject.transform.position;      //現在の位置を取得
                    this.starttime = Time.timeSinceLevelLoad;                                   //スタート時間を取得

                    //デバッグ用
                    //Debug.Log("座標" + i + ":" + itemlist[i].endposition);
                }
            }
        }

        //上が押されたら
        if (Input.GetKey(KeyCode.UpArrow))
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
                        itemlist[i].startposition = poslist[poslist.Length-1].pos;      //下の外へ

                        this.canmove = false;
                    }
                    itemlist[i].endposition = poslist[itemlist[i].stopnum].pos;     //新しく入る入れ物の座標を取得

                    //テスト実装
                    //itemlist[i].startposition = itemlist[i].gameobject.transform.position;      //現在の位置を取得
                    this.starttime = Time.timeSinceLevelLoad;                                   //スタート時間を取得

                    //デバッグ用
                    //Debug.Log("座標" + i + ":" + itemlist[i].endposition);
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
            var diff = Time.timeSinceLevelLoad - starttime;     //経過時間カウント?

            if (diff > time)     //時間切れなら
            {
                itemlist[i].gameobject.transform.position = poslist[itemlist[i].stopnum].pos;
                this.canmove = true;
                //enabled = false;
            }

            var rate = diff / time;

            itemlist[i].gameobject.transform.position = Vector3.Lerp(itemlist[i].startposition, itemlist[i].endposition, rate);
        }
    }
    //-----------------------------------------------------------------------------
    //-----------------------------------------------------------------------------


    //-----------------------------------------------------------------------------
    //シーン遷移について
    //-----------------------------------------------------------------------------
    void SetNextScene()
    {
        for (int i = 0; i < itemlist.Length; ++i)
        {
            if (itemlist[i].stopnum == 2)           //中央で止まっていたら
            {
                switch (itemlist[i].key)
                {
                    case ("stage"):
                        //移動先のsceneを順次追加
                        break;
                    case ("stage1"):
                        break;
                    case ("stage2"):
                        break;
                    case ("stage3"):
                        break;

                        //順次追加
                }
            }
        }
    }
    //-----------------------------------------------------------------------------
}