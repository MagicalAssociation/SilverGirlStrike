﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elixir
{
    public class Item : SGS.Item
    {
        public int recoverValue;
        public override void Init()
        {
            try {
                this.recoverValue = int.Parse(SGS.Item.Load(this.GetID())[(int)Item.Type.TYPE_NUM]);
            }
            catch
            {
                this.recoverValue = 0;
            }
        }
        public Item()
        {

        }
        public Item(SGS.Item item) : base(item)
        {
        }
        public override void Use()
        {
            //使用者のHPを回復する
            if (master == null || base.GetNumver() == 0)
            {
                return;
            }
            master.GetData().hitPoint.Recover(recoverValue);
            base.SetNumver(base.GetNumver() - 1);
        }
    }
    public class Elixir : SGS.ItemObject
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
