using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultScreen : MonoBehaviour {
    public Text clearTimeText;
    public Text damagePointText;
    public Text clearLevelText;

    bool isShow;

	// Use this for initialization
	void Start () {
        this.clearTimeText.text = "";
        this.damagePointText.text = "";
        this.clearLevelText.text = "";

        this.isShow = false;

        SetClearTime(60 * 100);
        SetDamagePoint(5);
    }
	
	// Update is called once per frame
	void Update () {


	}



    void SetClearTime(int clearTimeSecond)
    {
        int minute = clearTimeSecond / 60;
        int second = clearTimeSecond - minute * 60;

        this.clearTimeText.text = minute.ToString("00") + ":" + second.ToString("00");
    }

    void SetDamagePoint(int damage)
    {
        this.damagePointText.text = damage.ToString("00");
    }
}
