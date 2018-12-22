using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDelete : MonoBehaviour {

    public float deleteCnt;
    float timeCnt;

	// Use this for initialization
	void Start () {
        timeCnt = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
        timeCnt += Time.deltaTime;
        Delete();
	}

    void Delete()
    {
        if(this.deleteCnt < timeCnt)
        {
            Destroy(this.gameObject);
        }
    }
}
