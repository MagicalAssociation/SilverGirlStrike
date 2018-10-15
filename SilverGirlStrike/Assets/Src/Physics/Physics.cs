using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Physics_ : MonoBehaviour {
    //任意のオブジェクトとだけの衝突を行う
    public LayerMask layerMask;
    public float skinWitdh = 0.015f;
    public int xAxisCount = 4;
    public int yAxisCount = 4;
    public float maxClimbAngle = 80;
    float xAxis;
    float yAxis;
    BoxCollider2D boxCollider;
    //Physics.
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
