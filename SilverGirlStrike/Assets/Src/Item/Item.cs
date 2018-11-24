using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * file     Item.cs
 * brief    Item関連
 * author   Shou Kaneko
 * date     2018/11/24
*/

/**
 * brief    アイテムの基底クラス
 */
namespace SGS
{
    /**
     * enum ItemMode
     * brief    状態管理を行う定数
     */ 
    public enum ItemMode
    {
        //! Destroy
        KILL,
        //! Normal
        NORMAL,
        //! Stop Update
        STOP,
    }
    /**
     * brief アイテムの基底クラス
     */ 
    public abstract class Item
    {
        /**
         * brief    constructor
         */ 
        public Item()
        {

        }
        /**
         * brief    使用時の効果を記述
         */ 
        public abstract void Use();
    }
    /**
     * brief    設置するItem
     * これをコンポーネントしているGameObjectにはBoxCollider2Dをいれておくこと
     */
    public abstract class ItemObject : MonoBehaviour
    {
        //! 使用時効果等ある場合のための変数
        Item item;
        //! 状態管理
        ItemMode mode;
        //! 当たり判定を行うためのCollider
        BoxCollider2D collider;
        /**
         * brief    constructor
         */
        public ItemObject()
        {
            this.mode = ItemMode.NORMAL;
            collider = null;
        }
        /**
         * brief    constructor
         * param[in] ref Item item アイテムデータを登録
         */
        public ItemObject(ref Item item)
        {
            this.mode = ItemMode.NORMAL;
            //collider = GetComponent<BoxCollider2D>();
            this.item = item;
        }
        /**
         * brief    更新処理
         * フィールド上で動きを付ける時などに使用
         */
        public abstract void UpdateItem();
        /**
         * brief    当たった時の動作を記述する
         */
        public abstract void Enter();
        /**
         * brief    アイテムデータを登録する
         * param[in] ref Item item アイテムデータ
         */
        public void SetItemData(ref Item item)
        {
            this.item = item;
        }
        /**
         * brief    アイテムデータを取得する
         * return Item 登録されているアイテムデータ
         */
        public Item GetItemData()
        {
            return this.item;
        }
        /**
         * brief    状態を取得
         * return ItemMode Mode
         */
        public ItemMode GetItemMode()
        {
            return this.mode;
        }
        /**
         * brief    状態を変更する
         * param[in] ItemMode mode 指定Mode
         */
        public void ChangeMode(ItemMode mode)
        {
            this.mode = mode;
        }
        /**
         * brief    Colliderを登録する
         * param[in] ref BoxCollider2D collider
         */
         public void SetCollider(BoxCollider2D box)
        {
            this.collider = box;
        }
        /**
         * brief    当たり判定
         */
        public Collider2D HitCheck()
        {
            if (this.collider)
            {
                Collider2D hit = Physics2D.OverlapBox(this.collider.transform.position, this.collider.size,this.transform.eulerAngles.z, (int)M_System.LayerName.PLAYER);
                return hit;
            }
            return null;
        }
    }
}