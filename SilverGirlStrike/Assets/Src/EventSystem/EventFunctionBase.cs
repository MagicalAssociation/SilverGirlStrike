using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//編集履歴
//2018/11/13 板倉：作成
//2018/11/14 板倉：クラス本体を実装


namespace TextEvent
{
    //イベントテキストの文字列を数値に変換する関数軍
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

    public class EventFunctionDictionary
    {
        private Dictionary<string, Term.TermFunction> termFunctions;
        private Dictionary<string, Action.ActionFunction> actionFunctions;

        //固定値なのでコンストラクターで設定してもいいんじゃないかな
        public EventFunctionDictionary()
        {
            this.termFunctions = new Dictionary<string, Term.TermFunction>();
            this.actionFunctions = new Dictionary<string, Action.ActionFunction>();

            SetTermFunc();
            SetActionFunc();
        }

        //名前から条件判定機能を獲得
        public Term.TermFunction GetTermFunction(string name)
        {
            return this.termFunctions[name];
        }
        //名前からイベント処理機能を獲得
        public Action.ActionFunction GetActionFunction(string name)
        {
            return this.actionFunctions[name];
        }

        //条件追加
        private void SetTermFunc()
        {
            this.termFunctions.Add("term1", new Term.Term1());
        }
        //イベント処理追加
        private void SetActionFunc()
        {
            this.actionFunctions.Add("action1", new Action.Action1());
        }

    }

}
