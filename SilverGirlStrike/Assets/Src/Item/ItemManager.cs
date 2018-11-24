using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * file     ItemManager.cs
 * brief    GameScene上に設置されているItemObjectを管理するclass
 * author   Shou Kaneko
 * date     2018/11/24
*/
namespace SGS
{
    public class ItemManager : MonoBehaviour
    {
        //! ItemObjectData
        List<ItemObject> items;
        //! 指定GameObjectの子のItemObjectを取得するための変数
        public GameObject itemChild;
        /**
         * brief    Use this for initialization 
         */
        void Start()
        {
            items = new List<ItemObject>();
            for (int i = 0; i < itemChild.transform.childCount; ++i)
            {
                items.Add(itemChild.transform.GetChild(i).GetComponent<ItemObject>());
            }
        }
        /**
         * brief    Update is called once per frame
         */
        void Update()
        {
            this.GameUpdate();
            this.DestroyCheck();
        }
        /**
         * brief    ItemObjectを新規に登録する
         * param[in] ref ItemObject item 新規ItemObject
         */ 
        public void SetItemObject(ref ItemObject item)
        {
            this.items.Add(item);
        }
        /**
         * brief    ItemObjectデータを取得する
         * return List<ItemObject> 登録されている全ItemObject
         */ 
        public List<ItemObject> GetItemObjects()
        {
            return this.items;
        }
        /**
         * brief    更新処理
         */ 
        private void GameUpdate()
        {
            //通常時のみ更新を行う
            foreach (var item in this.items.ToArray())
            {
                switch (item.GetItemMode())
                {
                    case SGS.ItemMode.NORMAL:
                        item.UpdateItem();
                        break;
                    default:
                        break;
                }
            }
        }
        /**
         * brief    削除確認
         */ 
        private void DestroyCheck()
        {
            //削除指示されているオブジェクトを破棄する
            foreach (var item in this.items.ToArray())
            {
                if (item.GetItemMode() == ItemMode.KILL)
                {
                    Destroy(item.gameObject);
                    items.Remove(item);
                }
            }
        }
    }
}
