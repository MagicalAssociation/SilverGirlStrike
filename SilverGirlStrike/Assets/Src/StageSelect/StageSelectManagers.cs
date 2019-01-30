using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ここでセーブとステージセレクトとアイテムセレクトの入力管理を行う

public class StageSelectManagers : MonoBehaviour {
    public enum Type
    {
        STAGE = 0,
        SAVE = 1,
        TYPE_NUM = 2,
    }
    public Type begin;
    private Type next;
    private Type now;
    private Type pre;
    [System.Serializable]
    public class MoveObject
    {
        public enum Type
        {
            SPRITE,IMAGE
        }
        public GameObject obj;
        public Vector2 offset;
        public Type type;
    }
    [System.Serializable]
    public class Parameter
    {
        public Type type;
        public GameObject manager;
        public MoveObject[] moveObject;
        private Vector2[] origin;
        public void SetPosition(Vector2 position)
        {
            for(int i = 0;i < moveObject.Length;++i)
            {
                moveObject[i].obj.transform.position = new Vector3(position.x, moveObject[i].obj.transform.position.y);
            }
        }
        public void SetOrigin()
        {
            origin = new Vector2[moveObject.Length];
            for(int i = 0;i < origin.Length;++i)
            {
                origin[i] = new Vector2(moveObject[i].obj.transform.position.x, moveObject[i].obj.transform.position.y);
            }
        }
        public void MovePosition(Vector2 vector)
        {
            for(int i = 0;i < moveObject.Length;++i)
            {
                //Vector2 now = moveObject[i].transform.position;
                moveObject[i].obj.transform.position = new Vector3(origin[i].x + vector.x, moveObject[i].obj.transform.position.y);
                Debug.Log(moveObject[i].obj.name + moveObject[i].obj.transform.position);
            }
        }
    }
    public Vector2 size;
    public Parameter[] parameters;
    public Easing easing;
    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < parameters.Length; ++i)
        {
            if (begin == parameters[i].type)
            {
                parameters[i].manager.SetActive(true);
            }
            else
            {
                parameters[i].manager.SetActive(false);
            }
        }
        now = begin;
    }

    // Update is called once per frame
    void Update()
    {
        if (easing.IsPlay())
        {
            float tmp = easing.Move(easing.parameter.move);
            GetParameter(now).MovePosition(new Vector2(tmp, 0));
            GetParameter(next).MovePosition(new Vector2(tmp, 0));
            if(!easing.IsPlay())
            {
                pre = now;
                now = next;
                GetParameter(pre).manager.SetActive(false);
                M_System.input.SetEnableStop(false);
            }
        }
        else if (M_System.input.Down(SystemInput.Tag.LSTICK_LEFT))
        {
            //-
            next = now - 1;
            if(next < 0)
            {
                next = Type.TYPE_NUM - 1;
            }
            GetParameter(next).SetPosition(new Vector2(20, 0));
            GetParameter(next).manager.SetActive(true);
            GetParameter(next).SetOrigin();
            GetParameter(now).SetOrigin();
            M_System.input.SetEnableStop(true);
            easing.ResetTime();
            easing.Set(0, -20, easing.parameter.time, easing.parameter.type);
        }
        else if(M_System.input.Down(SystemInput.Tag.LSTICK_RIGHT))
        {
            //+
            //next = now + 1;
            //if(next >= Type.TYPE_NUM)
            //{
            //    next = 0;
            //}
            //GetParameter(next).SetPosition(new Vector2(-960, 0));
            //GetParameter(next).manager.SetActive(true);
            //GetParameter(next).SetOrigin();
            //GetParameter(now).SetOrigin();
            //M_System.input.SetEnableStop(true);
            //easing.ResetTime();
            //easing.Set(0, 960, easing.parameter.time, easing.parameter.type);
        }
    }
    private Parameter GetParameter(Type type)
    {
        for(int i = 0;i < parameters.Length;++i)
        {
            if(parameters[i].type == type)
            {
                return parameters[i];
            }
        }
        return null;
    }
}
