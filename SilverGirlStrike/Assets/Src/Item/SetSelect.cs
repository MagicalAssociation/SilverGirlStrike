using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSelect : CursorParam
{
    SGS.Item item;
    public SpriteRenderer imageSprite;
    public SpriteRenderer backSprite;
    public override void Decision()
    {

    }
    public void SetItemData(SGS.Item item)
    {
        this.item = item;
    }
    public void SetColor(Color image,Color back)
    {
        imageSprite.color = image;
        backSprite.color = back;
    }
}
