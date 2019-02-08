using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGame : CursorParam
{
    IEnumerator GameStart()
    {
        yield return new WaitForSeconds(0.0f);
        SceneManager.LoadScene("GameScene");
    }
    public override void Decision()
    {
        StartCoroutine(GameStart());
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
