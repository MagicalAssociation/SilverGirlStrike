using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//セーブデータの値を持ってきてそこからアイテムの手持ち数をTextでGUIに出すようにする

public class ItemSelectManager : CursorSystem
{
    [System.Serializable]
    public class CursorColor
    {
        public Color selectImageColor;
        public Color notSelectcImageColor;
        public Color selectBackColor;
        public Color notSelectBackColor;
    }
    public Sprite back;
    public ItemSelect prefab;
    public Transform startPosition;
    public Vector2 size;
    public Vector2 clearance;
    public int oneRowNumber;
    public CursorColor cursorColor;
    public ItemSetSelectManager nextManager;
    public SGS.Item.ResourceData[] resourceData;
    private List<List<ItemSelect>> itemSelects;
    // Use this for initialization
    void Start()
    {
        //カーソルの1列分をまとめる親GameObjectを生成する
        string[] texts = SGS.Item.Load();
        if(texts.Length < oneRowNumber)
        {
            oneRowNumber = texts.Length;
        }
        cursors = new GameObject[oneRowNumber];
        itemSelects = new List<List<ItemSelect>>();
        for(int i = 0;i < oneRowNumber;++i)
        {
            cursors[i] = new GameObject();
            itemSelects.Add(new List<ItemSelect>());
        }
        for(int i = 0;i < texts.Length;++i)
        {
            ItemSelect itemSelect = Object.Instantiate(prefab, new Vector3(
                startPosition.position.x + (GetPosition(i, SGS.Item.GetID(texts[i])).x + GetClearance(i, SGS.Item.GetID(texts[i])).x), 
                startPosition.position.y - (GetPosition(i, SGS.Item.GetID(texts[i])).y + GetClearance(i, SGS.Item.GetID(texts[i])).y)),
                Quaternion.identity) as ItemSelect;
            itemSelect.ItemDataLoad(SGS.Item.GetID(texts[i]));
            itemSelect.back.sprite = back;
            itemSelect.itemImg.sprite = SGS.Item.ResourceData.GetSprite(resourceData,itemSelect.GetItem().GetID());
            itemSelect.GetItem().SetSprite(itemSelect.itemImg.sprite);
            itemSelect.itemImg.color = cursorColor.notSelectcImageColor;
            itemSelect.back.color = cursorColor.notSelectBackColor;
            //itemSelectの親にカーソルの親を指定する
            itemSelect.transform.parent = cursors[i % oneRowNumber].transform;
            //リストに登録(Colorをあとでも変更可能にするため)
            itemSelects[i % oneRowNumber].Add(itemSelect);
        }
        base.Init();
        itemSelects[GetNow().x][GetNow().y].SetColor(cursorColor.selectImageColor, cursorColor.selectBackColor);
    }
	
    public override void SystemUpdate(CursorSystemManager manager)
    {
        if (CursorMoveInput())
        {
            //色変更処理
            itemSelects[GetNow().x][GetNow().y].SetColor(cursorColor.selectImageColor, cursorColor.selectBackColor);
        }
        else
        {
            //アイテムデータが存在するところで決定ボタンを押したら、そのアイテムデータを渡す。
            if (itemSelects[GetNow().x][GetNow().y].GetItem() != null && M_System.input.Down(SystemInput.Tag.DECISION))
            {
                nextManager.SetItemData(itemSelects[GetNow().x][GetNow().y].GetItem());
                manager.Next(1);
            }
            else if (M_System.input.Down(SystemInput.Tag.CANCEL))
            {
                //ステージセレクト画面に戻る
            }
        }
    }
    bool CursorMoveInput()
    {
        if (M_System.input.Down(SystemInput.Tag.LSTICK_UP))
        {
            itemSelects[GetNow().x][GetNow().y].SetColor(cursorColor.notSelectcImageColor, cursorColor.notSelectBackColor);
            return base.Up();
        }
        else if (M_System.input.Down(SystemInput.Tag.LSTICK_DOWN))
        {
            itemSelects[GetNow().x][GetNow().y].SetColor(cursorColor.notSelectcImageColor, cursorColor.notSelectBackColor);
            return base.Down();
        }
        else if(M_System.input.Down(SystemInput.Tag.LSTICK_LEFT))
        {
            itemSelects[GetNow().x][GetNow().y].SetColor(cursorColor.notSelectcImageColor, cursorColor.notSelectBackColor);
            return base.Left();
        }
        else if(M_System.input.Down(SystemInput.Tag.LSTICK_RIGHT))
        {
            itemSelects[GetNow().x][GetNow().y].SetColor(cursorColor.notSelectcImageColor, cursorColor.notSelectBackColor);
            return base.Right();
        }
        return false;
    }
    private Vector2 GetClearance(int num,int id)
    {
        return new Vector2((num % oneRowNumber) * (clearance.x / SGS.Item.ResourceData.GetSprite(resourceData, id).pixelsPerUnit), (num / oneRowNumber) * (clearance.y / SGS.Item.ResourceData.GetSprite(resourceData, id).pixelsPerUnit));
    }
    private Vector2 GetPosition(int num,int id)
    {
        return new Vector2((num % oneRowNumber) * (size.x / SGS.Item.ResourceData.GetSprite(resourceData, id).pixelsPerUnit), (num / oneRowNumber) * (size.y / SGS.Item.ResourceData.GetSprite(resourceData, id).pixelsPerUnit));
    }
    public override void Enter()
    {
        itemSelects[GetNow().x][GetNow().y].SetColor(cursorColor.selectImageColor, cursorColor.selectBackColor);
    }
    public override void Exit()
    {
        itemSelects[GetNow().x][GetNow().y].SetColor(cursorColor.notSelectcImageColor, cursorColor.notSelectBackColor);
    }
}
