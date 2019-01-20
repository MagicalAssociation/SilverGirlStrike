using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopTutorial : MonoBehaviour {

    public Animator animator;

    bool isActive;
    string clipname;

	// Use this for initialization
	void Start () {
        this.isActive = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        this.isActive = false;
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        //Debug.Log(isActive);
        if (clipname != "appear")
        {
            animator.Play("appear");
            clipname = "appear";
        }
        this.isActive = true;
    }

}
