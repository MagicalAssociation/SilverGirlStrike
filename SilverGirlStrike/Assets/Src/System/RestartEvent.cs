using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//ゲーム開始時の初期状態を変更する処理を行う
public class RestartEvent : MonoBehaviour {
    [SerializeField]
    private Transform startPoint;

    //初期位置をいじるプレイヤー
    [SerializeField]
    private GameObject player;


    void Start () {
        float zPos = this.player.transform.position.z;
        player.transform.position = new Vector3(this.startPoint.position.x, this.startPoint.position.y, zPos);
	}
	
	void Update () {
		
	}
}
