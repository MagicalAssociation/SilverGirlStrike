using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("A"))
        {
            Debug.Log("Input:A");
        }
        if (Input.GetButtonDown("B"))
        {
            Debug.Log("Input:B");
        }
        if (Input.GetButtonDown("X"))
        {
            Debug.Log("Input:X");
        }
        if (Input.GetButtonDown("Y"))
        {
            Debug.Log("Input:Y");
        }
        if (Input.GetButtonDown("U"))
        {
            Debug.Log("Input:U");
        }
        if(Input.GetButtonDown("D"))
        {
            Debug.Log("Input:D");
        }
        if(Input.GetButtonDown("L"))
        {
            Debug.Log("Input:L");
        }
        if(Input.GetButtonDown("R1"))
        {
            Debug.Log("Input:R1");
        }
        if (Input.GetButtonDown("L1"))
        {
            Debug.Log("Input:L1");
        }
        if (Input.GetButtonDown("R"))
        {
            Debug.Log("Input:R");
        }
        if (Input.GetButtonDown("START"))
        {
            Debug.Log("Input:START");
        }
        if (Input.GetAxis("U") > 0.0f)
        {
            Debug.Log("Axis:U:" + Input.GetAxis("U"));
        }
        if (Input.GetAxis("D") > 0.0f)
        {
            Debug.Log("Axis:D:" + Input.GetAxis("D"));
        }
        if(Input.GetAxis("L") > 0.0f)
        {
            Debug.Log("Axis:L:" + Input.GetAxis("L"));
        }
        if (Input.GetAxis("R") > 0.0f)
        {
            Debug.Log("Axis:R:" + Input.GetAxis("R"));
        }
        if(Input.GetAxis("RStickX") != 0.0f)
        {
            Debug.Log("Axis:StickX" + Input.GetAxis("RStickX"));
        }
        if (Input.GetAxis("RStickY") != 0.0f)
        {
            Debug.Log("Axis:StickY" + Input.GetAxis("RStickY"));
        }
    }
}
