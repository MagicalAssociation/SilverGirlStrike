using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foot : MonoBehaviour{
    public bool isFoot;

    public BoxCollider2D boxColloder;
    // Use this for initialization
    private void Start()
    {
        this.boxColloder = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
    }

    public bool CheckHit()
    {
        isFoot = (Physics2D.OverlapBox(
            boxColloder.transform.position + new Vector3(boxColloder.offset.x, boxColloder.offset.y, 0),
            new Vector2(boxColloder.size.x, boxColloder.size.y) * 0.5f,
            this.boxColloder.transform.eulerAngles.z,
            (int)M_System.LayerName.GROUND) == null) ? false : true;
        return this.isFoot;
    }

    public void LineDraw()
    {
        Vector2 bl = (Vector2)this.transform.position;
        bl += boxColloder.offset + Vector2.left * boxColloder.size.x * 0.5f + Vector2.down * boxColloder.size.y * 0.5f;
        Vector2 tr = (Vector2)this.transform.position;
        tr += boxColloder.offset + Vector2.right * boxColloder.size.x * 0.5f + Vector2.up * boxColloder.size.y * 0.5f;

        Debug.DrawRay(bl, tr - bl, Color.green);

    }
}
