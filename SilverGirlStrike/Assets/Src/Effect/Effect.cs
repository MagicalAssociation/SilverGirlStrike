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

    //検索用
    private Dictionary<string, GameObject> effectDictionary;
    //管理するオブジェクト（id管理用）
    List<GameObject> objectList;

    // Use this for initialization
    void Start () {

        this.effectDictionary = new Dictionary<string, GameObject>();
        this.objectList = new List<GameObject>();

        //touroku
        foreach(var i in effectlist)
        {
            effectDictionary.Add(i.key, i.eff);
        }
    }

    // Update is called once per frame
    void Update () {
    }

    //IDにてエフェクトのGameObjectを取得
    public GameObject GetEffectameObject(int id)
    {
        //範囲外はnull
        if(id >= this.objectList.Count && id < 0)
        {
            return null;
        }

        return this.objectList[id].gameObject;
    }

    //名前を指定してエフェクトを生成する、IDを返す
    public int CreateEffect(string name, Vector3 pos, Quaternion rot, Vector3 scale)           //検索する名前に一致するgameObjectを生成する
    {
        GameObject eff = Instantiate(this.effectDictionary[name], Vector3.zero, Quaternion.identity);
        eff.transform.position = pos;
        eff.transform.rotation = rot;
        eff.transform.localScale = scale;

        int id = GetUseableID();
        this.objectList[id] = eff;
        return id;
    }

    //指定IDのエフェクトを削除
    public void DeleteEffect(int id)
    {
        //範囲外は何もしない
        if (id >= this.objectList.Count && id < 0)
        {
            return;
        }
        //無効なidも何もしない
        if(this.objectList[id] == null)
        {
            return;
        }


        //削除し、nullを代入
        Destroy(this.objectList[id]);
        this.objectList[id] = null;
    }


    //現在の空いているIDを検索、空きがない場合は-1を返す
    int GetUseableID()
    {
        int count = this.objectList.Count;
        int index = -1;
        for (int i = 0; i < count; ++i)
        {
            if (this.objectList[i] == null)
            {
                index = i;
                break;
            }
        }

        //空きがないので追加
        if (index == -1)
        {
            this.objectList.Add(null);
            index = this.objectList.Count - 1;
        }

        return index;
    }
}
