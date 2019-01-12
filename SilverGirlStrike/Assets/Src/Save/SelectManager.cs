using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectManager : MonoBehaviour {

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
                text = GameData.GetSaveFilePath()[i] + "\ngold " + data.gold;
            }
            else
            {
                text = "NO DATA";
            }
            child[i].TextChange(text);
            child[i].SetData(data);
        }
    }

	// Update is called once per frame
	void Update () {
		
	}
}
