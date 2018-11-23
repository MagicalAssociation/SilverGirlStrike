using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    //メンバ変数
    public string filePath;
    Dictionary<string, int> termFunctions;
    TextEvent.EventUnit manager;
    // Use this for initialization
    void Start()
    {
        this.termFunctions = new Dictionary<string, int>();
        this.termFunctions["termNo1"] = 1;
        test3();
    }

    // Update is called once per frame
    void Update()
    {
        this.manager.Update();
    }

    void test3()
    {
        //専用クラスを用いた実際の判定処理のテスト
        TextEvent.EventPerser eventReader = new TextEvent.EventPerser(filePath);
        var text = eventReader.GetEventText();
        this.manager = new TextEvent.EventUnit(text);

    }


    void test2()
    {
        //読み込みからの、条件判定テスト(条件文は仮)
        TextEvent.EventPerser eventReader = new TextEvent.EventPerser(filePath);
        var text = eventReader.GetEventText();

        bool result = false;
        foreach (var termText in text.termText)
        {
            if (this.termFunctions[termText.eventName] == 1)
            {
                if (int.Parse(termText.args[0]) == 10)
                {
                    if (int.Parse(termText.args[1]) == 3)
                    {
                        if (int.Parse(termText.args[2]) == 6)
                        {
                            result = true;
                        }
                    }
                }

            }
        }

        if (result)
        {
            foreach (var v in text.actionText)
            {
                Debug.Log(v.eventName);
                foreach (var i in v.args)
                {
                    Debug.Log(i);
                }
            }
        }

    }

    void test1()
    {
        //読み込みからの全表示テスト
        TextEvent.EventPerser eventReader = new TextEvent.EventPerser(filePath);
        var text = eventReader.GetEventText();

        foreach (var v in text.actionText)
        {
            Debug.Log(v.eventName);
            foreach (var i in v.args)
            {
                Debug.Log(i);
            }
        }
        foreach (var v in text.termText)
        {
            Debug.Log(v.eventName);
            foreach (var i in v.args)
            {
                Debug.Log(i);
            }
        }
        foreach (var v in text.labelData)
        {
            Debug.Log(v.Key);
            Debug.Log(v.Value);
        }
    }


}
