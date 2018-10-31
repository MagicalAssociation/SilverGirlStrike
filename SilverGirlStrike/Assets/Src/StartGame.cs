using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour {
        // Use this for initialization
        void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (M_System.input.Down(SystemInput.Tag.DECISION))
        {
            //　スタートボタンを押したら実行する
            SceneManager.LoadScene("GameScene");
        }
    }
}
