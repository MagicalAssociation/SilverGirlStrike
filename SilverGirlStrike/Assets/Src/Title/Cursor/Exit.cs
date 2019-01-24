using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameExit : CursorParam
{

    public override void Decision()
    {
        Application.Quit();
    }

    public override void Enter()
    {
        throw new System.NotImplementedException();
    }

    public override void Exit()
    {
        throw new System.NotImplementedException();
    }
}
