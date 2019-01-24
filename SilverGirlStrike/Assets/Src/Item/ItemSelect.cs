using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemSelect : CursorParam {
    //public SpriteRenderer back;
    //public SpriteRenderer itemImg;
    public Image backImage;
    public Image itemImage;
    SGS.Item item;
    int num;
    public SGS.Item GetItem()
    {
        return this.item;
    }
    public void ItemDataLoad(int id)
    {
        item = new SGS.Item();
        item.SetData(SGS.Item.Load(id));
        M_System.currentData.SetData(new Save.DataParameter());
        num = M_System.currentData.GetData().GetItemNumber(id);
    }
    public void SetSize(Vector2 size)
    {
        backImage.rectTransform.sizeDelta = size;
        itemImage.rectTransform.sizeDelta = size;
    }

    public override void Decision()
    {

    }
    public void SetColor(Color image,Color back)
    {
        //itemImg.color = iamge;
        //this.back.color = back;
        itemImage.color = image;
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
