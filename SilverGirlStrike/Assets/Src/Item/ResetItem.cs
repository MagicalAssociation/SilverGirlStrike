using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ResetItem : CursorParam {
    public ItemSetSelectManager manager;
    public SGS.CursorColor cursorColor;
    public Image image;
    public override void Decision()
    {
        manager.parameter.Reset();
    }

    public override void Enter()
    {
        image.color = cursorColor.selectImageColor;
    }

    public override void Exit()
    {
        image.color = cursorColor.notSelectcImageColor;
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
