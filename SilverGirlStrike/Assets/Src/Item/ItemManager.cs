using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SGS
{
    public class ItemManager : MonoBehaviour
    {
        List<ItemObject> items;
        // Use this for initialization
        void Start()
        {
            items = new List<ItemObject>();
        }

        // Update is called once per frame
        void Update()
        {
            foreach (var item in this.items)
            {
                item.UpdateItem();
            }
        }
        public void SetItemObject(ref ItemObject item)
        {
            this.items.Add(item);
        }
        public List<ItemObject> GetItemObjects()
        {
            return this.items;
        }
    }
}
