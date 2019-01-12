using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSelectManager : MonoBehaviour {
    
	// Use this for initialization
	void Start ()
    {
        LoadText[] child;
        child = GetComponentsInChildren<LoadText>();
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
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
