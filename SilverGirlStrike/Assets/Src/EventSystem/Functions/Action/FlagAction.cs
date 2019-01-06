using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TextEvent
{
    namespace Action
    {
        /////////////////////////////////////////////////////////////////////////
        //フラグ宣言
        //args: (string fragName)
        public class CreateFlag : ActionFunction
        {
            string flagName;

            public CreateFlag(EventGameData eventGameData) :
                base(eventGameData)
            {
            }

            public override void Action()
            {
                GetGameData().flagManager.Create(this.flagName);
            }

            public override void ActionStart(string[] args)
            {
                this.flagName = args[0];
            }

            public override bool IsEnd()
            {
                //すぐ終わる
                return true;
            }
        }

        /////////////////////////////////////////////////////////////////////////
        //フラグに代入
        //args: (string fragName)
        public class SetFlag : ActionFunction
        {
            string flagName;
            int value;

            public SetFlag(EventGameData eventGameData):
                base(eventGameData)
            {
            }

            public override void Action()
            {
                GetGameData().flagManager.SetValue(this.flagName, this.value);
            }

            public override void ActionStart(string[] args)
            {
                this.flagName = args[0];
                this.value = int.Parse(args[1]);
            }

            public override bool IsEnd()
            {
                //すぐ終わる
                return true;
            }
        }
        /////////////////////////////////////////////////////////////////////////
        //フラグに加算、マイナスで引き算
        //args: (string fragName, int value)
        public class AddFlag : ActionFunction
        {
            string flagName;
            int value;

            public AddFlag(EventGameData eventGameData) :
                base(eventGameData)
            {
            }

            public override void Action()
            {
                GetGameData().flagManager.AddValue(this.flagName, this.value);
            }

            public override void ActionStart(string[] args)
            {
                this.flagName = args[0];
                this.value = int.Parse(args[1]);
            }

            public override bool IsEnd()
            {
                //すぐ終わる
                return true;
            }
        }

        /////////////////////////////////////////////////////////////////////////
        //フラグを乗算
        //args: (string fragName, int value)
        public class Mulflag : ActionFunction
        {
            string flagName;
            int value;

            public Mulflag(EventGameData eventGameData) :
                base(eventGameData)
            {
            }

            public override void Action()
            {
                GetGameData().flagManager.MultipleValue(this.flagName, this.value);
            }

            public override void ActionStart(string[] args)
            {
                this.flagName = args[0];
                this.value = int.Parse(args[1]);
            }

            public override bool IsEnd()
            {
                //すぐ終わる
                return true;
            }
        }

        /////////////////////////////////////////////////////////////////////////
        //フラグを割算
        //args: (string fragName, int value)
        public class DivFlag : ActionFunction
        {
            string flagName;
            int value;

            public DivFlag(EventGameData eventGameData) :
                base(eventGameData)
            {
            }

            public override void Action()
            {
                GetGameData().flagManager.DivideValue(this.flagName, this.value);
            }

            public override void ActionStart(string[] args)
            {
                this.flagName = args[0];
                this.value = int.Parse(args[1]);
            }

            public override bool IsEnd()
            {
                //すぐ終わる
                return true;
            }
        }

        /////////////////////////////////////////////////////////////////////////
        //フラグを記録
        //args: (無し)
        public class SaveFlag : ActionFunction
        {
            public SaveFlag(EventGameData eventGameData) :
                base(eventGameData)
            {
            }

            public override void Action()
            {
                GetGameData().flagManager.SaveFlag();
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
        /////////////////////////////////////////////////////////////////////////
        //フラグを読み出し
        //args: (無し)
        public class LoadFlag : ActionFunction
        {
            public LoadFlag(EventGameData eventGameData) :
                base(eventGameData)
            {
            }

            public override void Action()
            {
                GetGameData().flagManager.LoadFlag();
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

    }
}

