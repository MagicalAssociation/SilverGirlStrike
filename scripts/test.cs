using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {
	// Use this for initialization
	void Start () {
        Event eventReader = new Event("t/test");
        var text = eventReader.GetEventText();
        Debug.Log(text[0].eventName);
        foreach (var i in text[0].args)
            Debug.Log(i);
    }

    // Update is called once per frame
    void Update () {
		
	}

    //メンバ変数
    public string filePath;
}
