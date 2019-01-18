﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        cursors = new GameObject[oneRowNumber];
        itemSelects = new List<List<ItemSelect>>();
        for(int i = 0;i < oneRowNumber;++i)
        {
            cursors[i] = new GameObject();
            itemSelects.Add(new List<ItemSelect>());
        }
        for (int i = 0; i < resourceData.Length; ++i)
        {
            //Objectの生成と各値の登録
            //リソースデータの上から順番に生成
            //1列に出す数とピクセルサイズ/PixelsPerUnitをかけて隙間のない位置を指定する
            ItemSelect itemSelect = Object.Instantiate(prefab, new Vector3(startPosition.position.x + (GetPosition(i).x + GetClearance(i).x), startPosition.position.y - (GetPosition(i).y + GetClearance(i).y), 0), Quaternion.identity) as ItemSelect;
            itemSelect.ItemDataLoad(resourceData[i].id);
            itemSelect.back.sprite = back;
            itemSelect.itemImg.sprite = resourceData[i].sprite;
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
	
	// Update is called once per frame
	void Update () {
        if (CursorMoveInput())
        {
            //色変更処理
            itemSelects[GetNow().x][GetNow().y].SetColor(cursorColor.selectImageColor, cursorColor.selectBackColor);
        }
        else
        {
            //アイテムデータが存在するところで決定ボタンを押したら、そのアイテムデータを渡す。
            if(itemSelects[GetNow().x][GetNow().y].GetItem() != null && M_System.input.Down(SystemInput.Tag.DECISION) && GetEnable() == true)
            {
                nextManager.SetItemData(itemSelects[GetNow().x][GetNow().y].GetItem());
                this.SetEnable(false);
                nextManager.SetEnable(true);
            }
            else if(M_System.input.Down(SystemInput.Tag.CANCEL) && GetEnable() == true)
            {
                //ステージセレクト画面に戻る
            }
        }
    }
    bool CursorMoveInput()
    {
        if (M_System.input.Down(SystemInput.Tag.LSTICK_UP) && GetEnable() == true)
        {
            itemSelects[GetNow().x][GetNow().y].SetColor(cursorColor.notSelectcImageColor, cursorColor.notSelectBackColor);
            return base.Up();
        }
        else if (M_System.input.Down(SystemInput.Tag.LSTICK_DOWN) && GetEnable() == true)
        {
            itemSelects[GetNow().x][GetNow().y].SetColor(cursorColor.notSelectcImageColor, cursorColor.notSelectBackColor);
            return base.Down();
        }
        else if(M_System.input.Down(SystemInput.Tag.LSTICK_LEFT) && GetEnable() == true)
        {
            itemSelects[GetNow().x][GetNow().y].SetColor(cursorColor.notSelectcImageColor, cursorColor.notSelectBackColor);
            return base.Left();
        }
        else if(M_System.input.Down(SystemInput.Tag.LSTICK_RIGHT) && GetEnable() == true)
        {
            itemSelects[GetNow().x][GetNow().y].SetColor(cursorColor.notSelectcImageColor, cursorColor.notSelectBackColor);
            return base.Right();
        }
        return false;
    }
    private Vector2 GetClearance(int num)
    {
        return new Vector2((num % oneRowNumber) * (clearance.x / resourceData[num].sprite.pixelsPerUnit), (num / oneRowNumber) * (clearance.y / resourceData[num].sprite.pixelsPerUnit));
    }
    private Vector2 GetPosition(int num)
    {
        return new Vector2((num % oneRowNumber) * (size.x / resourceData[num].sprite.pixelsPerUnit), (num / oneRowNumber) * (size.y / resourceData[num].sprite.pixelsPerUnit));
    }
}
