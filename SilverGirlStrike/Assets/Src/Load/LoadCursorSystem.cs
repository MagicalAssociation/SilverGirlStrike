using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadCursorSystem : CursorSystem {
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
    private LoadGameStart[] loadParams;
    private void Start()
    {
        base.Init();
        loadParams = loadSelects.GetComponentsInChildren<LoadGameStart>();
        SetColor();
    }
    private void Update()
    {
        if(CursorMoveInput())
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
            }
            else if (M_System.input.Down(SystemInput.Tag.CANCEL))
            {
                StartCoroutine(Cancel());
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
        for(int i = 0;i < loadParams.Length;++i)
        {
            if(i == GetPos().y)
            {
                //選択カラー
                loadParams[i].image.color = cursorColor.selectImageColor;
                loadParams[i].text.color = cursorColor.selectTextColor;
            }
            else
            {
                //非選択カラー
                loadParams[i].image.color = cursorColor.notSelectcImageColor;
                loadParams[i].text.color = cursorColor.notSelectTextColor;
            }
        }
    }
    IEnumerator Cancel()
    {
        yield return new WaitForSeconds(0.0f);
        SceneManager.LoadScene("TitleScene");
    }
}
