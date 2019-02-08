using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TextEvent
{
    //イベント処理
    namespace Action
    {
        /////////////////////////////////////////////////
        //HPゲージ召喚
        //args: (HP参照先のキャラ名, Vector2(少数2つのスクリーン空間) position)
        public class CreateBossHPGauge : ActionFunction
        {
            public CreateBossHPGauge(EventGameData gameData) :
                base(gameData)
            {
            }

            private string objName;
            private Vector3 pos;

            public override void Action()
            {
                // プレハブを取得
                GameObject prefab = (GameObject)Resources.Load("prefab/BossGauge");
                // プレハブからインスタンスを生成
                var obj = GameObject.Instantiate<GameObject>(prefab, Vector3.zero, Quaternion.identity, GetGameData().canvas.transform);
                //参照先設定
                obj.GetComponent<BossGauge>().target = GetGameData().characterManager.GetCharacter(objName);
                obj.GetComponent<RectTransform>().anchoredPosition = this.pos;

                Debug.Log(GetGameData().characterManager.GetCharacterData(objName).hitPoint.GetMaxHP());
            }

            public override void ActionStart(string[] args)
            {
                this.objName = args[0];
                this.pos = new Vector3(float.Parse(args[1]), float.Parse(args[2]));
            }

            public override bool IsEnd()
            {
                //すぐ終わる
                return true;
            }
        }
        /////////////////////////////////////////////////
        //リザルト召喚
        //args: (無し)
        public class CreateResult : ActionFunction
        {
            public CreateResult(EventGameData gameData) :
                base(gameData)
            {
            }

            public override void Action()
            {
                // プレハブを取得
                GameObject prefab = (GameObject)Resources.Load("prefab/Result");
                // プレハブからインスタンスを生成
                var obj = GameObject.Instantiate<GameObject>(prefab, GetGameData().canvas.transform);
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
        //ウェイト
        //args: (フレーム単位のウェイト)
        public class WaitForFrame : ActionFunction
        {
            float waitFrame;
            int count;

            public WaitForFrame(EventGameData gameData) :
                base(gameData)
            {
            }


            public override void Action()
            {
                ++count;
            }

            public override void ActionStart(string[] args)
            {
                this.waitFrame = float.Parse(args[0]);
                this.count = 0;
            }

            public override bool IsEnd()
            {
                //カウントが終わったら終わる
                if(this.count > this.waitFrame)
                {
                    return true;
                }

                return false;
            }
        }

        /////////////////////////////////////////////////
        //カメラのターゲットを指定する(キャラ)
        //args: (Vector2(少数2つ) position)
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
        //カメラを止める
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


        /////////////////////////////////////////////////
        //BGMストップ
        //args: (float フェード時間)
        public class BGMStop : ActionFunction
        {
            int fadeTime;
            int count;

            public BGMStop(EventGameData gameData) :
                base(gameData)
            {

            }

            public override void Action()
            {
                ++this.count;
                Sound.SetVolumeBGM(Sound.GetBaseVolumeBGM() * (1.0f - (float)this.count / (float)this.fadeTime));
                if (this.count > this.fadeTime)
                {
                    Sound.StopBGM();
                }
            }

            public override void ActionStart(string[] args)
            {
                //徐々に音量を下げる
                this.fadeTime = int.Parse(args[0]);
                this.count = 0;
            }

            public override bool IsEnd()
            {
                if(this.count > this.fadeTime)
                {
                    return true;
                }
                return false;
            }
        }

        /////////////////////////////////////////////////
        //BGMスタート
        //args: (string BGMName, bool isLoop)
        public class BGMPlay : ActionFunction
        {
            //BGM再生のデータ
            string bgmName;
            bool isLoop;

            public BGMPlay(EventGameData gameData) :
                base(gameData)
            {

            }

            public override void Action()
            {
                Sound.PlayBGM(this.bgmName, this.isLoop);
                Sound.SetVolumeBGM(Sound.GetBaseVolumeBGM());
            }

            public override void ActionStart(string[] args)
            {
                //BGM名
                this.bgmName = args[0];
                this.isLoop = TextEvent.TextParser.ParseBoolean(args[1]);
            }

            public override bool IsEnd()
            {
                //すぐ終わる
                return true;
            }
        }


        /////////////////////////////////////////////////
        //入力をストップ
        //args: (無し)
        public class StopInput : ActionFunction
        {
            //相対的な位置

            public StopInput(EventGameData gameData) :
                base(gameData)
            {

            }

            public override void Action()
            {
                M_System.input.SetEnableStop(true);
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
        //入力ストップを解除
        //args: (無し)
        public class StartInput : ActionFunction
        {
            //相対的な位置

            public StartInput(EventGameData gameData) :
                base(gameData)
            {

            }

            public override void Action()
            {
                M_System.input.SetEnableStop(false);

                //強制入力も解除
                M_System.input.SetForced(SystemInput.Tag.LSTICK_RIGHT, false);
                M_System.input.SetForced(SystemInput.Tag.LSTICK_DOWN, false);
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
        //左スティック強制入力
        //args: (float x, float y, int waitFrame)
        public class InputLeftStick : ActionFunction
        {
            //スティックの値
            Vector2 axis;
            int waitFrame;
            int count;

            public InputLeftStick(EventGameData gameData) :
                base(gameData)
            {

            }

            public override void Action()
            {
                M_System.input.SetAxisForced(SystemInput.Tag.LSTICK_RIGHT, this.axis.x);
                M_System.input.SetAxisForced(SystemInput.Tag.LSTICK_DOWN, this.axis.y);
                M_System.input.SetForced(SystemInput.Tag.LSTICK_RIGHT, true);
                M_System.input.SetForced(SystemInput.Tag.LSTICK_DOWN, true);
            }

            public override void ActionStart(string[] args)
            {
                this.axis = new Vector2(float.Parse(args[0]), float.Parse(args[1]));
                this.waitFrame = int.Parse(args[2]);

                this.count = 0;
            }

            public override bool IsEnd()
            {
                //解除しない限りはずっと倒しっぱなし
                return true;
            }
        }


        /////////////////////////////////////////////////
        //キャラのステート強制書き換え
        //args: (string characterName, int stateID)
        public class ChangeCharacterState : ActionFunction
        {
            string characterName;
            int stateID;

            public ChangeCharacterState(EventGameData gameData) :
                base(gameData)
            {

            }

            public override void Action()
            {
                GetGameData().characterManager.GetCharacter(this.characterName).ChangeState(this.stateID);
            }

            public override void ActionStart(string[] args)
            {
                this.characterName = args[0];
                this.stateID = int.Parse(args[1]);
            }

            public override bool IsEnd()
            {
                //すぐ終わる
                return true;
            }
        }
    }
}
