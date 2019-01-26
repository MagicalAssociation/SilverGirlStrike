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
            if (left.GetItem() != null)
            {
                left.GetItem().SetNumver(left.GetItem().GetNumver() + 1);
            }
            left.SetItemData(null);
            if (down.GetItem() != null)
            {
                down.GetItem().SetNumver(down.GetItem().GetNumver() + 1);
            }
            down.SetItemData(null);
            if (up.GetItem() != null)
            {
                up.GetItem().SetNumver(up.GetItem().GetNumver() + 1);
            }
            up.SetItemData(null);
            if (right.GetItem() != null)
            {
                right.GetItem().SetNumver(right.GetItem().GetNumver() + 1);
            }
            right.SetItemData(null);
        }
        public void ItemDataCheck(SGS.Item item)
        {
            if (left.GetItem() != null)
            {
                if (left.GetItem().GetID() == item.GetID())
                {
                    left.GetItem().SetNumver(left.GetItem().GetNumver() + 1);
                    left.SetItemData(null);
                }
            }
            if (right.GetItem() != null)
            {
                if (right.GetItem().GetID() == item.GetID())
                {
                    right.GetItem().SetNumver(right.GetItem().GetNumver() + 1);
                    right.SetItemData(null);
                }
            }
            if (up.GetItem() != null)
            {
                if (up.GetItem().GetID() == item.GetID())
                {
                    up.GetItem().SetNumver(up.GetItem().GetNumver() + 1);
                    up.SetItemData(null);
                }
            }
            if (down.GetItem() != null)
            {
                if (down.GetItem().GetID() == item.GetID())
                {
                    down.GetItem().SetNumver(down.GetItem().GetNumver() + 1);
                    down.SetItemData(null);
                }
            }
        }

        public SetSelect Search(SetSelect select)
        {
            if(select == up)
            {
                return up;
            }
            else if(select == down)
            {
                return down;
            }
            else if(select == left)
            {
                return left;
            }
            else if(select == right)
            {
                return right;
            }
            return null;
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
                parameter.ItemDataCheck(item);
                if(nowParam.GetItem() != null)
                {
                    nowParam.GetItem().SetNumver(nowParam.GetItem().GetNumver() + 1);
                }
                nowParam.SetItemData(item);
                manager.Next((int)ItemSelectManagers.Type.SELECT);
            }
            else if(M_System.input.Down(SystemInput.Tag.CANCEL))
            {
                manager.Next((int)ItemSelectManagers.Type.SELECT);
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
