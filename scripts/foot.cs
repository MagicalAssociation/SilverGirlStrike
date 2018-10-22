using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class foot : MonoBehaviour {
    public bool isFoot;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        this.isFoot = false;
	}

    private void OnCollisionStay2D(Collision2D collision)
    {
        this.isFoot = true;
    }
}
