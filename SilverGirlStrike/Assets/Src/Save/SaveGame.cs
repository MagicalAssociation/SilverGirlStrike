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
            for (int i = 0; i < 2; ++i)
            {
                Save.DataParameter data = new Save.DataParameter();
                data.filePath = GameData.GetSaveFilePath()[i];
                data.gold = 200 * i;
                data.itemData.Add(new Save.DataParameter.ItemData(0, 1));
                data.itemData.Add(new Save.DataParameter.ItemData(1, 3));
                data.itemData.Add(new Save.DataParameter.ItemData(3, 10));
                GameData.Save(data);
            }
        }
    }
}
