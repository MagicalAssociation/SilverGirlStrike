using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTest : MonoBehaviour {
   
	// Use this for initialization
	void Start () {
	}

    // Update is called once per frame
    void Update()
    {
        //if(M_System.input.Down(SystemInput.Tag.JUMP))
        //{
        //    M_System.input.SetAxisForced(SystemInput.Tag.ATTACK, 0.8f);
        //    M_System.input.SetForced(SystemInput.Tag.ATTACK, !M_System.input.GetForced(SystemInput.Tag.ATTACK));
        //}
        //Debug.Log("On  :" + SystemInput.Tag.ATTACK + M_System.input.On(SystemInput.Tag.ATTACK));
        //Debug.Log("On  :" + SystemInput.Tag.LSTICK_UP + M_System.input.On(SystemInput.Tag.LSTICK_UP));
        //Debug.Log("Axis:" + SystemInput.Tag.ATTACK + M_System.input.Axis(SystemInput.Tag.ATTACK));
        //Debug.Log("Axis:" + SystemInput.Tag.LSTICK_UP + M_System.input.Axis(SystemInput.Tag.LSTICK_UP));
        Debug.Log(M_System.input.On(SystemInput.Tag.LSTICK_DOWN));
    }
}