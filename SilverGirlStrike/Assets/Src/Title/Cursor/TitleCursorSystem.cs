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
    Vector2 imagePos;
    Parameter parameter;
    private void Start()
    {
        cursorMove = new Easing();
        cursorMove.Use(new Easing.Linear());
        imagePos = base.GetNowParam().transform.position;
    }
    private void Update()
    {
        //if(M_System.input.Down(SystemInput.Tag.LSTICK_UP))
        //{
        //    base.Up();
        //}
        //else if(M_System.input.Down(SystemInput.Tag.LSTICK_DOWN))
        //{
        //    base.Down();
        //}
        //else if(M_System.input.Down(SystemInput.Tag.LSTICK_LEFT))
        //{
        //    base.Left();
        //}
        //else if(M_System.input.Down(SystemInput.Tag.LSTICK_RIGHT))
        //{
        //    base.Right();
        //}

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
                    
                }
            }
        }
        else
        {
            this.imagePos.y = this.cursorMove.In();
        }
        
    }
    bool CursorMoveInput()
    {
        return (M_System.input.Down(SystemInput.Tag.LSTICK_UP)) ? 
            base.Up() : (M_System.input.Down(SystemInput.Tag.LSTICK_DOWN)) ? 
            base.Down() : (M_System.input.Down(SystemInput.Tag.LSTICK_LEFT)) ? 
            base.Left() : (M_System.input.Down(SystemInput.Tag.LSTICK_RIGHT)) ? 
            base.Right() : false;
    }
    void SetMoveCursorEasing()
    {
        this.cursorMove.ResetTime();
        this.cursorMove.Set(this.imagePos.y, base.GetNowParam().transform.position.y - this.imagePos.y, parameter.moveTime);
    }
}
