using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Title
{
    public class TitleProduction : MonoBehaviour
    {
        [System.Serializable]
        public class TitleLogoMover
        {
            public GameObject logoObject;
            public Transform target;
            public Easing.Type type;
            public float moveTime;
            public int waitTime;
            Easing[] easing;
            public void Init()
            {
                easing = new Easing[2];
                for(int i = 0;i < 2;++i)
                {
                    easing[i] = new Easing();
                }
                easing[1].Use(type);
                easing[1].Set(logoObject.transform.position.y, target.position.y - logoObject.transform.position.y);
            }
            public void Move()
            {
                logoObject.transform.position = new Vector2(logoObject.transform.position.x, easing[1].Out());
            }
            public Easing[] GetEasing()
            {
                return this.easing;
            }
        }
        [System.Serializable]
        public class FlachParameter
        {
            public Image flash;
            public GameObject[] displayImage;
            public Easing.Type type;
            public float time;
            Easing easing;
            public void Init()
            {
                easing = new Easing();
                easing.Use(type);
                easing.Set(1.0f, -1.0f, time);
            }
            public Easing GetEasing()
            {
                return this.easing;
            }
        }
        [System.Serializable]
        public class FeadInTextParameter
        {
            public Text[] texts;
            public Easing.Type type;
            public float time;
            Easing easing;
            public void Init()
            {
                easing = new Easing();
                easing.Use(type);
                easing.Set(0.0f,1.0f, time);
            }
            public Easing GetEasing()
            {
                return this.easing;
            }
        }
        [System.Serializable]
        public class FlashingBackGround
        {
            public Image back;
            public Color minColor;
            public Color maxColor;
            public void Update(int cnt)
            {
                back.color = Color.Lerp(minColor, maxColor, Mathf.Cos(((cnt * Mathf.PI) / 180.0f) + 1) / 2);
            }
        }
        [System.Serializable]
        public class CursorMoveParameter
        {
            public Image cursor;
            public Easing.Type type;
            public float time;
            public FlashingBackGround flashing;
            Easing easing;
            public void Init()
            {
                easing = new Easing();
                easing.Use(type);
                easing.Set(0.0f, 1.0f, time);
            }
            public Easing GetEasing()
            {
                return this.easing;
            }
        }
        public enum State
        {
            LOGOMOVE,
            FLASH,
            FEADIN_TEXT,
            CURSOR_MOVE,
        }
        public TitleCursorSystem titleCursorSystem;
        public FlachParameter flachParameter;
        public FeadInTextParameter textParameter;
        public CursorMoveParameter moveParameter;
        StateManager stateManager;
        public TitleLogoMover[] titleLogoMover;
        // Use this for initialization
        void Start()
        {
            //カーソルの移動を禁ずる
            titleCursorSystem.SetEnable(false);
            //あとから出すやつのActiveを切る
            for(int i = 0;i < flachParameter.displayImage.Length;++i)
            {
                flachParameter.displayImage[i].SetActive(false);
            }

            stateManager = new StateManager();
            stateManager.SetParameter((int)State.FLASH, new Flash(this));
            stateManager.SetParameter((int)State.FEADIN_TEXT, new FeadInText(this));
            stateManager.SetParameter((int)State.CURSOR_MOVE, new CursorMove(this));
            stateManager.ChengeState((int)State.FLASH);
        }
        // Update is called once per frame
        void Update()
        {
            stateManager.Update();
        }
    }
    //元State
    public abstract class BaseState : StateParameter
    {
        TitleProduction parent;
        public BaseState(TitleProduction titleProduction)
        {
            parent = titleProduction;
        }
        public TitleProduction Base()
        {
            return this.parent;
        }
    }
    //タイトルロゴ動かす
    public class LogoMove : BaseState
    {
        //現在配列番号
        int now;
        int waitCount;
        public LogoMove(TitleProduction titleProduction) : base(titleProduction)
        {
            for (int i = 0; i < Base().titleLogoMover.Length; ++i)
            {
                Base().titleLogoMover[i].Init();
            }
            now = 0;
            waitCount = 0;
        }

        public override void Enter(ref StateManager manager)
        {
            throw new System.NotImplementedException();
        }

        public override void Exit(ref StateManager manager)
        {
        }

        public override bool Transition(ref StateManager manager)
        {
            //配列最後のEasingが終了したなら次へ
            if (!Base().titleLogoMover[Base().titleLogoMover.Length - 1].GetEasing()[1].IsPlay())
            {
                manager.SetNextState((int)Title.TitleProduction.State.FLASH);
                return true;
            }
            return false;
        }

        public override void Update()
        {
            Base().titleLogoMover[now].Move();
            if(!Base().titleLogoMover[now].GetEasing()[1].IsPlay())
            {
                ++waitCount;
                if(waitCount >= Base().titleLogoMover[now].waitTime)
                {
                    ++now;
                    waitCount = 0;
                }
            }
        }
    }
    //画面フラッシュと指定オブジェクトの表示
    public class Flash : BaseState
    {
        TitleProduction.FlachParameter parameter;
        public Flash(TitleProduction titleProduction) : base(titleProduction)
        {
            parameter = titleProduction.flachParameter;
        }

        public override void Enter(ref StateManager manager)
        {
            Base().flachParameter.flash.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            for(int i = 0;i < parameter.displayImage.Length;++i)
            {
                parameter.displayImage[i].SetActive(true);
            }
            parameter.Init();
        }

        public override void Exit(ref StateManager manager)
        {
        }

        public override bool Transition(ref StateManager manager)
        {
            if(!parameter.GetEasing().IsPlay())
            {
                manager.SetNextState((int)TitleProduction.State.FEADIN_TEXT);
                return true;
            }
            return false;
        }

        public override void Update()
        {
            parameter.flash.color = new Color(parameter.flash.color.r, parameter.flash.color.g, parameter.flash.color.b, parameter.GetEasing().Out());
        }
    }
    //テキスト文字フェードイン
    public class FeadInText : BaseState
    {
        TitleProduction.FeadInTextParameter parameter;
        public FeadInText(TitleProduction titleProduction) : base(titleProduction)
        {
            parameter = titleProduction.textParameter;
            for(int i = 0;i < parameter.texts.Length;++i)
            {
                parameter.texts[i].color = new Color(
                    parameter.texts[i].color.r,
                    parameter.texts[i].color.g,
                    parameter.texts[i].color.b,
                    0.0f);
            }
            parameter.Init();
        }

        public override void Enter(ref StateManager manager)
        {

        }

        public override void Exit(ref StateManager manager)
        {
        }

        public override bool Transition(ref StateManager manager)
        {
            if(!parameter.GetEasing().IsPlay())
            {
                manager.SetNextState((int)TitleProduction.State.CURSOR_MOVE);
                return true;
            }
            return false;
        }

        public override void Update()
        {
            for(int i = 0;i < parameter.texts.Length;++i)
            {
                parameter.texts[i].color = new Color(1.0f, 1.0f, 1.0f, parameter.GetEasing().In());
            }
        }
    }
    //カーソルの移動を許可
    public class CursorMove : BaseState
    {
        TitleProduction.CursorMoveParameter parameter;
        public CursorMove(TitleProduction titleProduction) : base(titleProduction)
        {
            parameter = titleProduction.moveParameter;
            parameter.cursor.color = Color.clear;
        }

        public override void Enter(ref StateManager manager)
        {
            Base().titleCursorSystem.SetEnable(true);
            parameter.Init();
        }

        public override void Exit(ref StateManager manager)
        {
        }

        public override bool Transition(ref StateManager manager)
        {
            return false;
        }

        public override void Update()
        {
            parameter.cursor.color = new Color(1.0f, 1.0f, 1.0f, parameter.GetEasing().In());
            parameter.flashing.Update(GetTime());
        }
    }
};
