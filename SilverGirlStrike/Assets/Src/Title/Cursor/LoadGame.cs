using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGame : CursorParam
{
    IEnumerator LoadStart()
    {
        yield return new WaitForSeconds(0.0f);
        SceneManager.LoadScene("LoadScene");
    }
    public override void Decision()
    {
        StartCoroutine(LoadStart());
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
