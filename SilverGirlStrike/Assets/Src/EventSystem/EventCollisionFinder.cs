using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TextEvent
{
    public class EventCollisionFinder : MonoBehaviour
    {
        //コリジョンを獲得
        public EventCollision FindCollision(string name)
        {
            var obj = this.transform.Find(name);
            return obj.GetComponent<EventCollision>();
        }
    }
}
