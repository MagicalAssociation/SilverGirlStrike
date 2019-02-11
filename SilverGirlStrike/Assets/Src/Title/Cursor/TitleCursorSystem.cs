using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCursorSystem : CursorSystem
{
    [System.Serializable]
    public class Parameter
    {
        public Easing.Type type;
        public Easing.MoveType moveType;
        public float moveTime;
    }
    Easing[] cursorMove;
    //Vector2 imagePos;
    public Parameter parameter;
    public GameObject cursorObject;
    private void Awake()
    {
        base.Init();
        cursorMove = new Easing[2];
        cursorMove[0] = new Easing();
        cursorMove[1] = new Easing();
        //EasingはLinearを使用（仮）
        cursorMove[0].Use(parameter.type);
        cursorMove[1].Use(parameter.type);
        //初期位置は0,0の位置(仮)
        cursorObject.transform.position = base.GetNowParam().transform.position;
    }
    private void Update()
    {
        //カーソルが動いていない時に決定を優先に入力判定を行う
        if (!cursorMove[0].IsPlay() && !cursorMove[1].IsPlay())
        {
            //決定ボタンで決定処理を行う
            if (M_System.input.Down(SystemInput.Tag.DECISION) && base.GetEnable())
            {
                Sound.PlaySE("systemDesision");
                base.GetNowParam().Decision();
            }
            else
            {
                //カーソルの移動入力の検知
                if(CursorMoveInput())
                {
                    Sound.PlaySE("systemMove");
                    //Easingの登録をする
                    SetMoveCursorEasing();
                }
            }
        }
        else
        {
            //Easingを使ってy座標の移動をする
            cursorObject.transform.position = new Vector2(this.cursorMove[0].Move(parameter.moveType), this.cursorMove[1].Move(parameter.moveType));
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
        this.cursorMove[0].ResetTime();
        this.cursorMove[0].Set(cursorObject.transform.position.x, base.GetNowParam().transform.position.x, parameter.moveTime);
        this.cursorMove[1].ResetTime();
        this.cursorMove[1].Set(cursorObject.transform.position.y, base.GetNowParam().transform.position.y, parameter.moveTime);
    }
}
