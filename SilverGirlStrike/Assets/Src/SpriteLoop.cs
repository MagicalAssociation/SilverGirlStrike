using System.Collections;
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
    public int spritePos;

    float movedDistance;

    //コピーされたものはこの変数がtrueになる
    public bool copyFrag;


    public Transform cameraPos;
    Vector3 prevCameraPos;

    SpriteLoop()
    {
        //Start()はInstantiate()の直後に呼ばれるわけではないので、Instantiate()直後にcopyFragを下す
        this.copyFrag = true;
    }
    private void Awake()
    {
        this.prevCameraPos = this.cameraPos.position;
    }
    void Start()
    {
        Initialize();

        //コピー元のオブジェクトだけがクローンを生成
        if (this.copyFrag)
        {
            Transform[] transforms = new Transform[this.spriteCount];

            for(int i = 0; i < this.spriteCount; ++i)
            {
                var a = Instantiate<GameObject>(this.gameObject).GetComponent<SpriteLoop>();
                a.copyFrag = false;
                float moveDistance = this.spriteSize.x / this.pixelPerUnit * i;
                a.movedDistance = moveDistance;
                a.transform.position = this.transform.position;
                a.transform.position += new Vector3(moveDistance, 0.0f, 0.0f);
                transforms[i] = a.transform;
            }
            for(int i = 0; i < transforms.Length; ++i)
            {
                transforms[i].parent = this.transform;
            }

            //コピー元は非表示（座標が動かない）
            this.enabled = false;
            this.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    //少なくともカメラの移動後、という要件を満たせばLateUpdate()でなくてもいい
    void LateUpdate()
    {
        //可動域
        float moveDistance = this.spriteSize.x * this.spritePos;

        if (this.copyFrag)
        {
            Vector3 moveVector = this.cameraPos.position - this.prevCameraPos;
            this.transform.position += moveVector * speed;
            this.prevCameraPos = this.cameraPos.position;
        }
        {
            Vector3 moveVector = this.cameraPos.position - this.prevCameraPos;
            this.transform.position += moveVector * speed;
            this.prevCameraPos = this.cameraPos.position;

            this.movedDistance -= moveVector.x * (1.0f - speed);
        }

        //左端に達した時
        if (this.movedDistance < 0.0f)
        {
           FixL();
        }
        //右端に達した時
        if (this.movedDistance > this.spriteSize.x / this.pixelPerUnit * this.spriteCount)
        {
            FixR();
        }
    }

    void FixL()
    {
        //スプライトのサイズ分だけ移動
        float moveDistance = this.spriteSize.x / this.pixelPerUnit;
        this.transform.position += new Vector3(moveDistance, 0.0f, 0.0f) * this.spriteCount;
        this.movedDistance += moveDistance * this.spriteCount;
    }
    void FixR()
    {
        //スプライトのサイズ分だけ移動
        float moveDistance = this.spriteSize.x / this.pixelPerUnit;
        this.transform.position -= new Vector3(moveDistance, 0.0f, 0.0f) * this.spriteCount;
        this.movedDistance -= moveDistance * this.spriteCount;
    }



    public void Initialize()
    {
        this.pixelPerUnit = GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
        this.spriteSize = GetComponent<SpriteRenderer>().size * this.pixelPerUnit;

        //左上を原点としたカメラ矩形
        this.screenRect.x = -this.viewCamera.refResolutionX * 0.5f;
        this.screenRect.y = this.viewCamera.refResolutionY * 0.5f;
        this.screenRect.width = this.viewCamera.refResolutionX;
        this.screenRect.height = this.viewCamera.refResolutionY;
    }

}
