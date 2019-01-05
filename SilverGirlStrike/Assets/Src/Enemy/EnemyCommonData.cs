using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy 
{ 
    [System.Serializable]
    public enum Direction
    {
        LEFT = 1,
        RIGHT = -1,
    }
    [System.Serializable]
    public class AutoScaleChange
    {
        //! 向きを自動変更する確認
        Direction direction;
        GameObject myself;
        GameObject target;
        Vector3 scale;
        public TargetSearch targetSearch;
        public void Init(GameObject gameObject)
        {
            myself = gameObject;
            scale = myself.transform.localScale;
            direction = Direction.LEFT;

        }
        public void SetDirection(Direction direction)
        {
            if(targetSearch.enable != true)
            {
                this.direction = direction;
            }
        }
        public Direction GetDirection()
        {
            return this.direction;
        }
        public void DirectionUpdate()
        {
            if(this.targetSearch.enable == true)
            {
                target = targetSearch.Search();
                if (target == null)
                {

                }
                else
                {
                    float dif = target.transform.position.x - myself.transform.position.x;
                    if (dif > 0)
                    {
                        direction = Direction.RIGHT;
                    }
                    else if(dif < 0)
                    {
                        direction = Direction.LEFT;
                    }
                }
            }
            this.myself.transform.localScale = new Vector3(scale.x * (int)direction, scale.y, scale.z);
        }
    }
}
