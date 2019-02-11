using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//*@brief:class カメラの基本動作(カメラマンを追従する)
//@author:薄井
//@date:2018.10.19

public class NomalCameraMover : MonoBehaviour {

    public GameObject target;   //通常はプレイヤを追従
    public int posZ;            //カメラの引き具合を指定
    //基本的にカメラはプレイヤーを中央に映す
    //中央からずらす場合は以下の変数にずらす分の値を設定
    public int posY;
    public int posX;

    //カメラの停止処理に使用する
    public bool IsStop;         //カメラを止めるか否か
    //Vector2 stopPos;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        CheckTarget();
    }

    //ターゲットが存在するかどうか
    void CheckTarget(/*Vector2 pos*/)
    {
        //ターゲットが存在していなかったら
        if (target == null)
        {

        }
        //ターゲットが存在していたら
        else
        {
            if (!IsStop)
            {
                transform.position = new Vector3(target.transform.position.x + posX, target.transform.position.y + posY, posZ);
            }
        }
    }
}
