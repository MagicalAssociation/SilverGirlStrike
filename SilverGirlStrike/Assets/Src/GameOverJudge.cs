using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameOverJudge : MonoBehaviour {
    bool missFrag;
    //カメラ止める用
    public Camera camera;

    public Image fade;

    private void Start()
    {
        this.missFrag = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //ゴール領域にプレイヤーが入り込んだら
        if (collision.tag == "Player" && !this.missFrag)
        {
            Sound.StopBGM();
            this.missFrag = true;
            StartCoroutine(EndGame());
        }
    }

    IEnumerator EndGame()
    {
        this.camera.gameObject.GetComponent<ObjectChaser>().chaseTarget = null;

        float fadeAlpha = 0.0f;
        yield return new WaitForSeconds(2.0f);
        for (int i = 0; i < 60; ++i)
        {
            fadeAlpha += 1.0f / 60.0f;
            fade.color = new Color(0.0f, 0.0f, 0.0f, fadeAlpha);
            yield return null;
        }
        fade.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        yield return null;

        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("TitleScene");
    }
}
