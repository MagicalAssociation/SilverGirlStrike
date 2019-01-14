using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Star
{
    public class Item : SGS.Item
    {
        public int invincible;
        public override void Use()
        {
            //使用者のHPを回復する
            if (master == null)
            {
                return;
            }
            master.GetData().hitPoint.SetInvincible(invincible);
        }
    }
    public class Star : SGS.ItemObject
    {
        Item item;
        private void Start()
        {
            item = new Item();
            item.SetData(SGS.Item.Load(id));
            try
            {
                item.invincible = int.Parse(SGS.Item.Load(id)[4]);
            }
            catch
            {
                item.invincible = 0;
            }
            base.Init(item);
        }
        public override void Enter(GameObject hitObject)
        {
            if (hitObject.tag == "Player")
            {
                //アイテムデータの使用者に当たったObjectを登録して使用
                base.GetItemData().master = hitObject.GetComponent<CharacterObject>();
                base.GetItemData().Use();
                base.Destory();
            }
        }
        public override void UpdateItem()
        {
        }
    }
}
