using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//編集履歴
//2018/11/16 板倉：作成

//フラグの書き込み、作成、削除などを行うクラス
//フラグは、整数型で表現される
public class FlagManager {
    Dictionary<string, int> flags;

    //新規追加（初期値0）
    public void Create(string name)
    {
        flags[name] = 0;
    }
    //代入
    public void SetValue(string name, int value)
    {
        CheckExist(name);
        flags[name] = value;
    }
    //加算
    public void AddValue(string name, int value)
    {
        CheckExist(name);
        flags[name] += value;
    }
    //掛け算
    public void MultipleValue(string name, int value)
    {
        CheckExist(name);
        flags[name] *= value;
    }
    //割り算
    public void DivideValue(string name, int value)
    {
        CheckExist(name);
        flags[name] /= value;
    }

    //valueと同じなら
    public bool Equal(string name, int value)
    {
        CheckExist(name);
        return flags[name] == value;
    }
    //valueより大きいなら
    public bool Greater(string name, int value)
    {
        CheckExist(name);
        return flags[name] > value;
    }
    //valueより小さいなら
    public bool Smaller(string name, int value)
    {
        CheckExist(name);
        return flags[name] < value;
    }

    //存在してなかったら例外を出す関数
    void CheckExist(string name)
    {
        if (!this.flags.ContainsKey(name))
        {
            throw new System.Exception("flag error: not exist flag name");
        }
    }

}
