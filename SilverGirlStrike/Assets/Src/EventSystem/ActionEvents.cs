using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TextEvent
{
    //イベント処理
    namespace Action
    {

        //イベントの実際の処理を行うための基底クラス
        public abstract class ActionFunction
        {
            public abstract void ActionStart(string[] args);

            //イベント処理、引数を表す値として文字列配列が渡される
            public abstract void Action();
            //イベントの処理が終わったかどうか
            public abstract bool IsEnd();
        }


        //テスト処理
        public class Action1 : ActionFunction
        {
            private string objName;
            private Vector3 pos;



            public override void Action()
            {
                //指定位置に特定のオブジェクトを動かす
                var obj = GameObject.Find(this.objName);
                if (obj == null)
                {
                    return;
                }
                obj.transform.position = this.pos;
            }

            public override void ActionStart(string[] args)
            {
                this.objName = args[0];
                this.pos = new Vector3(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]));
            }

            public override bool IsEnd()
            {
                //すぐ終わる
                return true;
            }
        }
    }
}
