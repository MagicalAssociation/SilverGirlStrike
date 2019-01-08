using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TextEvent
{
    //イベントで扱うコリジョンを管理するクラス
    //主にGetComponent<EventCollision>()でアクセスして確実にコリジョンを得る方法
    public class EventCollision : MonoBehaviour
    {

        Collider2D[] hitResult;
        private Collider2D collision;

        // Use this for initialization
        void Start()
        {
            this.collision = GetComponent<Collider2D>();
            //当たってるかどうかだけほしいので１つ
            this.hitResult = new Collider2D[1];

            Debug.Log(this.collision);
        }

        //コリジョンを獲得
        public Collider2D GetCollision()
        {
            return this.collision;
        }

        //コリジョンに当たっているかをチェック
        public bool IsHit(int layerMask)
        {
            LayerMask layer = new LayerMask();
            layer.value = layerMask;

            ContactFilter2D contactFilter2D = new ContactFilter2D();
            contactFilter2D.SetLayerMask(layer);
            contactFilter2D.useTriggers = true;
            int resultLength = Physics2D.OverlapCollider(this.collision, contactFilter2D, hitResult);

            if(resultLength > 0)
            {
                return true;
            }

            return false;
        }
    }
}
