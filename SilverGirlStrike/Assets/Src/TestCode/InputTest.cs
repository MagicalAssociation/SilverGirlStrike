using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTest : MonoBehaviour {
   
	// Use this for initialization
	void Start () {
        M_System.input.SetEnableStop(SystemInput.Tag.CANCEL, true);
	}
	
	// Update is called once per frame
	void Update () {
    }
}