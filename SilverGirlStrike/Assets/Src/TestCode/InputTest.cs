using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTest : MonoBehaviour {
    SystemInput systemInput = new SystemInput();
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
       if(systemInput.Down(SystemInput.Tag.DECISION))
        {
            Debug.Log("DECISION:DOWN");
        }
    }
}
