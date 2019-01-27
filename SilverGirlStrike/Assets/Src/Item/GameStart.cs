using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameStart : CursorParam
{
    public SGS.CursorColor cursorColor;
    public Image image;
    public ItemSetSelectManager manager;
    public override void Decision()
    {
        //Itemデータを相手に渡す
        CurrentData.SetItemData((int)CurrentData.ItemDirection.UP, manager.parameter.up.GetItem());
        CurrentData.SetItemData((int)CurrentData.ItemDirection.DOWN, manager.parameter.down.GetItem());
        CurrentData.SetItemData((int)CurrentData.ItemDirection.LEFT, manager.parameter.left.GetItem());
        CurrentData.SetItemData((int)CurrentData.ItemDirection.RIGHT, manager.parameter.right.GetItem());
        Sound.PlaySE("clearSound2");
        SceneManager.LoadScene("GameScene");
    }

    public override void Enter()
    {
        image.color = cursorColor.selectImageColor;
    }

    public override void Exit()
    {
        image.color = cursorColor.notSelectcImageColor;
    }
}
