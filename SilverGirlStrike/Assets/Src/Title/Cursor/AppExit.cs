using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppExit : CursorParam
{

    public override void Decision()
    {
        Application.Quit();
    }

    public override void Enter()
    {
    }

    public override void Exit()
    {
    }
}
