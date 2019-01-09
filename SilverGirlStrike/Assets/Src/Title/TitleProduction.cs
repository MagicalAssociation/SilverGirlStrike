using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            Easing[] easing;
            public void Init()
            {
                easing = new Easing[2];
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
        public enum State
        {
            LOGOMOVE,
            FLASH,
        }
        public TitleCursorSystem titleCursorSystem;
        StateManager stateManager;
        public TitleLogoMover[] titleLogoMover;
        // Use this for initialization
        void Start()
        {
            titleCursorSystem.SetEnable(false);
            for (int i = 0; i < titleLogoMover.Length; ++i)
            {
                titleLogoMover[i].Init();
            }
            stateManager = new StateManager();
            stateManager.SetParameter((int)State.LOGOMOVE, new LogoMove(this));
        }
        // Update is called once per frame
        void Update()
        {
            stateManager.Update();
        }
    }
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
    public class LogoMove : BaseState
    {
        //現在配列番号
        int now;
        public LogoMove(TitleProduction titleProduction) : base(titleProduction)
        {
            now = 0;
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
                ++now;
            }
        }
    }
    public class Flash : BaseState
    {
        public Flash(TitleProduction titleProduction) : base(titleProduction)
        {
        }

        public override void Enter(ref StateManager manager)
        {
            throw new System.NotImplementedException();
        }

        public override void Exit(ref StateManager manager)
        {
            throw new System.NotImplementedException();
        }

        public override bool Transition(ref StateManager manager)
        {
            throw new System.NotImplementedException();
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
        }
    }
};
