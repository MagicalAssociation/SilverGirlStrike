using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TextEvent
{
    //イベント条件
    namespace Term
    {
        /////////////////////////////////////////////////////////////////////////////////////
        //指定回数のみtrue
        //args: (int count)
        public class CheckCount : TermFunction
        {
            public CheckCount(EventGameData gameData) :
                base(gameData)
            {
            }

            public override bool Judge(string[] args)
            {
                int count = int.Parse(args[0]);
                return (GetGameData().currentEvent.GetExecuteCount() < count);
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////
        //キャラが生きているかどうか
        //args: (string characterName)
        public class CheckAliveCharacter : TermFunction
        {
            public CheckAliveCharacter(EventGameData gameData) :
                base(gameData)
            {
            }

            public override bool Judge(string[] args)
            {
                var character = GetGameData().characterManager.GetCharacterData(args[0]);

                return (character != null);
            }
        }
        /////////////////////////////////////////////////////////////////////////////////////
        //コリジョンに触れているとtrue
        //args: (string collisionObjectName)
        public class CheckCollision : TermFunction
        {
            public CheckCollision(EventGameData gameData) :
                base(gameData)
            {
            }

            public override bool Judge(string[] args)
            {
                var collision = GetGameData().collisionFinder.FindCollision(args[0]);
                bool result =  collision.IsHit((int)M_System.LayerName.PLAYER);
                return result;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////
        //フラグが等しいときtrue
        //args: (string flagName, int value)
        public class EqualFlagValue : TermFunction
        {
            public EqualFlagValue(EventGameData gameData) :
                base(gameData)
            {
            }

            public override bool Judge(string[] args)
            {
                string flagName = args[0];
                int value = int.Parse(args[1]);

                bool result = GetGameData().flagManager.Equal(flagName, value);

                return result;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////
        //フラグがvalueより大きい時true
        //args: (string flagName, int value)
        public class GreaterFlagValue : TermFunction
        {
            public GreaterFlagValue(EventGameData gameData) :
                base(gameData)
            {
            }

            public override bool Judge(string[] args)
            {
                string flagName = args[0];
                int value = int.Parse(args[1]);

                bool result = GetGameData().flagManager.Greater(flagName, value);

                return result;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////
        //フラグがvalueより小さい時true
        //args: (string flagName, int value)
        public class SmallerFlagValue : TermFunction
        {
            public SmallerFlagValue(EventGameData gameData) :
                base(gameData)
            {
            }

            public override bool Judge(string[] args)
            {
                string flagName = args[0];
                int value = int.Parse(args[1]);

                bool result = GetGameData().flagManager.Smaller(flagName, value);

                return result;
            }
        }
    }
}
