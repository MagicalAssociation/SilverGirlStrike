using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeScreen : MonoBehaviour {
    public Easing.Type easingType;
    Easing easing;

    public UnityEngine.UI.Image image;
    public Easing.Type colorEasingType;
    Easing colorEasing;

    public int fadeFrameTime;

    FadeScreen()
    {
        this.easing = new Easing();
        this.colorEasing = new Easing();
    }


	// Use this for initialization
	void Start () {
        FadeIn();
        this.easing.Use(this.easingType);
        this.colorEasing.Use(this.colorEasingType);
        this.image.color = new Color(this.image.color.r, this.image.color.g, this.image.color.b, 0.0f);
    }

    // Update is called once per frame
    void Update () {
        this.transform.localScale = new Vector3(this.transform.localScale.x, this.easing.Out());
        this.image.color = new Color(this.image.color.r, this.image.color.g, this.image.color.b, this.colorEasing.Out());
	}



    public void FadeIn()
    {
        this.easing.ResetTime();
        this.easing.Set(1.0f, 0.0f, this.fadeFrameTime * 0.1f);
        this.colorEasing.ResetTime();
        this.colorEasing.Set(1.0f, 0.0f, this.fadeFrameTime * 0.1f);
    }
    public void FadeOut()
    {
        this.easing.ResetTime();
        this.easing.Set(0.0f, 1.0f, this.fadeFrameTime * 0.1f);
        this.colorEasing.ResetTime();
        this.colorEasing.Set(0.0f, 1.0f, this.fadeFrameTime * 0.1f);
    }

}
