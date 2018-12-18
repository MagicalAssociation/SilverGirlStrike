using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TextEvent
{
    //イベント処理
    namespace Action
    {
        /////////////////////////////////////////////////
        //テストイベント処理
        public class Action1 : ActionFunction
        {
            public Action1(EventGameData gameData) :
                base(gameData)
            {
            }

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

        /////////////////////////////////////////////////
        //カメラのターゲットを指定する(キャラ)
        //args: (Vector2(少数2つ) relationalPosition)
        public class SetCameraTargetPosition : ActionFunction
        {
            Vector3 position;

            public SetCameraTargetPosition(EventGameData gameData):
                base(gameData)
            {

            }

            public override void Action(){
                //カメラの相対位置を設定
                GetGameData().cameraTargetPosition.SetTarget(this.position);
            }

            public override void ActionStart(string[] args){
                //相対位置を設定
                this.position = new Vector2(float.Parse(args[0]), float.Parse(args[1]));
            }

            public override bool IsEnd(){
                //すぐ終わる
                return true;
            }
        }
        /////////////////////////////////////////////////
        //カメラのターゲットを指定する(キャラ)
        //args: (string charactername)
        public class SetCameraTargetCharacter : ActionFunction
        {
            Transform character;

            public SetCameraTargetCharacter(EventGameData gameData) :
                base(gameData)
            {

            }

            public override void Action()
            {
                //カメラの相対位置を設定
                GetGameData().cameraTargetPosition.SetTarget(this.character);
            }

            public override void ActionStart(string[] args)
            {
                string characterName = args[0];

                //キャラクター名で登録されているキャラの位置を獲得
                this.character = GetGameData().characterManager.GetCharacterTrans(characterName);
            }

            public override bool IsEnd()
            {
                //すぐ終わる
                return true;
            }
        }

        /////////////////////////////////////////////////
        //カメラのターゲットを指定する(キャラ)
        //args: 無し
        public class StopCameraChase : ActionFunction
        {
            Transform character;

            public StopCameraChase(EventGameData gameData) :
                base(gameData)
            {

            }

            public override void Action()
            {
                //カメラの相対位置を設定
                GetGameData().cameraTargetPosition.StopChase();
            }

            public override void ActionStart(string[] args)
            {
            }

            public override bool IsEnd()
            {
                //すぐ終わる
                return true;
            }
        }

        /////////////////////////////////////////////////
        //カメラの相対位置を指定する
        //args: (Vector2(カンマ区切り少数2つ) relationalPosition)
        public class SetCameraLocalPosition : ActionFunction
        {
            //相対的な位置
            Vector2 relationalPosition;

            public SetCameraLocalPosition(EventGameData gameData) :
                base(gameData)
            {

            }

            public override void Action()
            {
                //カメラの相対位置とターゲットを同時設定
                GetGameData().cameraTargetPosition.SetRelativePosition(this.relationalPosition);
            }

            public override void ActionStart(string[] args)
            {
                //相対位置を設定
                this.relationalPosition = new Vector2(float.Parse(args[0]), float.Parse(args[1]));
            }

            public override bool IsEnd()
            {
                //すぐ終わる
                return true;
            }
        }

    }
}
