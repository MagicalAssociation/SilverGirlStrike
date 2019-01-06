using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TextEvent
{
    //イベントでいじれるゲームのシステムへのアクセス
    [System.Serializable]
    public struct EventGameData{
        //カメラの位置をつかさどるオブジェクト
        public ObjectChaser cameraTargetPosition;
        //フラグをいじるアクセス
        public FlagManager flagManager;
        //キャラクター管理
        public CharacterManager characterManager;
        //イベント判定コリジョンの情報
        public EventCollisionFinder collisionFinder;
    }




    //イベントを実行管理するクラス
    //EventUnitクラスをリストで管理している
    public class EventManager
    {
        private static EventManager myself = null;
        private EventGameData data;
        private List<EventUnit> eventList;
        private EventFunctionDictionary functions;


        public static void Create(EventGameData data)
        {
            if(myself == null)
            {
                myself = new EventManager(data);
            }
        }
        public static EventManager Get()
        {
            return myself;
        }

        private EventManager(EventGameData data)
        {
            this.eventList = new List<EventUnit>();
            this.functions = new EventFunctionDictionary(data);
            this.data = data;
        }

        //全てのイベントを回す
        public void Update()
        {
            foreach (var eventUnit in this.eventList)
            {
                eventUnit.Update(this.functions);
            }
        }

        //戻り値は、配列の位置
        public int AddEvent(EventPerser.EventData data)
        {
            //イベント追加
            this.eventList.Add(new EventUnit(data));

            return this.eventList.Count - 1;
        }

        public void ClearAll()
        {
            this.eventList.Clear();
        }

        public void SetActive(int index, bool isActive)
        {
            if (index >= this.eventList.Count)
            {
                return;
            }
            this.eventList[index].SetActive(isActive);
        }
    }

    //イベント実行の1単位、複数の条件と、複数のイベント内容を管理している
    public class EventUnit
    {
        private EventPerser.EventData data;
        private int actionPosition;
        private bool isAwake;
        private bool isActive;

        private Action.ActionFunction currentAction;

        public EventUnit(EventPerser.EventData data)
        {
            this.data = data;
            this.actionPosition = 0;
            this.isAwake = false;
            this.isActive = true;
        }

        //更新
        public void Update(EventFunctionDictionary func)
        {
            //そもそも無効化されたイベントは条件すら見ない
            if (!this.isActive)
            {
                return;
            }
            TermCheck(func);
            ActionUpdate(func);
        }

        public void SetActive(bool isActive)
        {
            this.isActive = isActive;
        }

        //イベント処理
        private void ActionUpdate(EventFunctionDictionary func)
        {
            //条件をクリアしていない場合はイベント処理は動かない
            if (!this.isAwake)
            {
                return;
            }

            //アクションを実行
            this.currentAction.Action();
            //イベントの1アクションが終わったら次のアクションへ
            if (this.currentAction.IsEnd())
            {
                ++this.actionPosition;

                //イベントの終端に達したらイベント終わり
                if (this.actionPosition >= this.data.actionText.Count)
                {
                    this.isAwake = false;
                    this.actionPosition = 0;
                }
                else
                {
                    //終端じゃないなら次のイベントを設定
                    SetCurrentAction(this.actionPosition, func);
                }
            }

        }

        private void TermCheck(EventFunctionDictionary func)
        {
            //条件判定を一度でもクリアしたら以降はイベント処理を行う
            if (this.isAwake)
            {
                return;
            }
            //判定
            bool result = true;
            foreach (var i in this.data.termText)
            {
                var term = func.GetTermFunction(i.eventName);
                if (term.Judge(i.args) == i.requestedBoolean)
                {
                    continue;
                }
                //一つでも条件を満たさなかったものがある場合は条件未クリアとする
                result = false;
            }

            this.isAwake = result;
            //条件クリア
            if (this.isAwake)
            {
                SetCurrentAction(this.actionPosition, func);
            }
        }

        private void SetCurrentAction(int listPosition, EventFunctionDictionary func)
        {
            var actionText = this.data.actionText[listPosition];
            this.currentAction = func.GetActionFunction(actionText.eventName);
            //アクションを実行
            this.currentAction.ActionStart(actionText.args);
        }

    }
}