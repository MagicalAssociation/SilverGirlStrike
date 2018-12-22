using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//エフェクト生成クラス、シングルトン
public class Effect {
    private static Effect myself;

    //検索用
    private Dictionary<string, GameObject> effectDictionary;
    //管理するオブジェクト（id管理用）
    List<GameObject> objectList;


    // Use this for initialization
    private Effect() {
        this.effectDictionary = new Dictionary<string, GameObject>();
        this.objectList = new List<GameObject>();
    }

    //生成メソッド
    static public Effect Get()
    {
        if(myself == null)
        {
            myself = new Effect();
        }
        return myself;
    }

    //エフェクトを追加
    public void AddEffect(string name, GameObject effect)
    {
        if (effectDictionary.ContainsKey(name))
        {
            // すでに登録済みなのでいったん消す
            effectDictionary.Remove(name);
        }
        effectDictionary.Add(name, effect);
    }

    //IDにてエフェクトのGameObjectを取得
    public GameObject GetEffectGameObject(int id)
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
        GameObject eff = Object.Instantiate(this.effectDictionary[name], Vector3.zero, Quaternion.identity);
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
        Object.Destroy(this.objectList[id]);
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
