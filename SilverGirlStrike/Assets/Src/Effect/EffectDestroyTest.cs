using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//エフェクト用の、一定時間経過後にオブジェクトを自壊させるスクリプト

//薄井さんがエフェクト消去の機構をあげるまでのつなぎ
//もしかしたら、実際の処理に採用されるかもしれない
public class EffectDestroyTest : MonoBehaviour {
    public float deleteTime;
    private float currentTime;

	// Use this for initialization
	void Start () {
        this.currentTime = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
        this.currentTime += Time.deltaTime * 60.0f;

		if(this.currentTime > this.deleteTime)
        {
            Destroy(this.gameObject);
        }
	}
}
