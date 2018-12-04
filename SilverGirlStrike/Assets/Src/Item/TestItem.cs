using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestItem : SGS.ItemObject
{
    private void Start()
    {
        this.SetCollider(GetComponent<BoxCollider2D>());
    }
    public override void Enter()
    {
    }
    public override void UpdateItem()
    {
        var hit = this.HitCheck();
        if (hit)
        {
            if (hit.tag == "Player")
            {
                hit.GetComponent<CharacterObject>().GetData().hitPoint.Recover(5);
                this.ChangeMode(SGS.ItemMode.KILL);
            }
        }
    }
}
