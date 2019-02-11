using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameOverJudge : MonoBehaviour {
    bool missFrag;
    //カメラ止める用
    public Camera cameraObj;
    public CharacterManager manager;


    private void Start()
    {
        this.missFrag = false;

    }

    private void Update()
    {
        if(this.manager.GetCharacterData("Player") == null)
        {
            Sound.StopBGM();
            this.missFrag = true;
            StartCoroutine(EndGame());
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //領域にプレイヤーが入り込んだら
        if (collision.tag == "Player" && !this.missFrag)
        {
            Sound.StopBGM();
            this.missFrag = true;
            StartCoroutine(EndGame());
        }
    }

    IEnumerator EndGame()
    {
        this.cameraObj.gameObject.GetComponent<ObjectChaser>().chaseTarget = null;

        yield return new WaitForSeconds(2.0f);
        // 現在のScene名を取得する
        Scene loadScene = SceneManager.GetActiveScene();
        // Sceneの読み直し
        SceneManager.LoadScene(loadScene.name);
    }
}
