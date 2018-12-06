using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
    //　経過時間
    private float nowTime;

    void Start()
    {
        nowTime = 0f;
    }

    void Update()
    {

        //　Time.deltaTimeを足す
        nowTime += Time.deltaTime;

        //　経過時間を表示
        //Debug.Log(Time.deltaTime);

        //　10秒を超えたら0に戻す
        if (nowTime >= 10f)
        {
            nowTime = 0f;
        }
    }
}
