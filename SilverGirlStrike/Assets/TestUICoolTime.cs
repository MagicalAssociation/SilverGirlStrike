using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestUICoolTime : MonoBehaviour {

    public Fuchan.PlayerObject player;
    Image image;


	// Use this for initialization
	void Start () {
        this.image = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
        float ratio = (float)player.param.strikeAsultRestTime / player.inspectorParam.strikeAsultCoolTime;
        this.image.transform.localScale = new Vector3(ratio, 1.0f, 1.0f);

    }
}
