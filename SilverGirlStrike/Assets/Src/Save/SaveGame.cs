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
        if (GetComponent<Select>().GetData() != null)
        {
            //仮処理
            //save2にお金200にして保存する
            Save.DataParameter data = new Save.DataParameter();
            data.filePath = GameData.GetSaveFilePath()[1];
            data.gold = 200;
            GameData.Save(data);
        }
    }
}
