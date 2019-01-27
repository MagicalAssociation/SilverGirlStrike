using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Star
{
    public class Item : SGS.Item
    {
        public int invincible;
        public Item()
        {

        }
        public Item(SGS.Item item) : base(item)
        {
        }
        public override void Init()
        {
            try
            {
                invincible = int.Parse(SGS.Item.Load(this.GetID())[4]);
            }
            catch
            {
                invincible = 0;
            }
        }
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
        private void Start()
        {

            Item item = new Item();
            item.SetData(SGS.Item.Load(id));
            item.Init();
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
