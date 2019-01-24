using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetItem : CursorParam {
    public ItemSetSelectManager manager;
    public SGS.CursorColor cursorColor;
    public override void Decision()
    {
        manager.parameter.Reset();
    }

    public override void Enter()
    {
        throw new System.NotImplementedException();
    }

    public override void Exit()
    {
        throw new System.NotImplementedException();
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
