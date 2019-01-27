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


    //イベント関数と、実際のクラスの組み合わせを指す辞書クラス
    public class EventFunctionDictionary
    {
        private Dictionary<string, Term.TermFunction> termFunctions;
        private Dictionary<string, Action.ActionFunction> actionFunctions;
        EventGameData gameData;

        //固定値なのでコンストラクターで設定してもいいんじゃないかな
        public EventFunctionDictionary(EventGameData gameData)
        {
            this.termFunctions = new Dictionary<string, Term.TermFunction>();
            this.actionFunctions = new Dictionary<string, Action.ActionFunction>();
            this.gameData = gameData;

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
            this.termFunctions.Add("checkCount", new Term.CheckCount(this.gameData));
            this.termFunctions.Add("checkAliveCharacter", new Term.CheckAliveCharacter(this.gameData));
            this.termFunctions.Add("checkCollision", new Term.CheckCollision(this.gameData));
            this.termFunctions.Add("equalFlag", new Term.EqualFlagValue(this.gameData));
            this.termFunctions.Add("greaterFlag", new Term.GreaterFlagValue(this.gameData));
            this.termFunctions.Add("smallerFlag", new Term.SmallerFlagValue(this.gameData));
        }
        //イベント処理追加
        private void SetActionFunc()
        {
            this.actionFunctions.Add("waitForFrame", new Action.WaitForFrame(this.gameData));
            this.actionFunctions.Add("createBossHPGauge", new Action.CreateBossHPGauge(this.gameData));
            this.actionFunctions.Add("createResult", new Action.CreateResult(this.gameData));
            this.actionFunctions.Add("setCameraTargetPosition", new Action.SetCameraTargetPosition(this.gameData));
            this.actionFunctions.Add("setCameraTarget", new Action.SetCameraTargetCharacter(this.gameData));
            this.actionFunctions.Add("setCameraPosition", new Action.SetCameraTargetPosition(this.gameData));
            this.actionFunctions.Add("setCameraLocalPosition", new Action.SetCameraLocalPosition(this.gameData));
            this.actionFunctions.Add("stopCameraChase", new Action.StopCameraChase(this.gameData));

            this.actionFunctions.Add("createFlag", new Action.CreateFlag(this.gameData));
            this.actionFunctions.Add("setFlagValue", new Action.SetFlag(this.gameData));
            this.actionFunctions.Add("addFlagValue", new Action.AddFlag(this.gameData));
            this.actionFunctions.Add("mulFlagValue", new Action.Mulflag(this.gameData));
            this.actionFunctions.Add("divFlagValue", new Action.DivFlag(this.gameData));
            this.actionFunctions.Add("saveFlag", new Action.SaveFlag(this.gameData));
            this.actionFunctions.Add("loadFlag", new Action.LoadFlag(this.gameData));

            this.actionFunctions.Add("playBGM", new Action.BGMPlay(this.gameData));
            this.actionFunctions.Add("stopBGM", new Action.BGMStop(this.gameData));

            this.actionFunctions.Add("stopInput", new Action.StopInput(this.gameData));
            this.actionFunctions.Add("startInput", new Action.StartInput(this.gameData));

            this.actionFunctions.Add("inputLeftStick", new Action.InputLeftStick(this.gameData));
        }

    }

    namespace Action
    {
        //イベントの実際の処理を行うための基底クラス
        public abstract class ActionFunction
        {
            private EventGameData gameData;


            public ActionFunction(EventGameData gameData)
            {
                this.gameData = gameData;
            }

            public EventGameData GetGameData()
            {
                return this.gameData;
            }

            public abstract void ActionStart(string[] args);

            //イベント処理、引数を表す値として文字列配列が渡される
            public abstract void Action();
            //イベントの処理が終わったかどうか
            public abstract bool IsEnd();
        }
    }

    //イベント条件
    namespace Term
    {


        //イベントでの条件判定を行うための基底クラス
        public abstract class TermFunction
        {
            private EventGameData gameData;
            private int count;


            public TermFunction(EventGameData gameData)
            {
                this.gameData = gameData;
            }

            public EventGameData GetGameData()
            {
                return this.gameData;
            }

            //条件を判定する、引数を示す値として文字列配列が渡される
            public abstract bool Judge(string[] args);
        }
    }
}
