using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class foot : MonoBehaviour {
    public bool isFoot;

    public Collider2D collider;
	// Use this for initialization
	void Start () {
        this.collider = GetComponent<Collider2D>();
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if(Physics2D.OverlapBox(this.collider.transform.position, this.collider.transform.localScale, this.collider.transform.eulerAngles.z, (int)M_System.LayerName.GROUND) == null)
        {
            this.isFoot = false;
        }
        else
        {
            this.isFoot = true;
        }
        Debug.Log(this.collider.transform.position);

        Debug.DrawLine(collider.transform.position - (collider.transform.localScale / 2.0f), collider.transform.position + (collider.transform.localScale / 2.0f));

    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag != "Ground")
        {
            return;
        }
        //this.isFoot = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
       // this.isFoot = false;
    }

    public bool CheckHit()
    {
        return (Physics2D.OverlapBox(this.transform.position, this.transform.localScale, this.transform.eulerAngles.z, (int)M_System.LayerName.GROUND) == null) ? false : true;
    }
}
