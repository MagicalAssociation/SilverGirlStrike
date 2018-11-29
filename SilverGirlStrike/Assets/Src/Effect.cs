using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Effect : MonoBehaviour {

    //Dictionary<string, GameObject> list;          //C#のMapの役割

    [System.Serializable]
    public struct EffectList
    {
        public string key;
        public GameObject eff;
    }
    public EffectList[] effectlist;     //effectを格納するリスト
    public Vector2 pos;     //生成する座標を指定
    public int deletetime;  //エフェクトが消えるまでの時間

    private List<GameObject> effList = new List<GameObject>();

    int effectSize;         //effectListのsizeを格納

    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void Update () {
    }

    //名前が一致するゲームオブジェクトがあればreturn、なければnull
    public GameObject GetEffect(string name)        //検索する名前に一致するgameObjectをreturnする
    {
        this.effectSize = effectlist.Length;    //長さを取得
        for (int i = 0; i < this.effectSize; ++i)
        {
            if(effectlist[i].key==name)
            {
                return effectlist[i].eff;       //見つかったのでgameObjectを返す
            }
        }
        //デバッグ用
        Debug.Log(name + "error");              //見つからなかったのでnullを返す
        return null;
    }

    //名前を指定してエフェクトを生成する
    public void CreateEffect(string name)           //検索する名前に一致するgameObjectを生成する
    {
        this.effectSize = effectlist.Length;    //長さを取得
        //検索した名前のエフェクトがあるかどうか調べて、あれば生成
        for (int i = 0; i < this.effectSize; ++i)
        {
            if (effectlist[i].key == name)
            {
                GameObject eff = Instantiate(effectlist[i].eff, this.pos, Quaternion.identity);
                effList.Add(eff);
            }
        }
    }

    //名前を指定して生成したエフェクトを削除する(一括削除)
    public void DeleteEffect(string name)
    {
        this.effectSize = effectlist.Length;    //長さを取得
        for (int i = 0; i < this.effectSize; ++i)
        {
            if (effectlist[i].key == name)
            {
                //現存するエフェクトを問答無用で削除
                for (int j = 0; j < effList.Count; ++j)
                {
                    Destroy(effList[j]);
                }
            }
        }
    }
}
