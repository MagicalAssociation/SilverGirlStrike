﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

//編集履歴
// 2018/10/15 板倉

//指定個数のスプライトを、左右にループする（上スクロールへの対応も後々やっておく）
public class SpriteLoop : MonoBehaviour
{
    [Range(0, 1)]
    public float
        speed = 1;
    public int spriteCount = 3;
    Vector3 spriteSize;
    public PixelPerfectCamera viewCamera;
    public Rect screenRect;
    float pixelPerUnit;

    //コピーされたものはこの変数がtrueになる
    public bool copyFrag;


    public Transform cameraPos;
    Vector3 prevCameraPos;

    SpriteLoop()
    {
        //Start()はInstantiate()の直後に呼ばれるわけではないので、Instantiate()直後にcopyFragを下す
        this.copyFrag = true;
    }

    void Start()
    {
        this.prevCameraPos = this.cameraPos.position;

        Initialize();

        //コピー元のオブジェクトだけがクローンを生成
        if (this.copyFrag)
        {
            for(int i = 1; i < this.spriteCount; ++i)
            {
                var a = Instantiate<GameObject>(this.gameObject).GetComponent<SpriteLoop>();
                a.copyFrag = false;
                float moveDistance = this.spriteSize.x / this.pixelPerUnit * i;
                a.transform.position = this.transform.position;
                a.transform.position += new Vector3(moveDistance, 0.0f, 0.0f);
            }
        }
    }

    //少なくともカメラの移動後、という要件を満たせばLateUpdate()でなくてもいい
    void LateUpdate()
    {
        Vector3 moveVector = this.cameraPos.position - this.prevCameraPos;
        this.transform.position += moveVector * speed;
        this.prevCameraPos = this.cameraPos.position;

        //Debug.Log(this.prevCameraPos);


#if UNITY_EDITOR

        var spritex = ((transform.position - this.cameraPos.position) * this.pixelPerUnit + this.spriteSize / 2).x;
        if (spritex < this.screenRect.x)
        {
            FixL();
        }
        if ((transform.position.x - this.cameraPos.position.x) * this.pixelPerUnit > this.screenRect.x + this.screenRect.width * this.spriteCount)
        {
            FixR();
        }
        //Debug.Log(transform.position * this.pixelPerUnit);
#endif
    }

    void FixL()
    {
        //スプライトのサイズ分だけ移動
        float moveDistance = this.spriteSize.x / this.pixelPerUnit;
        this.transform.position += new Vector3(moveDistance, 0.0f, 0.0f) * this.spriteCount;
    }
    void FixR()
    {
        //スプライトのサイズ分だけ移動
        float moveDistance = this.spriteSize.x / this.pixelPerUnit;
        this.transform.position -= new Vector3(moveDistance, 0.0f, 0.0f) * this.spriteCount;
    }



    public void Initialize()
    {
        this.pixelPerUnit = GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
        this.spriteSize = GetComponent<SpriteRenderer>().size * this.pixelPerUnit;

        this.screenRect.x = -this.viewCamera.refResolutionX * 0.5f;
        this.screenRect.y = this.viewCamera.refResolutionY * 0.5f;
        this.screenRect.width = this.viewCamera.refResolutionX;
        this.screenRect.height = this.viewCamera.refResolutionY;
    }

}
