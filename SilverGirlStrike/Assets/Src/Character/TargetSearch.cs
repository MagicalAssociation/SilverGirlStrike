using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TargetSearch
{
    //ターゲット検索のONOFF
    public bool enable;
    //ターゲットに指定するレイヤー
    public M_System.LayerName targetLayer;
    //範囲
    public Collider2D searchRange;
    //検索とそのオブジェクトを返す関数
    public GameObject Search()
    {
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        LayerMask layerMask = new LayerMask();
        //レイヤー指定
        layerMask.value = (int)targetLayer;
        contactFilter2D.SetLayerMask(layerMask);
        contactFilter2D.useTriggers = true;
        Collider2D[] hitObject = new Collider2D[1];
        searchRange.enabled = true;
        int length = Physics2D.OverlapCollider(this.searchRange, contactFilter2D, hitObject);
        searchRange.enabled = false;
        //OverlapColliderで検索をかけ、その結果が１以上なら配列の最初のGameObjectを返す、結果が0ならばnullを返す
        return length > 0 ? hitObject[0].gameObject : null;
    }
    public Collider2D[] Searchs()
    {
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        LayerMask layerMask = new LayerMask();
        //レイヤー指定
        layerMask.value = (int)targetLayer;
        contactFilter2D.SetLayerMask(layerMask);
        contactFilter2D.useTriggers = true;
        Collider2D[] hitObject = new Collider2D[50];
        searchRange.enabled = true;
        //OverlapColliderで検索をかけ、その結果を返す
        Physics2D.OverlapCollider(this.searchRange, contactFilter2D, hitObject);
        searchRange.enabled = false;
        return hitObject;
    }
}