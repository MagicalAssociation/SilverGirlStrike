using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemReset : CursorSystem
{
    public ItemSelectManager itemSelect;
    public override void Enter()
    {
        base.GetNowParam().Enter();
    }

    public override void Exit()
    {
        base.GetNowParam().Exit();
    }

    public override void SystemUpdate(CursorSystemManager manager)
    {

        if (CursorMove())
        {
            base.GetNowParam().Enter();
        }
        else
        {
            if (M_System.input.Down(SystemInput.Tag.CANCEL))
            {
                manager.Next((int)ItemSelectManagers.Type.SELECT);
            }
            else if(M_System.input.Down(SystemInput.Tag.LSTICK_UP))
            {
                itemSelect.WarpPosition(Warp.BUTTOM);
                manager.Next((int)ItemSelectManagers.Type.SELECT);
            }
            else if(M_System.input.Down(SystemInput.Tag.LSTICK_DOWN))
            {
                itemSelect.WarpPosition(Warp.TOP);
                manager.Next((int)ItemSelectManagers.Type.SELECT);
            }
            else if (M_System.input.Down(SystemInput.Tag.DECISION))
            {
                GetNowParam().Decision();
                manager.Next((int)ItemSelectManagers.Type.SELECT);
            }
        }
    }
    private bool CursorMove()
    {
        if (M_System.input.Down(SystemInput.Tag.LSTICK_LEFT))
        {
            base.GetNowParam().Exit();
            return base.Left();
        }
        else if (M_System.input.Down(SystemInput.Tag.LSTICK_RIGHT))
        {
            base.GetNowParam().Exit();
            return base.Right();
        }
        return false;
    }
    // Use this for initialization
    void Start()
    {
        base.Init();
        for (int i = 0; i < base.GetList().Count; ++i)
        {
            for (int j = 0; j < base.GetLine(i).Length; ++j)
            {
                base.GetParam(i, j).Exit();
            }
        }
    }
}
