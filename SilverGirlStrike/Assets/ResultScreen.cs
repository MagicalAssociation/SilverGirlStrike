using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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


        var obj = GameObject.Find("RestartEvent");

        SetClearTime((int)obj.GetComponent<GameResultData>().GetTime());
        SetDamagePoint(obj.GetComponent<GameResultData>().GetDamagePoint());

        //クリアタイムを保存する
        int number;
        switch (CurrentData.GetDataInstance().stageName)
        {
            case "GameScene":
                number = 0;
                break;

            case "EnnStage":
                number = 1;
                break;
            default:
                number = 0;
                break;
        }

        CurrentData.GetDataInstance().GetData().stageClearFlag[0] = 1;
        CurrentData.GetDataInstance().GetData().stageClearTime[0] = (int)obj.GetComponent<GameResultData>().GetTime();
        Debug.Log(CurrentData.GetDataInstance().GetData().stageClearTime[0]);
    }
	
	// Update is called once per frame
	void Update () {
        M_System.input.SetEnableStop(SystemInput.Tag.DECISION, false);

        //ボタンを押したら、シーンを変える
        if (M_System.input.Down(SystemInput.Tag.DECISION))
        {
            SceneManager.LoadScene("StageSelect");
        }


	}



    public void SetClearTime(int clearTimeSecond)
    {
        int minute = clearTimeSecond / 60;
        int second = clearTimeSecond - minute * 60;

        this.clearTimeText.text = minute.ToString("00") + ":" + second.ToString("00");
    }

    public void SetDamagePoint(int damage)
    {
        this.damagePointText.text = damage.ToString("00");
    }
}
