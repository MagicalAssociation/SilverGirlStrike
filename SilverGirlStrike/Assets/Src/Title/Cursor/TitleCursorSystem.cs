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
    private void Start()
    {
        base.Init();
        cursorMove = new Easing();
        cursorMove.Use(new Easing.Linear());
        //cursorObject = GetComponentInChildren<GameObject>();
        cursorObject.transform.position = base.GetNowParam().transform.position;
    }
    private void Update()
    {
        //カーソルが動いていない時に決定を優先に入力判定を行う
        if (!cursorMove.IsPlay())
        {
            if (M_System.input.Down(SystemInput.Tag.DECISION))
            {
                base.GetNowParam().Decision();
            }
            else
            {
                if(CursorMoveInput())
                {
                    //Easingの登録をする
                    SetMoveCursorEasing();
                }
            }
        }
        else
        {
            cursorObject.transform.position = new Vector2(cursorObject.transform.position.x, this.cursorMove.In());
        }
        
    }
    bool CursorMoveInput()
    {
        //return (M_System.input.On(SystemInput.Tag.LSTICK_UP)) ? 
        //    base.Up() : (M_System.input.On(SystemInput.Tag.LSTICK_DOWN)) ? 
        //    base.Down() : (M_System.input.On(SystemInput.Tag.LSTICK_LEFT)) ? 
        //    base.Left() : (M_System.input.On(SystemInput.Tag.LSTICK_RIGHT)) ? 
        //    base.Right() : false;
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
        this.cursorMove.ResetTime();
        this.cursorMove.Set(cursorObject.transform.position.y, base.GetNowParam().transform.position.y - cursorObject.transform.position.y, parameter.moveTime);
    }
}
