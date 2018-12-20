using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CursorParam
{
    public abstract void Decision();
}

public class CursorSystem : MonoBehaviour {
    public GameObject[] cursors;
    public Animator animator;
    private Vector2 nowPosition;
    private Vector2Int nowPos;
	// Use this for initialization
	void Start () {
        nowPos = new Vector2Int(0, 0);
	}
	
	// Update is called once per frame
	void Update () {
        cursors[0].GetComponentsInChildren<CursorParam>()[0].Decision();
	}
}