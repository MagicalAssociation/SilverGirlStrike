using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foot : MonoBehaviour{
    public bool isFoot;

    public BoxCollider2D collider;
    // Use this for initialization
    private void Start()
    {
        this.collider = GetComponent<BoxCollider2D>();
    }

    public bool CheckHit()
    {
        return (Physics2D.OverlapBox(
            collider.transform.position + new Vector3(collider.offset.x, collider.offset.y, 0),
            new Vector3(collider.size.x, collider.size.y, 0),
            this.collider.transform.eulerAngles.z,
            (int)M_System.LayerName.GROUND) == null) ? false : true;
    }

    public void LineDraw()
    {
        Debug.DrawLine(
           collider.transform.position + new Vector3(collider.offset.x, collider.offset.y, 0) + new Vector3(collider.size.x / 2.0f, collider.size.y / 2.0f, 0),
           collider.transform.position + new Vector3(collider.offset.x, collider.offset.y, 0) - new Vector3(collider.size.x / 2.0f, collider.size.y / 2.0f, 0)
           );
    }
}
