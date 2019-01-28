using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//StageSelectは表示するものをCanvasの子にいくつか作ってそれを指定する
//次の画面を横にスライドしながら表示する
//操作するManagerを管理するものを作る

public class ItemSelectManagers : CursorSystemManager
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
