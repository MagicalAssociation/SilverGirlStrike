using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : CursorParam
{
    public SGS.CursorColor cursorColor;
    IEnumerator Game()
    {
        yield return new WaitForSeconds(0.0f);
        SceneManager.LoadScene("GameScene");
    }
    public override void Decision()
    {
        Sound.PlaySE("clearSound2");
        StartCoroutine(Game());
    }

    public override void Enter()
    {
        throw new System.NotImplementedException();
    }

    public override void Exit()
    {
        throw new System.NotImplementedException();
    }
}
