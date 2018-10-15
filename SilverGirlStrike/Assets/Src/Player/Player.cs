using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public SpriteRenderer sprite;
    public Rigidbody2D rigidbody;
	// Use this for initialization
	void Start () {
        rigidbody = this.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        //this.rigidbody.velocity = new Vector2(Input.GetAxis("RStickX") * 10.0f, 0.0f);
        this.rigidbody.velocity = new Vector2(Input.GetAxis("RStickX") * 2.0f, (9.8f / 60.0f / 60.0f * 32.0f) * -10.0f);

    }
    /**
     * 重力処理
     */
    private void Function()
    {
    }
}
