using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSelect : CursorParam
{
    SGS.Item item;
    public SpriteRenderer imageSprite;
    public SpriteRenderer backSprite;
    public M_System.ItemDirection direction;
    public override void Decision()
    {
        M_System.gameStartItems[(int)direction] = this.item;
    }
    public void SetItemData(SGS.Item item)
    {
        this.item = item;
        if (this.item == null)
        {
            this.imageSprite.sprite = null;
        }
        else
        {
            this.imageSprite.sprite = item.GetSprite();
        }
    }
    public void SetColor(Color image,Color back)
    {
        imageSprite.color = image;
        backSprite.color = back;
    }
}