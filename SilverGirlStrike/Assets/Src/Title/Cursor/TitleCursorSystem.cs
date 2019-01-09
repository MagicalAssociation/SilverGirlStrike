using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCursorSystem : CursorSystem
{
    [System.Serializable]
    public class Parameter
    {
        public float moveTime;
    }
    Easing cursorMove;
    //Vector2 imagePos;
    public Parameter parameter;
    public GameObject cursorObject;
    private void Awake()
    {
        base.Init();
        cursorMove = new Easing();
        //EasingはLinearを使用（仮）
        cursorMove.Use(new Easing.Linear());
        //cursorObject = GetComponentInChildren<GameObject>();
        //初期位置は0,0の位置(仮)
        cursorObject.transform.position = base.GetNowParam().transform.position;
    }
    private void Update()
    {
        //カーソルが動いていない時に決定を優先に入力判定を行う
        if (!cursorMove.IsPlay())
        {
            //決定ボタンで決定処理を行う
            if (M_System.input.Down(SystemInput.Tag.DECISION))
            {
                base.GetNowParam().Decision();
            }
            else
            {
                //カーソルの移動入力の検知
                if(CursorMoveInput())
                {
                    //Easingの登録をする
                    SetMoveCursorEasing();
                }
            }
        }
        else
        {
            //Easingを使ってy座標の移動をする
            cursorObject.transform.position = new Vector2(cursorObject.transform.position.x, this.cursorMove.In());
        }
        
    }
    bool CursorMoveInput()
    {
        if(M_System.input.On(SystemInput.Tag.LSTICK_UP))
        {
            return base.Up();
        }
        else if(M_System.input.On(SystemInput.Tag.LSTICK_DOWN))
        {
            return base.Down();
        }
        return false;
    }
    void SetMoveCursorEasing()
    {
        //Easingを登録
        this.cursorMove.ResetTime();
        this.cursorMove.Set(cursorObject.transform.position.y, base.GetNowParam().transform.position.y - cursorObject.transform.position.y, parameter.moveTime);
    }
}
