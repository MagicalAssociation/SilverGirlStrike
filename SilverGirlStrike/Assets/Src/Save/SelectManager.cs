using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectManager : MonoBehaviour
{
    [System.Serializable]
    public class FlashingBackGround
    {
        public Image back;
        public Color minColor;
        public Color maxColor;
        private int count;
        public FlashingBackGround()
        {
            count = 0;
        }
        public void Update()
        {
            back.color = Color.Lerp(minColor, maxColor, Mathf.Sin((((count + 270) * Mathf.PI) / 180.0f) + 1) / 2);
            ++count;
        }
    }
    //public FlashingBackGround flashing;
    Select[] child;
    // Use this for initialization
    void Start ()
    {
        child = GetComponentsInChildren<Select>();
        this.GameDataUpdate();
	}
	
    public void GameDataUpdate()
    {
        for (int i = 0; i < child.Length; ++i)
        {
            Save.DataParameter data = GameData.Load(GameData.GetSaveFilePath()[i]);
            string text;
            if (data != null)
            {
                int stageClearNumber = 0;
                for (int j = 0; j < data.stageClearFlag.Length; ++j)
                {
                    if (data.stageClearFlag[j] > 0)
                    {
                        ++stageClearNumber;
                    }
                }
                text = GameData.GetSaveFilePath()[i] + "\n Number " + stageClearNumber.ToString();
            }
        
            else
            {
                text = "NO DATA";
            }
            child[i].TextChange(text);
            child[i].SetData(data);
            child[i].SetStageNumber(i);
        }
    }

	// Update is called once per frame
	void Update ()
    {
        //flashing.Update();	
	}
}
