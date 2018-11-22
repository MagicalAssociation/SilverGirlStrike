using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * file     Item.cs
 * brief    Item関連
 * author   Shou Kaneko
 * date     2018/11/21
*/

/**
 * brief    アイテムの基底クラス
 */
namespace SGS
{
    public abstract class Item
    {
        public Item()
        {

        }
        public abstract void Use();
    }

    public abstract class ItemObject : MonoBehaviour
    {
        Item item;
        public abstract void UpdateItem();
        public abstract void Enter();
        public void SetItemData(ref Item item)
        {
            this.item = item;
        }
        public Item GetItemData()
        {
            return this.item;
        }
    }
}