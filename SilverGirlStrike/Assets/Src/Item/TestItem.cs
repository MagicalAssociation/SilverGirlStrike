using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestItem : SGS.ItemObject
{
    private void Start()
    {

    }
    public override void Enter(GameObject hitObject)
    {
        if (hitObject.tag == "Player")
        {
            hitObject.GetComponent<CharacterObject>().GetData().hitPoint.Recover(5);
            this.Destory();
        }
    }
    public override void UpdateItem()
    {

    }
}