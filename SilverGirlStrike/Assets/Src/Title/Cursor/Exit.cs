using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : CursorParam
{
    public override void Decision()
    {
        Application.Quit();
    }
}
