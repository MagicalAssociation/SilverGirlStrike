using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//アイテムデータはStart時に登録を行い、M_Systemにあるデータを読み込む
//リトライ時は外部からセーブ関数を呼びM_Systemに情報を渡す。それを再びStartで読み込む
public class ItemManager : MonoBehaviour
{
    SGS.Item[] itemData;
    //使用者
    public CharacterObject master;
    public enum ItemDirection : int
    {
        UP = 0,
        DOWN = 1,
        LEFT = 2,
        RIGHT = 3,
    }
    // Use this for initialization
    void Start()
    {
        itemData = new SGS.Item[4];
        for(int i = 0;i < itemData.Length;++i)
        {
            itemData[i] = new SGS.Item();
        }
        //アイテムデータをロードする
        //実処理が書いてあるのはItemを継承した先なのでidに応じて生成それをnewしてそこにidやらを渡してやる
        itemData[(int)ItemDirection.UP] = M_System.gameStartItems[(int)M_System.ItemDirection.UP];
        itemData[(int)ItemDirection.DOWN] = M_System.gameStartItems[(int)M_System.ItemDirection.DOWN];
        itemData[(int)ItemDirection.LEFT] = M_System.gameStartItems[(int)M_System.ItemDirection.LEFT];
        itemData[(int)ItemDirection.RIGHT] = M_System.gameStartItems[(int)M_System.ItemDirection.RIGHT];
        for(int i = 0;i < itemData.Length;++i)
        {
            itemData[i].master = master;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(M_System.input.Down(SystemInput.Tag.ITEM_U))
        {
            if (itemData[(int)ItemDirection.UP] != null)
            {
                itemData[(int)ItemDirection.UP].Use();
            }
        }
        else if(M_System.input.Down(SystemInput.Tag.ITEM_D))
        {
            if (itemData[(int)ItemDirection.DOWN] != null)
            {
                itemData[(int)ItemDirection.DOWN].Use();
            }
        }
        else if(M_System.input.Down(SystemInput.Tag.ITEM_L))
        {
            if (itemData[(int)ItemDirection.LEFT] != null)
            {
                itemData[(int)ItemDirection.LEFT].Use();
            }
        }
        else if(M_System.input.Down(SystemInput.Tag.ITEM_R))
        {
            if (itemData[(int)ItemDirection.RIGHT] != null)
            {
                itemData[(int)ItemDirection.RIGHT].Use();
            }
        }
	}
    public void SaveItemData()
    {
        M_System.gameStartItems[(int)M_System.ItemDirection.UP] = itemData[(int)ItemDirection.UP];
        M_System.gameStartItems[(int)M_System.ItemDirection.DOWN] = itemData[(int)ItemDirection.DOWN];
        M_System.gameStartItems[(int)M_System.ItemDirection.LEFT] = itemData[(int)ItemDirection.LEFT];
        M_System.gameStartItems[(int)M_System.ItemDirection.RIGHT] = itemData[(int)ItemDirection.RIGHT];
    }
}
