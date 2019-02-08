using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class anchorHitEffect : MonoBehaviour {

    public float goalScale;
    public float movePower;
    float alpha;
    int timeCnt;
    Color baseColor;

	// Use this for initialization
	void Start () {
        this.transform.localScale = Vector3.zero;
        this.alpha = 1;
        this.timeCnt = 0;
        this.baseColor = GetComponent<SpriteRenderer>().color;
    }
	
	// Update is called once per frame
	void Update () {
        ++this.timeCnt;
        if (this.timeCnt > 10)
        {
            GetComponent<SpriteRenderer>().color = new Color(this.baseColor.r, this.baseColor.g, this.baseColor.b, this.alpha);
            this.alpha += -this.alpha * 0.1f;
        }


        float scale = (this.goalScale - this.transform.localScale.x) * this.movePower;
        this.transform.localScale += Vector3.one * scale;

        if(this.alpha < 0.0001f)
        {
            Destroy(this.gameObject);
        }
    }
}
