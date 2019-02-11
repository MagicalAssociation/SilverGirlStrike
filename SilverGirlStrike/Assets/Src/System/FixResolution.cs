using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class FixResolution : MonoBehaviour {
    public float width;
	// Use this for initialization
	void Awake () {
        GetComponent<PixelPerfectCamera>().assetsPPU = (int)(50.0f * ((float)Screen.width / width));
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
