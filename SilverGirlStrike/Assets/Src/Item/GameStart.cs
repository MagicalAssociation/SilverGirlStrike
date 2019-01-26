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
        M_System.gameStartItems[(int)M_System.ItemDirection.UP] = manager.parameter.up.GetItem();
        M_System.gameStartItems[(int)M_System.ItemDirection.DOWN] = manager.parameter.down.GetItem();
        M_System.gameStartItems[(int)M_System.ItemDirection.LEFT] = manager.parameter.left.GetItem();
        M_System.gameStartItems[(int)M_System.ItemDirection.RIGHT] = manager.parameter.right.GetItem();
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
