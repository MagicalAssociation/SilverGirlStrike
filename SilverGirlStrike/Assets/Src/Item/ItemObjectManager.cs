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
    public class ItemObjectManager : MonoBehaviour
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
            Add();
        }
        /**
         * brief    Update is called once per frame
         */
        void Update()
        {
            this.GameUpdate();
            this.HitCheck();
            this.DestroyCheck();
        }
        /**
         * brief    ItemObjectを新規に登録する
         * param[in] ref ItemObject item 新規ItemObject
         */ 
        public void SetItemObject(ItemObject item)
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
        private void HitCheck()
        {
            foreach (var item in this.items)
            {
                for (int i = 0; i < item.targets.Length; ++i)
                {
                    ContactFilter2D contactFilter2D = new ContactFilter2D();
                    LayerMask layerMask = new LayerMask();
                    //レイヤー指定
                    layerMask.value = (int)item.targets[i];
                    contactFilter2D.SetLayerMask(layerMask);
                    contactFilter2D.useTriggers = true;
                    Collider2D[] hitObject = new Collider2D[1];
                    item.collider.enabled = true;
                    int length = Physics2D.OverlapCollider(item.collider, contactFilter2D, hitObject);
                    item.collider.enabled = false;
                    //OverlapColliderで検索をかけ、その結果が１以上なら配列の最初のGameObjectを返す、結果が0ならばnullを返す
                    if(length > 0)
                    {
                        item.Enter(hitObject[i].gameObject);
                        break;
                    }
                }
            }
        }
        private void Add()
        {
            if(itemChild == null)
            {
                return;
            }
            for (int i = 0; i < itemChild.transform.childCount; ++i)
            {
                if (itemChild.gameObject.activeSelf == true)
                {
                    items.Add(itemChild.transform.GetChild(i).GetComponent<ItemObject>());
                }
            }
        }
    }
}
