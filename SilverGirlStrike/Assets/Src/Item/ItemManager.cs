using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//アイテムデータはStart時に登録を行い、M_Systemにあるデータを読み込む
//リトライ時は外部からセーブ関数を呼びM_Systemに情報を渡す。それを再びStartで読み込む
public class ItemManager : MonoBehaviour
{
    public enum ItemTag
    {
        Elixir1 = 0,
        Elixir2 = 1,
        Elixir3 = 2,
        Star1 = 3,
    }
    SGS.Item[] itemData;
    //使用者
    public CharacterObject master;
    // Use this for initialization
    void Start()
    {
        itemData = new SGS.Item[4];
        //アイテムデータをロードする
        //実処理が書いてあるのはItemを継承した先なのでidに応じて生成それをnewしてそこにidやらを渡してやる
        for (int i = 0; i < itemData.Length; ++i)
        {
            CreateItemData(i, CurrentData.GetItemData(i));
            if (itemData[i] != null)
            {
                itemData[i].master = master;
                itemData[i].Init();
            }
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(M_System.input.Down(SystemInput.Tag.ITEM_U))
        {
            Debug.Log("ITEM_U");
            if (itemData[(int)CurrentData.ItemDirection.UP] != null)
            {
                itemData[(int)CurrentData.ItemDirection.UP].Use();
            }
        }
        else if(M_System.input.Down(SystemInput.Tag.ITEM_D))
        {
            Debug.Log("ITEM_D");
            if (itemData[(int)CurrentData.ItemDirection.DOWN] != null)
            {
                itemData[(int)CurrentData.ItemDirection.DOWN].Use();
            }
        }
        else if(M_System.input.Down(SystemInput.Tag.ITEM_L))
        {
            Debug.Log("ITEM_L");
            if (itemData[(int)CurrentData.ItemDirection.LEFT] != null)
            {
                itemData[(int)CurrentData.ItemDirection.LEFT].Use();
            }
        }
        else if(M_System.input.Down(SystemInput.Tag.ITEM_R))
        {
            Debug.Log("ITEM_R");
            if (itemData[(int)CurrentData.ItemDirection.RIGHT] != null)
            {
                itemData[(int)CurrentData.ItemDirection.RIGHT].Use();
            }
        }
	}
    public void SaveItemData()
    {
        CurrentData.SetItemData((int)CurrentData.ItemDirection.UP, itemData[(int)CurrentData.ItemDirection.UP]);
        CurrentData.SetItemData((int)CurrentData.ItemDirection.DOWN, itemData[(int)CurrentData.ItemDirection.DOWN]);
        CurrentData.SetItemData((int)CurrentData.ItemDirection.LEFT, itemData[(int)CurrentData.ItemDirection.LEFT]);
        CurrentData.SetItemData((int)CurrentData.ItemDirection.RIGHT, itemData[(int)CurrentData.ItemDirection.RIGHT]);
    }

    private void CreateItemData(int num,SGS.Item loaditem)
    {
        if (loaditem == null) 
        {
            return;
        }
        switch(loaditem.GetID())
        {
            case (int)ItemTag.Elixir1:
            case (int)ItemTag.Elixir2:
            case (int)ItemTag.Elixir3:
                itemData[num] = new Elixir.Item(loaditem);
                break;
            case (int)ItemTag.Star1:
                itemData[num] = new Star.Item(loaditem);
                break;
        }
    }
}
