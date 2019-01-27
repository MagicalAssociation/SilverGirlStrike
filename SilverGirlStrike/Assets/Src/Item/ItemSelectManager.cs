using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//セーブデータの値を持ってきてそこからアイテムの手持ち数をTextでGUIに出すようにする

public class ItemSelectManager : CursorSystem
{
    //アイテムの背景画像
    public Sprite back;
    //アイテムデータを表示するprefab
    public ItemSelect prefab;
    //アイテム表示の開始位置
    public Transform startPosition;
    //アイテムの表示サイズ
    public Vector2 size;
    //アイテム表示の隙間
    public Vector2 clearance;
    //アイテムを１列にいくつ並べるかの個数
    public int oneRowNumber;
    //選択アイテムと非選択アイテムの色設定
    public SGS.CursorColor cursorColor;
    //setManagerにアイテム情報を送るため
    public ItemSetSelectManager nextManager;
    //アイテムはCanvasの子として登録するので
    public Canvas canvas;
    //アイテムの説明文出す場所
    public Text text;
    //画像データ,ResourceIDとSpriteを登録しておく
    public SGS.Item.ResourceData[] resourceData;
    private List<List<ItemSelect>> itemSelects;
    // Use this for initialization
    void Start()
    {
        //仮処理、save1のデータを読み込んで登録する
        CurrentData.GetDataInstance().SetData(GameData.Load(GameData.GetSaveFilePath()[0]));
        //カーソルの1列分をまとめる親GameObjectを生成する
        string[] texts = SGS.Item.Load();
        //１行の量よりも小さい数しか存在しないなら１行の量を変更する
        if(texts.Length < oneRowNumber)
        {
            oneRowNumber = texts.Length;
        }
        cursors = new GameObject[oneRowNumber];
        itemSelects = new List<List<ItemSelect>>();
        for(int i = 0;i < oneRowNumber;++i)
        {
            cursors[i] = new GameObject();
            //ImageなのでCanvasの子に登録してあげないと画面に映らない
            cursors[i].transform.parent = canvas.transform;
            itemSelects.Add(new List<ItemSelect>());
        }
        //生成と登録
        for(int i = 0;i < texts.Length;++i)
        {
            ItemSelect itemSelect = Object.Instantiate(prefab, new Vector3(
                startPosition.position.x + (GetPosition(i, SGS.Item.GetResourceID(texts[i])).x + GetClearance(i, SGS.Item.GetResourceID(texts[i])).x), 
                startPosition.position.y - (GetPosition(i, SGS.Item.GetResourceID(texts[i])).y + GetClearance(i, SGS.Item.GetResourceID(texts[i])).y)),
                Quaternion.identity) as ItemSelect;
            itemSelect.SetSize(size);
            itemSelect.ItemDataLoad(SGS.Item.GetID(texts[i]));
            itemSelect.backImage.sprite = back;
            itemSelect.itemImage.sprite = SGS.Item.ResourceData.GetSprite(resourceData,itemSelect.GetItem().GetResourceID());
            itemSelect.GetItem().SetSprite(itemSelect.itemImage.sprite);
            itemSelect.itemImage.color = cursorColor.notSelectcImageColor;
            itemSelect.backImage.color = cursorColor.notSelectBackColor;
            //itemSelectの親にカーソルの親を指定する
            itemSelect.transform.parent = cursors[i % oneRowNumber].transform;
            //リストに登録(Colorをあとでも変更可能にするため)
            itemSelects[i % oneRowNumber].Add(itemSelect);
        }
        //継承元の初期化
        base.Init();
        //色変更
        itemSelects[GetPos().x][GetPos().y].SetColor(cursorColor.selectImageColor, cursorColor.selectBackColor);
        //ループ設定変更
        base.SetLoop(false, Direction.Y);
        text.text = itemSelects[GetPos().x][GetPos().y].GetItem().remarks;
    }
	
    public override void SystemUpdate(CursorSystemManager manager)
    {
        if (CursorMoveInput())
        {
            //色変更処理
            itemSelects[GetPos().x][GetPos().y].SetColor(cursorColor.selectImageColor, cursorColor.selectBackColor);
            text.text = itemSelects[GetPos().x][GetPos().y].GetItem().remarks;
        }
        else
        {
            //色変更処理
            itemSelects[GetPos().x][GetPos().y].SetColor(cursorColor.selectImageColor, cursorColor.selectBackColor);
            //アイテムデータが存在するところで決定ボタンを押したら、そのアイテムデータを渡す。
            if (M_System.input.Down(SystemInput.Tag.DECISION) && 
                (itemSelects[GetPos().x][GetPos().y].GetItem() != null &&
                itemSelects[GetPos().x][GetPos().y].GetItem().GetNumver() != 0))
            {
                nextManager.SetItemData(itemSelects[GetPos().x][GetPos().y].GetItem());
                manager.Next((int)ItemSelectManagers.Type.SET);
            }
            else if (M_System.input.Down(SystemInput.Tag.CANCEL))
            {
                //ステージセレクト画面に戻る
            }
            else if(M_System.input.Down(SystemInput.Tag.LSTICK_DOWN) || M_System.input.Down(SystemInput.Tag.LSTICK_UP))
            {
                //ゲームセレクトへ移行
                manager.Next((int)ItemSelectManagers.Type.GAME);
            }
        }
    }
    bool CursorMoveInput()
    {
        if (M_System.input.Down(SystemInput.Tag.LSTICK_UP))
        {
            itemSelects[GetPos().x][GetPos().y].SetColor(cursorColor.notSelectcImageColor, cursorColor.notSelectBackColor);
            return base.Up();
        }
        else if (M_System.input.Down(SystemInput.Tag.LSTICK_DOWN))
        {
            itemSelects[GetPos().x][GetPos().y].SetColor(cursorColor.notSelectcImageColor, cursorColor.notSelectBackColor);
            return base.Down();
        }
        else if(M_System.input.Down(SystemInput.Tag.LSTICK_LEFT))
        {
            itemSelects[GetPos().x][GetPos().y].SetColor(cursorColor.notSelectcImageColor, cursorColor.notSelectBackColor);
            return base.Left();
        }
        else if(M_System.input.Down(SystemInput.Tag.LSTICK_RIGHT))
        {
            itemSelects[GetPos().x][GetPos().y].SetColor(cursorColor.notSelectcImageColor, cursorColor.notSelectBackColor);
            return base.Right();
        }
        return false;
    }
    private Vector2 GetClearance(int num,int id)
    {
        return new Vector2((num % oneRowNumber) * (clearance.x/* / SGS.Item.ResourceData.GetSprite(resourceData, id).pixelsPerUnit*/), (num / oneRowNumber) * (clearance.y/* / SGS.Item.ResourceData.GetSprite(resourceData, id).pixelsPerUnit*/));
    }
    private Vector2 GetPosition(int num,int id)
    {
        return new Vector2((num % oneRowNumber) * (size.x/* / SGS.Item.ResourceData.GetSprite(resourceData, id).pixelsPerUnit*/), (num / oneRowNumber) * (size.y/* / SGS.Item.ResourceData.GetSprite(resourceData, id).pixelsPerUnit*/));
    }
    public override void Enter()
    {
        itemSelects[GetPos().x][GetPos().y].SetColor(cursorColor.selectImageColor, cursorColor.selectBackColor);
        itemSelects[GetPos().x][GetPos().y].TextUpdate();
        for(int x = 0;x < itemSelects.Count;++x)
        {
            for(int y = 0;y < itemSelects[x].Count;++y)
            {
                itemSelects[x][y].TextUpdate();
            }
        }
    }
    public override void Exit()
    {
        itemSelects[GetPos().x][GetPos().y].SetColor(cursorColor.notSelectcImageColor, cursorColor.notSelectBackColor);
    }
}
