using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleDitection : MonoBehaviour {
    public float radius;

    Collider2D[] hitList;

	// Use this for initialization
	void Start () {
        this.hitList = new Collider2D[0];
    }
	
	// Update is called once per frame
	void Update () {
        //for (int i = 0; i < this.hitList.Length; ++i)
        //{
        //    this.hitList[i].gameObject.GetComponent<SpriteRenderer>().color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        //}

        //int mask = 1 << 9;
        //this.hitList = Physics2D.OverlapCircleAll(this.transform.position, radius, mask);
        //for(int i = 0; i < this.hitList.Length; ++i)
        //{
        //    this.hitList[i].gameObject.GetComponent<SpriteRenderer>().color = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
        //}
	}

    public Collider2D[] FindAnchor()
    {
        int mask = (int)M_System.LayerName.ANCHOR;
        this.hitList = Physics2D.OverlapCircleAll(this.transform.position, radius, mask);

        return this.hitList;
    }

}
