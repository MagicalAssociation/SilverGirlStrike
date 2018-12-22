﻿using System.Collections;
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
    Vector3 targetPosition;


    // Use this for initialization
    void Awake () {
        if(this.chaseTarget == null)
        {
            return;
        }

        //ターゲットとの相対位置を取得
        this.relativePosition = this.transform.position - this.chaseTarget.position;
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if (this.chaseTarget == null)
        {
            return;
        }

        //相対位置分だけずらして追尾
        if (this.chaseTarget != this.transform)
        {
            this.targetPosition = this.chaseTarget.position;
        }


        this.transform.position += ((this.targetPosition + this.relativePosition) - this.transform.position) * this.chasePower * (Time.deltaTime * 60.0f);
	}


    //追従の速度を設定
    public void SetChasePowerRatio(float power)
    {
        this.chasePower = power;
    }
    //相対位置を指定
    public void SetRelativePosition(Vector3 pos)
    {
        this.relativePosition = pos;
    }

    //相対位置を指定
    public void SetRelativePosition(Vector2 pos)
    {
        this.relativePosition.x = pos.x;
        this.relativePosition.y = pos.y;
    }

    //移動追従のターゲットを設定
    public void SetTarget(Transform target)
    {
        this.chaseTarget = target;
    }
    //移動追従のターゲットを設定
    public void SetTarget(Vector3 targetPosition)
    {
        this.chaseTarget = this.transform;
        this.targetPosition = targetPosition;

    }

    //目的地の位置で追従をやめる
    public void StopChase()
    {
        SetTarget(this.chaseTarget.position);
    }
}
