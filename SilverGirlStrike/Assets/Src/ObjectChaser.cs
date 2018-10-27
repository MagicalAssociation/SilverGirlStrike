using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//変更履歴
//2018/10/26 板倉（作成）


//特定のゲームオブジェクトに、遅れて追尾するスクリプト（相対位置で追尾位置をずらせる）
public class ObjectChaser : MonoBehaviour {
    public Transform chaseTarget;
    [Range(0, 1)]
    public float chasePower;
    Vector3 relativePosition;



    // Use this for initialization
    void Start () {
        //ターゲットとの相対位置を取得
        this.relativePosition = this.transform.position - this.chaseTarget.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        //相対位置分だけずらして追尾
        this.transform.position += (this.chaseTarget.position + this.relativePosition - this.transform.position) * this.chasePower;
	}
}
