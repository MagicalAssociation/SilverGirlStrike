using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveGame : CursorParam {

    public Image image;
    public Text text;
    private void Start()
    {
    }
    public override void Decision()
    {
        CurrentData.GetDataInstance().GetData().filePath = GameData.GetSaveFilePath()[GetComponent<Select>().GetStageNumver()];
        GameData.Save(CurrentData.GetDataInstance().GetData());
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
