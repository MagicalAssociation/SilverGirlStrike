using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSelectManager : MonoBehaviour {
    
	// Use this for initialization
	void Start ()
    {
        Select[] child;
        child = GetComponentsInChildren<Select>();
        for(int i = 0;i < child.Length;++i)
        {
            Save.DataParameter data = GameData.Load(GameData.GetSaveFilePath()[i]);
            string text;
            if (data != null)
            {
                text = GameData.GetSaveFilePath()[i] + "\nmoney " + GameData.Load(GameData.GetSaveFilePath()[i]).gold.ToString();
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
