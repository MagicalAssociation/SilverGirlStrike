using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TextEvent
{
    //イベント条件
    namespace Term
    {
        //イベントでの条件判定を行うための基底クラス
        public abstract class TermFunction
        {
            //条件を判定する、引数を示す値として文字列配列が渡される
            public abstract bool Judge(string[] args);
        }



        //テスト条件
        public class Term1 : TermFunction
        {
            public override bool Judge(string[] args)
            {
                int arg1 = int.Parse(args[0]);
                int arg2 = int.Parse(args[1]);


                if (arg1 == arg2)
                {
                    return true;
                }
                return false;
            }
        }
    }
}
