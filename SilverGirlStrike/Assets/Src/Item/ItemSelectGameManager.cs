using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSelectGameManager : CursorSystem
{

    public override void Enter()
    {
    }

    public override void Exit()
    {
    }

    public override void SystemUpdate(CursorSystemManager manager)
    {

        if(CursorMove())
        {

        }
        else
        {
            if (M_System.input.Down(SystemInput.Tag.LSTICK_UP) || M_System.input.Down(SystemInput.Tag.LSTICK_DOWN))
            {
                manager.Next((int)ItemManagers.Type.SELECT);
            }
        }
    }
    private bool CursorMove()
    {
        if (M_System.input.Down(SystemInput.Tag.LSTICK_LEFT))
        {
            return base.Left();
        }
        else if(M_System.input.Down(SystemInput.Tag.LSTICK_RIGHT))
        {
            return base.Right();
        }
        return false;
    }
    // Use this for initialization
    void Start () {
        base.Init();
	}
}
