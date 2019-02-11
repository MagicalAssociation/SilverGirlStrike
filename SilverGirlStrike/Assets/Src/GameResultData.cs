using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResultData : MonoBehaviour {
    [SerializeField]
    string damageResultTargetName;

    CharacterObject.CharaData targetData;
    public float clearTimeCounter;
    int damage;
    int preHitPoint;

    // Use this for initialization
    void Start () {
        this.clearTimeCounter = 0.0f;
        //仕方がないのでFindで取得しておく
        var characterManager = GameObject.Find(M_System.characterManagerObjectName).GetComponent<CharacterManager>();
        this.targetData = characterManager.GetCharacterData("Player");

        this.preHitPoint = 0;
        this.damage = 0;
    }
	
	// Update is called once per frame
	void Update () {
		//前回との比較をもとにHPが減った時、ダメージ値の蓄積を行う
        if(this.preHitPoint > this.targetData.hitPoint.GetHP())
        {
            damage += this.preHitPoint - this.targetData.hitPoint.GetHP();
        }

        this.preHitPoint = this.targetData.hitPoint.GetHP();

        //ターゲットが生きてるときだけタイムを加算
        if (this.preHitPoint <= 0)
        {
            return;
        }
        this.clearTimeCounter += Time.deltaTime;
    }


    public float GetTime()
    {
        return this.clearTimeCounter;
    }
    public int GetDamagePoint()
    {
        return this.damage;
    }

    public void Init()
    {
        //仕方がないのでFindで取得しておく
        var characterManager = GameObject.Find(M_System.characterManagerObjectName).GetComponent<CharacterManager>();
        this.targetData = characterManager.GetCharacterData("Player");
    }
}
