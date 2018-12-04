using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//ポーズ画面など、プレイヤー挙動に影響しない操作によるシーン遷移を管理（後々名前変える）

public class SceneSelector : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (M_System.input.Down(SystemInput.Tag.PAUSE)){
            SceneManager.LoadScene("PauseScene");
        }


	}
}
