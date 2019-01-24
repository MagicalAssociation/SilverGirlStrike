using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemSetSelectManager : CursorSystem
{
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
        public void Reset()
        {
            left.SetItemData(null);
            right.SetItemData(null);
            up.SetItemData(null);
            down.SetItemData(null);
        }
    }
    public Parameter parameter;
    SetSelect nowParam;
    SGS.Item item;
    public SGS.CursorColor cursorColor;
    public GameObject cursor;
    public Vector2 cursorOffset;
	// Use this for initialization
	void Start ()
    {
        cursor.transform.position = parameter.up.transform.position;
        ChangeColor();
        cursor.SetActive(false);
        parameter.Reset();
	}	
	// Update is called once per frame
	public override void SystemUpdate(CursorSystemManager manager)
    {
        if(MoveInput() == true)
        {
            this.ChangeColor();
            this.CursoeMove();
        }
        else
        {
            if(M_System.input.Down(SystemInput.Tag.DECISION))
            {
                nowParam.SetItemData(item);
                manager.Next((int)ItemManagers.Type.SELECT);
            }
            else if(M_System.input.Down(SystemInput.Tag.CANCEL))
            {
                manager.Next((int)ItemManagers.Type.SELECT);
            }
        }
	}
    private bool MoveInput()
    {
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
    private void CursoeMove()
    {
        cursor.transform.position = nowParam.transform.position;
        cursor.transform.position = cursor.transform.position + new Vector3(cursorOffset.x, cursorOffset.y);
    }
    public void SetItemData(SGS.Item item)
    {
        this.item = item;
    }
    public override void Enter()
    {
        nowParam = parameter.up;
        ChangeColor();
        cursor.GetComponent<Image>().sprite = item.GetSprite();
        cursor.SetActive(true);
        CursoeMove();
    }
    public override void Exit()
    {
        nowParam = null;
        ChangeColor();
        cursor.GetComponent<Image>().sprite = null;
        cursor.SetActive(false);
    }
}
