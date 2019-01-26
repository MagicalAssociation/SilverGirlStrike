using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SetSelect : CursorParam
{
    SGS.Item item;
    public Image itemImage;
    public Image backImage;
    public override void Decision()
    {
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
            item.SetNumver(item.GetNumver() - 1);
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
    public SGS.Item GetItem()
    {
        return this.item;
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