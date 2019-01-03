using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGame : CursorParam
{
    IEnumerator GameStart()
    {
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("GameScene");
    }
    public override void Decision()
    {
        Sound.PlaySE("clearSound2");
        StartCoroutine(GameStart());
    }
}
