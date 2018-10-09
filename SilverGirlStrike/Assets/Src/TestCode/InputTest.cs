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
       if(M_System.input.Down(SystemInput.Tag.DECISION))
        {
            Debug.Log("DECISION:DOWN");
        }
        if (M_System.input.Down(SystemInput.Tag.CANCEL))
        {
            Debug.Log("CANCEL:DOWN");
        }
        if (M_System.input.Down(SystemInput.Tag.JUMP))
        {
            Debug.Log("JUMP:DOWN");
        }
        if (M_System.input.Down(SystemInput.Tag.ITEM_D))
        {
            Debug.Log("ITEM_D:DOWN");
        }
    }
}