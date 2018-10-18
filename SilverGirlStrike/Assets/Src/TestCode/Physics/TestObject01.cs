using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObject01 : MonoBehaviour {
    public float move;
    public float premove;
    public float oneframeTime;
    private bool moveing;
    private float timer;
	// Use this for initialization
	void Start () {
        this.premove = this.transform.localPosition.y;
        this.oneframeTime = Time.time;
        this.moveing = false;
        timer = 0.0f;
	}

    // Update is called once per frame
    void Update()
    {
        this.Move();
        if(this.moveing)
        {
            timer += Time.deltaTime;
            if(timer > 1.0f)
            {
                Debug.Log(this.transform.localPosition.y - this.premove);
                this.premove = this.transform.localPosition.y;
                timer = 0.0f;
            }
        }

    }

    void Move()
    {
        if (timer == 0.0f)
        {
            this.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, move * -1.0f);
            this.moveing = true;
        }  
    }
}
