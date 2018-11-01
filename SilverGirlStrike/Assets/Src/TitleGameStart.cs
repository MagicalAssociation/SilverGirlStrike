using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleGameStart : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (M_System.input.Down(SystemInput.Tag.DECISION))
        {
            Sound.PlaySE("clearSound2");
            StartCoroutine(GameStart());
        }

	}
    IEnumerator GameStart()
    {
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("GameScene");
    }
}
