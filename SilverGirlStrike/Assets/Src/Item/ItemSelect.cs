using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemSelect : CursorParam {
    //public SpriteRenderer back;
    //public SpriteRenderer itemImg;
    public Image backImage;
    public Image itemImage;
    public Text number;
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
        item.SetNumver(CurrentData.GetDataInstance().GetData().GetItemNumber(id));
        TextUpdate();
    }
    public void SetSize(Vector2 size)
    {
        backImage.rectTransform.sizeDelta = size;
        itemImage.rectTransform.sizeDelta = size;
    }

    public void TextUpdate()
    {
        number.text = item.GetNumver().ToString();
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
