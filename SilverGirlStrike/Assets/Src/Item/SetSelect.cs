using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SetSelect : CursorParam
{
    SGS.Item item;
    //public SpriteRenderer imageSprite;
    //public SpriteRenderer backSprite;
    public Image itemImage;
    public Image backImage;
    public enum Type
    {
        IMAGE,
        SPRITE,
    }
    public Type type;
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
            this.itemImage.sprite = null;
            this.itemImage.color = new Color(0, 0, 0, 0);
        }
        else
        {
            this.itemImage.sprite = item.GetSprite();
            this.itemImage.color = new Color(1, 1, 1, 1);
        }
    }
    public void SetColor(Color image,Color back)
    {
        if (itemImage.sprite != null)
        {
            itemImage.color = image;
        }
        backImage.color = back;
    }

    public override void Enter()
    {
        throw new System.NotImplementedException();
    }

    public override void Exit()
    {
        throw new System.NotImplementedException();
    }
}