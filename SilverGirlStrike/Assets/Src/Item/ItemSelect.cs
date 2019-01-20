﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSelect : CursorParam {
    public SpriteRenderer back;
    public SpriteRenderer itemImg;
    SGS.Item item;
    int num;
    public SGS.Item GetItem()
    {
        return this.item;
    }
    public void ItemDataLoad(int id,int number)
    {
        item = new SGS.Item();
        item.SetData(SGS.Item.Load(id));
        num = number;
    }

    public override void Decision()
    {

    }
    public void SetColor(Color iamge,Color back)
    {
        itemImg.color = iamge;
        this.back.color = back;
    }
}
