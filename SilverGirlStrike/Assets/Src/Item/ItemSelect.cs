using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSelect : MonoBehaviour {
    public SpriteRenderer back;
    public SpriteRenderer itemImg;
    SGS.Item item;
	// Use this for initialization
	void Start ()
    {
        item = new SGS.Item();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public SGS.Item GetItem()
    {
        return this.item;
    }
    public void ItemDataLoad(int id)
    {
        item = new SGS.Item();
        item.SetData(SGS.Item.Load(id));
    }
}
