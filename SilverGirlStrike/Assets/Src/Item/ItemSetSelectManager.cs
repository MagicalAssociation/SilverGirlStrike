using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSetSelectManager : CursorSystem
{
    [System.Serializable]
    public class CursorColor
    {
        public Color selectImageColor;
        public Color notSelectcImageColor;
        public Color selectBackColor;
        public Color notSelectBackColor;
    }
    [System.Serializable]
    public class Parameter
    {
        public SetSelect left;
        public SetSelect right;
        public SetSelect up;
        public SetSelect down;
        public void SetColor(Color image,Color back)
        {
            left.SetColor(image,back);
            right.SetColor(image,back);
            up.SetColor(image,back);
            down.SetColor(image,back);
        }
    }
    public Parameter parameter;
    public ItemSelectManager preManager;
    SetSelect nowParam;
    SGS.Item item;
    public CursorColor cursorColor;
	// Use this for initialization
	void Start ()
    {
        this.SetEnable(false);
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(MoveInput() == true)
        {
            this.ChangeColor();
        }
        else
        {
            if(M_System.input.Down(SystemInput.Tag.DECISION) && GetEnable() == true)
            {
                nowParam.SetItemData(item);
                base.SetEnable(false);
                preManager.SetEnable(true);
            }
            else if(M_System.input.Down(SystemInput.Tag.CANCEL) && GetEnable() == true)
            {
                base.SetEnable(false);
                base.SetEnable(false);
                preManager.SetEnable(true);
            }
        }
	}
    private bool MoveInput()
    {
        if(GetEnable() == false)
        {
            return false;
        }
        if(M_System.input.Down(SystemInput.Tag.LSTICK_UP))
        {
            nowParam = parameter.up;
            return true;
        }
        else if(M_System.input.Down(SystemInput.Tag.LSTICK_DOWN))
        {
            nowParam = parameter.down;
            return true;
        }
        else if(M_System.input.Down(SystemInput.Tag.LSTICK_LEFT))
        {
            nowParam = parameter.left;
            return true;
        }
        else if(M_System.input.Down(SystemInput.Tag.LSTICK_RIGHT))
        {
            nowParam = parameter.right;
            return true;
        }
        return false;
    }
    private void ChangeColor()
    {
        parameter.SetColor(cursorColor.notSelectcImageColor, cursorColor.notSelectBackColor);
        if(nowParam != null)
        {
            nowParam.SetColor(cursorColor.selectImageColor, cursorColor.selectBackColor);
        }
    }
    public void SetItemData(SGS.Item item)
    {
        this.item = item;
    }
    public override void Enter()
    {
        nowParam = parameter.up;
        ChangeColor();
    }
    public override void Exit()
    {
        nowParam = null;
        ChangeColor();
    }
}
