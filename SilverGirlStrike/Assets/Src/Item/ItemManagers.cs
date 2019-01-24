using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManagers : CursorSystemManager
{
    public enum Type
    {
        SELECT = 0,
        SET = 1,
        GAME = 2,
    }
    public Type type;
    // Use this for initialization
    void Start () {
        base.Init((int)type);
	}
	
	// Update is called once per frame
	void Update () {
        base.SystemUpdate();
	}
}
