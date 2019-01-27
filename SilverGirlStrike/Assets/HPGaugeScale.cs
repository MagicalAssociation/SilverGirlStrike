using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//より汎用的に、同じものを並べて置くことでHPを表現するクラス
public class HPGaugeScale : MonoBehaviour {
    public GameObject scaleObject;
    //目盛りをすべて管理
    List<GameObject> scales;

    public float distance;

    public int max;
    public int currentValue;

	// Use this for initialization
	void Start () {
        this.scales = new List<GameObject>();

        //まずマックスまでオブジェクト生成
        for (int i = 0; i < this.max; ++i) {
            var obj = Instantiate<GameObject>(this.scaleObject, this.transform);
            obj.transform.localPosition = Vector3.right * i * this.distance;
            this.scales.Add(obj);
        }

	}
	
	// Update is called once per frame
	void Update () {
        for(int i = 0; i < this.scales.Count; ++i)
        {
            //値に届いてないものを非表示、届いているのを表示
            if(i + 1 <= this.currentValue)
            {
                this.scales[i].SetActive(true);
            }
            else
            {
                this.scales[i].SetActive(false);
            }
        }

	}
}
