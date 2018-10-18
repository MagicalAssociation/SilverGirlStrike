using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PhysicsObject : MonoBehaviour
{
    public Vector2 move;
    public Vector2 speed;
    //public Collider2D collider;
    //public Rigidbody2D rigidbody;
    public float GRAVITY = 0.03f;
    public float FINSPEED = 0.03f;
    public float prePos;
    void Start()
    {
        //collider = GetComponent<Collider2D>();
        //rigidbody = GetComponent<Rigidbody2D>();
        this.move = new Vector2();
        this.speed = new Vector2();
        this.prePos = 0.0f;
    }
    // Update is called once per frame
    void Update()
    {
        //!Test!入力判定
        this.move += new Vector2(Input.GetAxis("RStickX"), 0.0f);
        //this.transform.position += new Vector3(Input.GetAxis("RStickX"), Input.GetAxis("RStickY"), 0.0f);
        //重力処理
        this.GravityUpdate();
        //移動処理
        this.MoveUpdate();
        //実際の移動
        this.MoveObject();
        //めりこみ処理
        this.Extrusion();
        this.move = new Vector2(0.0f, 0.0f);
    }
    void MoveUpdate()
    {
        this.speed.x = Mathf.Max(this.move.x, this.speed.x);
        this.speed.y = Mathf.Max(this.move.y, this.speed.y);
    }
    void GravityUpdate()
    {
        this.speed.y += GRAVITY;
    }
    void MoveObject()
    {
        //var ray = this.transform.TransformDirection(Vector3.forward);
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position + new Vector3(0.0f,1.1f / 2.0f * -1.0f,0.0f), new Vector2(0.0f, this.speed.y * -1), this.speed.y * -1, (int)M_System.LayerName.GROUND);
        if(hit.collider != null)
        {
            //Debug.Log("Hit" + hit.collider.tag);
        }
        else
        {
            //Debug.Log("NotHit");
            //this.transform.localPosition += new Vector3(0.0f, this.speed.y * -1, 0.0f);
            Debug.Log(this.transform.localPosition.y - this.prePos);
            this.GetComponent<Rigidbody2D>().velocity = new Vector3(0.0f, this.speed.y * -15.0f , 0.0f);
            this.prePos = this.transform.localPosition.y;
        }
    }
    void Extrusion()
    {

    }
}