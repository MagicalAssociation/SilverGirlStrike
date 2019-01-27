using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//編集履歴
//2018/11/01 板倉　作成


//クリア領域をチェックして、クリア判定を出すスクリプト
//例によって雑オブ雑な実装故に注意されたし

public class GameClearJudge : MonoBehaviour {
    bool clearFrag;

    float time;

    private void Start()
    {
        this.clearFrag = false;
        //計測開始
        this.time = 0.0f;
    }

    private void Update()
    {
    }
}
