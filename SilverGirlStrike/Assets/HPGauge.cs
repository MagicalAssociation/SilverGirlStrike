using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPGauge : MonoBehaviour {
    //HP最大値
    int maxHP;

    //表示するキャラ
    public CharacterObject targetCharacter;

    //延長枠の画像(コピー元)
    public GameObject frameImage;
    //ゲージの値を表すメモリの画像(コピー元)
    public GameObject barImage;

    //HPメモリ配列
    List<GameObject> barList;


    // Use this for initialization
    void Start () {
        this.barList = new List<GameObject>();

        //想定画面サイズに対しての倍率
        float aaa = Screen.width / 960.0f;

        this.maxHP = this.targetCharacter.GetData().hitPoint.GetMaxHP();


        if(this.maxHP < 10)
        {
            throw new System.Exception("最大HPは10以上にしてください");
        }

        //さいだいHPに合わせて、ゲージの長さを延長
        for (int i = 1; i <= this.maxHP - 10; ++i)
        {
            var obj = Instantiate(this.frameImage, this.gameObject.transform);
            obj.SetActive(true);
            obj.transform.position += new Vector3(12.0f * i * aaa, 0.0f, 0.0f);
        }
        //HPメモリを生成
        for (int i = 0; i < this.maxHP; ++i)
        {
            var obj = Instantiate(this.barImage, this.gameObject.transform);
            obj.SetActive(true);
            obj.transform.position += new Vector3(12.0f * i * aaa, 0.0f, 0.0f);
            this.barList.Add(obj);
        }
	}
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i < this.barList.Count; ++i)
        {
            if(this.targetCharacter.GetData().hitPoint.GetHP() <= i)
            {
                this.barList[i].SetActive(false);
            }
            else
            {
                this.barList[i].SetActive(true);
            }
        }
	}
}
