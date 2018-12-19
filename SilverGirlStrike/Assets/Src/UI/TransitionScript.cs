using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//シェーダーに値を渡していい感じの遷移アニメーションを実現するスクリプト
[ExecuteInEditMode()]
public class TransitionScript : MonoBehaviour {

    public Shader shader;
    public Texture ruleTexture;
    Graphic img;
    Material mat;

    [Range(0.0f, 1.0f)]
    public float border;

	// Use this for initialization
	void Start () {
        this.img = GetComponent<Graphic>();
        this.mat = new Material(this.shader);
        this.mat.SetTexture("_RuleTex", ruleTexture);

        this.img.material = this.mat;

    }
	
	// Update is called once per frame
	void Update () {
        this.mat.SetFloat("_Border", this.border);
    }
}
