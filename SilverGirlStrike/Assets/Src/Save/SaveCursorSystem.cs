using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveCursorSystem : CursorSystem {

    [System.Serializable]
    public class CursorColor
    {
        public Color selectImageColor;
        public Color notSelectcImageColor;
        public Color selectTextColor;
        public Color notSelectTextColor;
    }
    public CursorColor cursorColor;
    public GameObject loadSelects;
    public SelectManager selectManager;
    private SaveGame[] saveGame;
    private void Start()
    {
        GameData.SaveNameCreate();
        base.Init();
        saveGame = loadSelects.GetComponentsInChildren<SaveGame>();
        SetColor();
    }
    private void Update()
    {
        if (CursorMoveInput())
        {
            //色変更処理
            SetColor();
        }
        else
        {
            //決定ボタンで決定処理を行う
            if (M_System.input.Down(SystemInput.Tag.DECISION))
            {
                base.GetNowParam().Decision();
                selectManager.GameDataUpdate();
            }
        }
    }
    bool CursorMoveInput()
    {
        if (M_System.input.Down(SystemInput.Tag.LSTICK_UP))
        {
            return base.Up();
        }
        else if (M_System.input.Down(SystemInput.Tag.LSTICK_DOWN))
        {
            return base.Down();
        }
        return false;
    }
    void SetColor()
    {
        for (int i = 0; i < saveGame.Length; ++i)
        {
            if (i == GetNow().y)
            {
                //選択カラー
                saveGame[i].image.color = cursorColor.selectImageColor;
                saveGame[i].text.color = cursorColor.selectTextColor;
            }
            else
            {
                //非選択カラー
                saveGame[i].image.color = cursorColor.notSelectcImageColor;
                saveGame[i].text.color = cursorColor.notSelectTextColor;
            }
        }
    }
}
