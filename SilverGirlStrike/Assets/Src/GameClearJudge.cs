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

    public Text clearText;
    public Text clearTimeText;
    public Image fade;

    float time;

    private void Start()
    {
        this.clearFrag = false;
        //計測開始
        this.time = 0.0f;
        Sound.PlayBGM("bossBattle");
    }

    private void Update()
    {
        this.time += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //ゴール領域にプレイヤーが入り込んだら
        if (collision.tag == "Player" && !this.clearFrag)
        {
            Sound.StopBGM();
            Sound.PlaySE("clearSound");
            this.clearFrag = true;
            StartCoroutine(EndGame());
        }
    }


    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(2.0f);
        Sound.PlaySE("clearSound2");
        this.clearText.text = "StageClear!!!!";
        yield return new WaitForSeconds(0.5f);

        int minute = 0;
        int second = 0;

        minute = (int)this.time / 60;
        second = (int)this.time % 60;

        //クリア時間
        Sound.PlaySE("clearSound2");
        this.clearTimeText.text = "Time   " + minute.ToString("d2") + ":" + second.ToString("d2");


        float fadeAlpha = 0.0f;
        yield return new WaitForSeconds(2.0f);
        for(int i = 0; i < 60; ++i)
        {
            fadeAlpha += 1.0f / 60.0f;
            fade.color = new Color(0.0f, 0.0f, 0.0f, fadeAlpha);
            yield return null;
        }
        fade.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        yield return null;

        SceneManager.LoadScene("TitleScene");
    }
}
