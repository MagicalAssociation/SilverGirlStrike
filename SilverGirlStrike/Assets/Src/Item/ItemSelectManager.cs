using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSelectManager : MonoBehaviour {
    public Sprite back;
    public ItemSelect prefab;
    public SGS.Item.ResourceData[] resourceData;
	// Use this for initialization
	void Start ()
    {
        for(int i = 0;i < resourceData.Length;++i)
        {
            ItemSelect itemSelect = Object.Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity) as ItemSelect;
            itemSelect.ItemDataLoad(resourceData[i].id);
            itemSelect.back.sprite = back;
            itemSelect.itemImg.sprite = resourceData[i].sprite;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
