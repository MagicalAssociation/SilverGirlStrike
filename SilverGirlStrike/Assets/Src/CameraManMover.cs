using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//*@brief:class カメラマンの移動に関する処理
//@author:薄井
//@date:2018.10.19

public class CameraManMover : MonoBehaviour {

    public GameObject target;       //カメラを移動させる終点

    [SerializeField, Range(0.01f, 0.5f)]
    float easing;              //定数を代入(仮)

    public float distance;       //カメラマンとプレイヤとの距離を指定(現在は機能していない)
    private bool isStop;

    // Use this for initialization
    void Start () {
        transform.position = new Vector3(target.transform.position.x + distance, target.transform.position.y, target.transform.position.z);
        isStop = false;     //現在停止しているか
	}
	
	// Update is called once per frame
	void Update () {
        MoveCameraMan();        //移動処理実行
    }

    //カメラマンの移動処理
    void MoveCameraMan()
    {
        // 2点間の距離を速度に反映する
        Vector3 diff = target.transform.position - transform.position;
        Vector3 v = diff * easing;
        transform.position += v;

        // 十分近づいたら移動終了
        if (diff.magnitude < 0.01f)     //ベクトルの距離
        {
            diff = new Vector3(0, 0, 0);
        }
    }
    
    //引数にターゲットを入れることで追従対象を更新できる関数
    void GetTarget(GameObject target_)
    {
        target = target_;
    }
}
