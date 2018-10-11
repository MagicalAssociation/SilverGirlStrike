using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public SpriteRenderer sprite;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.localPosition += new Vector3(5.0f, 0.0f, 0.0f);
	}
    /**
     * 重力処理
     */
    private void Function()
    {
    }
}
