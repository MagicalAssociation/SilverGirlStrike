using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//編集履歴
//2018/11/13 板倉：作成
//2018/11/14 板倉：クラス本体を実装


namespace Event
{
    class TextParser
    {
        //boolっぽい文字をboolに変換
        public static bool ParseBoolean(string text)
        {
            Debug.Log(text);
            switch (text)
            {
                case "true":
                    return true;
                case "false":
                    return false;
                default:
                    //所定の文字列以外は全て例外
                    throw new System.Exception("illegal boolean text: " + text + "(boolean text is only 'true' and 'false')");
            }
        }
    }



    //イベントを実行管理するクラス
    public class EventManager
    {


        //条件判定
        public void Update()
        {

        }
    }

    public class EventUnit
    {
        EventPerser.EventData data;
        Dictionary<string, Term.TermFunction> termFunctions;
        Dictionary<string, Action.ActionFunction> actionFunctions;
        int actionPosition;
        bool isActive;

        public EventUnit(EventPerser.EventData data)
        {
            this.data = data;
            this.actionPosition = 0;
            this.isActive = false;

            SetDictionary();
        }

        //イベントと名前を紐づける
        void SetDictionary()
        {
            //条件
            this.termFunctions = new Dictionary<string, Term.TermFunction>();
            this.termFunctions.Add("term1", new Term.Term1());

            //イベント処理
            this.actionFunctions = new Dictionary<string, Action.ActionFunction>();
            this.actionFunctions.Add("action1", new Action.Action1());
        }

        public void Update()
        {
            TermCheck();
            ActionUpdate();
        }

        void ActionUpdate()
        {
            //条件をクリアしていない場合はイベント処理は動かない
            if (!this.isActive)
            {
                return;
            }
            var actionText = this.data.actionText[this.actionPosition];
            var cullentAction = this.actionFunctions[actionText.eventName];
            //アクションを実行
            cullentAction.Action(actionText.args);
            //イベントの1アクションが終わったら次のアクションへ
            if (cullentAction.IsEnd())
            {
                ++this.actionPosition;
            }

            //イベントの終端に達したらイベント終わり
            if(this.actionPosition >= this.data.actionText.Count)
            {
                this.isActive = false;
                this.actionPosition = 0;
            }

        }

        void TermCheck()
        {
            //条件判定を一度でもクリアしたら以降はイベント処理を行う
            if (this.isActive)
            {
                return;
            }
            //判定
            bool result = true;
            foreach(var i in this.data.termText)
            {
                var term = this.termFunctions[i.eventName];
                if (term.Judge(i.args) ==  i.requestedBoolean)
                {
                    continue;
                }
                //一つでも条件を満たさなかったものがある場合は条件未クリアとする
                result = false;
            }

            this.isActive = result;
        }

    }



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

    //イベント処理
    namespace Action
    {

        //イベントの実際の処理を行うための基底クラス
        public abstract class ActionFunction
        {
            //イベント処理、引数を表す値として文字列配列が渡される
            public abstract void Action(string[] args);
            //イベントの処理が終わったかどうか
            public abstract bool IsEnd();
        }
        //テスト処理
        public class Action1 : ActionFunction
        {
            public override void Action(string[] args)
            {
                //指定位置に特定のオブジェクトを動かす
                var obj = GameObject.Find(args[0]);
                if(obj == null)
                {
                    return;
                }
                obj.transform.position = new Vector3(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]));
            }

            public override bool IsEnd()
            {
                //すぐ終わる
                return true;
            }
        }
    }
}
