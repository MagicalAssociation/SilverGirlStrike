using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LoadGameStart : CursorParam
{
    public Image image;
    public Text text;
    private void Start()
    {
    }
    IEnumerator GameStart()
    {
        yield return new WaitForSeconds(0.0f);
        SceneManager.LoadScene("kariItemSelectScene");
    }
    public override void Decision()
    {
        if (GetComponent<Select>().GetData() != null)
        {
            M_System.currentData.SetData(GetComponent<Select>().GetData());
            StartCoroutine(GameStart());
        }
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
