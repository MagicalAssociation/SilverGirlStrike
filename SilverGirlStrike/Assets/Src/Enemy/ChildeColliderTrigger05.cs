using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildeColliderTrigger05 : MonoBehaviour {

    public Collider2D player;
    public bool hit;

    // Use this for initialization
    void Start () {
        this.hit = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!this.hit)
        {
            if (other.tag == "Player")            //otherとplayerを当てたい
            {
                this.hit = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")            //範囲から外れた
        {
            this.hit = false;
        }
    }

    // Update is called once per frame
    void Update () {
	}

    public bool GetPlayerHit()
    {
        return this.hit;
    }
}
