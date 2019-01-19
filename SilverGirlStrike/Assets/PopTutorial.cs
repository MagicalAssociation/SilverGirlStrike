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
        AnimatorClipInfo clipInfo = animator.GetCurrentAnimatorClipInfo(0)[0];
        if (!this.isActive)
        {
            if (clipname != "disappear")
            {
                animator.Play("disappear");
                clipname = "disappear";
            }
        }
        else
        {
            if (clipname != "appear")
            {
                animator.Play("appear");
                clipname = "appear";
            }
        }

        this.isActive = false;
        Debug.Log("c");
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("a");
        this.isActive = true;
    }
}
