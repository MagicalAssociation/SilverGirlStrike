using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGame : CursorParam
{
    IEnumerator LoadStart()
    {
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("LoadScene");
    }
    public override void Decision()
    {
        Sound.PlaySE("clearSound2");
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
