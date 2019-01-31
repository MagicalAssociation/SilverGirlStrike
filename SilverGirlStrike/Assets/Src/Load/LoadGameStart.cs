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
    public override void Decision()
    {
        if (GetComponent<Select>().GetData() != null)
        {
            CurrentData.GetDataInstance().SetData(GetComponent<Select>().GetData());
            SceneManager.LoadScene("StageSelect");
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
