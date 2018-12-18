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
    List<GameObject> hpMemoryList;
    //HPバー配列
    List<GameObject> barList;

    // Use this for initialization
    void Start () {
        this.hpMemoryList = new List<GameObject>();
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
            this.barList.Add(obj);
        }
        //HPメモリを生成
        for (int i = 0; i < this.maxHP; ++i)
        {
            var obj = Instantiate(this.barImage, this.gameObject.transform);
            obj.SetActive(true);
            obj.transform.position += new Vector3(12.0f * i * aaa, 0.0f, 0.0f);
            this.hpMemoryList.Add(obj);
        }
	}
	
	// Update is called once per frame
	void Update () {
        UpdateHPBar(this.targetCharacter.GetData().hitPoint.GetMaxHP());

        for (int i = 0; i < this.hpMemoryList.Count; ++i)
        {
            if(this.targetCharacter.GetData().hitPoint.GetHP() <= i)
            {
                this.hpMemoryList[i].SetActive(false);
            }
            else
            {
                this.hpMemoryList[i].SetActive(true);
            }
        }
	}


    private void UpdateHPBar(int maxHP)
    {
        //想定画面サイズに対しての倍率
        float aaa = Screen.width / 960.0f;

        //さいだいHPに合わせて、ゲージの長さを延長
        if (barList.Count < maxHP - 10)
        {
            for (int i = barList.Count + 1; i <= maxHP - 10; ++i)
            {
                var obj = Instantiate(this.frameImage, this.frameImage.transform.parent);
                obj.SetActive(true);
                obj.transform.position += new Vector3(12.0f * i * aaa, 0.0f, 0.0f);
                this.barList.Add(obj);
            }
        }
        if (barList.Count > maxHP - 10)
        {
            for (int i = maxHP - 10; i <= barList.Count; ++i)
            {
                Destroy(this.barList[i]);
                this.barList[i] = null;
            }
            this.barList.RemoveRange(this.maxHP, maxHP - this.maxHP);
        }

        //HPメモリを生成
        for (int i = this.hpMemoryList.Count; i < maxHP; ++i)
        {
            var obj = Instantiate(this.barImage, this.gameObject.transform);
            obj.SetActive(true);
            obj.transform.position += new Vector3(12.0f * i * aaa, 0.0f, 0.0f);
            this.hpMemoryList.Add(obj);
        }


        this.maxHP = maxHP;
    }
}
