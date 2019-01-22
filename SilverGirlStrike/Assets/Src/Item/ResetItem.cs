using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetItem : CursorParam {
    public ItemSetSelectManager manager;
    public override void Decision()
    {
        manager.parameter.Reset();
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
