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
    public class Parameter
    {
        public Type type;
        public GameObject manager;
        public GameObject[] moveObject;
        public void SetPosition(Vector2 position)
        {
            for(int i = 0;i < moveObject.Length;++i)
            {
                moveObject[i].transform.position = new Vector3(position.x, position.y);
            }
        }
        public void MovePosition(Vector2 vector)
        {
            for(int i = 0;i < moveObject.Length;++i)
            {
                Vector2 now = moveObject[i].transform.position;
                moveObject[i].transform.position = new Vector3(now.x + vector.x, now.y + vector.y);
                Debug.Log(moveObject[i].name + moveObject[i].transform.position);
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
            Debug.Log(tmp);
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
            GetParameter(next).SetPosition(new Vector2(-960, 0));
            GetParameter(next).manager.SetActive(true);
            M_System.input.SetEnableStop(true);
            easing.ResetTime();
            easing.Set(0, 960, easing.parameter.time, easing.parameter.type);
        }
        else if(M_System.input.Down(SystemInput.Tag.LSTICK_RIGHT))
        {
            //+
            next = now + 1;
            if(next >= Type.TYPE_NUM)
            {
                next = 0;
            }
            GetParameter(next).SetPosition(new Vector2(960, 0));
            GetParameter(next).manager.SetActive(true);
            M_System.input.SetEnableStop(true);
            easing.ResetTime();
            easing.Set(960, -960, easing.parameter.time, easing.parameter.type);
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
