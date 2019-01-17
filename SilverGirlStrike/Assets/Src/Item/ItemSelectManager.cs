using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSelectManager : MonoBehaviour {
    public Sprite back;
    public ItemSelect prefab;
    public Transform startPosition;
    public Vector2 size;
    public Vector2 clearance;
    public int oneRowNumber;
    public SGS.Item.ResourceData[] resourceData;
    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < resourceData.Length; ++i)
        {
            //Objectの生成と各値の登録
            //リソースデータの上から順番に生成
            //1列に出す数とピクセルサイズ/PixelsPerUnitをかけて隙間のない位置を指定する
            ItemSelect itemSelect = Object.Instantiate(prefab, new Vector3(startPosition.position.x + (GetPosition(i).x + GetClearance(i).x), startPosition.position.y - (GetPosition(i).y + GetClearance(i).y), 0), Quaternion.identity) as ItemSelect;
            itemSelect.ItemDataLoad(resourceData[i].id);
            itemSelect.back.sprite = back;
            itemSelect.itemImg.sprite = resourceData[i].sprite;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    private Vector2 GetClearance(int num)
    {
        return new Vector2((num % oneRowNumber) * (clearance.x / resourceData[num].sprite.pixelsPerUnit), (num / oneRowNumber) * (clearance.y / resourceData[num].sprite.pixelsPerUnit));
    }
    private Vector2 GetPosition(int num)
    {
        return new Vector2((num % oneRowNumber) * (size.x / resourceData[num].sprite.pixelsPerUnit), (num / oneRowNumber) * (size.y / resourceData[num].sprite.pixelsPerUnit));
    }
}
