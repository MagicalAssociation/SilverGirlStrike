using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//プレイヤーのHPゲージと違ってもっと応用の利く感じに仕上げたゲージクラス
//左右の端を位置、中央の伸ばしを拡縮で調整する
//事前に最小単位で位置合わせを手動でしなくてはならない
public class SimpleScaleGauge : MonoBehaviour {
    public RectTransform left;
    Vector3 leftOrigin;

    public RectTransform right;
    Vector3 rightOrigin;

    public RectTransform middle;
    Vector3 middleOrigin;

    public Image middleImage;

    float moveDistance;
    public float scale;

    // Use this for initialization
    void Start () {
        //初期地点を保存
        this.leftOrigin = left.localPosition;
        this.rightOrigin = right.localPosition;
        this.middleOrigin = middle.localScale;

        //中央画像の半分にスケールをかける
        this.moveDistance = this.middleImage.sprite.bounds.size.x * this.middleImage.sprite.pixelsPerUnit * 0.5f * middleOrigin.x;
        Debug.Log(this.middleImage.sprite.bounds.size.x * this.middleImage.sprite.pixelsPerUnit * 0.5f * middleOrigin.x);
    }
	
	// Update is called once per frame
	void Update () {
        //左右を位置合わせ、中央を拡縮する
        this.middle.localScale = new Vector3(this.middleOrigin.x * this.scale, this.middleOrigin.y, this.middleOrigin.z);
        this.left.localPosition = this.leftOrigin + Vector3.left * (this.scale - 1.0f) * this.moveDistance;
        this.right.localPosition = this.rightOrigin + Vector3.right * (this.scale - 1.0f) * this.moveDistance;
	}
}
