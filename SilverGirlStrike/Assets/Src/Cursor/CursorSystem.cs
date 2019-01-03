using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CursorSystem : MonoBehaviour {
    public GameObject[] cursors;
    private Vector2Int nowPos;
    private List<CursorParam[]> cursorlist;
    bool enable;
    bool loop;
	// Use this for initialization
	public void Init () {
        nowPos = new Vector2Int(0, 0);
        cursorlist = new List<CursorParam[]>();
        for (int i = 0; i < cursors.Length; ++i)
        {
            cursorlist.Add(cursors[i].GetComponentsInChildren<CursorParam>());
        }
        enable = true;
        loop = true;
	}
	
	// Update is called once per frame
	void Update ()
    {
        //if(enable && M_System.input.Down(SystemInput.Tag.DECISION))
        //{
        //    cursorlist[nowPos.x][nowPos.y].Decision();
        //}
	}
    public Vector2Int GetNow()
    {
        return this.nowPos;
    }
    public List<CursorParam[]> GetList()
    {
        return this.cursorlist;
    }
    public CursorParam GetNowParam()
    {
        return cursorlist[nowPos.x][nowPos.y];
    }
    public CursorParam GetParam(int x,int y)
    {
        //マイナスの場合最大値-値の番号を返すよう値の修正を行う
        if(x < 0)
        {
            x = cursorlist.Count - ((x * -1) % cursorlist.Count);
        }
        if(y < 0)
        {
            y = cursorlist[x].Length - ((y * -1) % cursorlist[x].Length);
        }
        return cursorlist[x % cursorlist.Count][y % cursorlist[x % cursorlist.Count].Length];
    }
    //returnでfalseを返すのはSEとか流すときに移動しなかった時の判断用
    //ループ設定でマイナスするとき、最大値以上のマイナスになることを考慮した計算をしていない
    //※--しかしないからいらない。
    public bool Down()
    {
        //カーソル値を下に移動
        if(loop)
        {
            //0~配列数-1をループする計算
            nowPos.y = (nowPos.y + 1) % cursorlist[nowPos.x].Length;
        }
        else
        {
            //最大値を超えた場合最大値-1をいれる
            ++nowPos.y;
            if (cursorlist[nowPos.x].Length >= nowPos.y)
            {
                nowPos.y = cursorlist[nowPos.x].Length - 1;
                return false;
            }
        }
        return true;
    }
    public bool Up()
    {
        --nowPos.y;
        //位置がマイナスになった時ループなら下に、そうでないなら0にする
        if(nowPos.y < 0)
        {
            if(loop)
            {
                //マイナスの分だけ最大値から引く
                nowPos.y = cursorlist[nowPos.x].Length + nowPos.y;
            }
            else
            {
                //0で停止
                nowPos.y = 0;
                return false;
            }
        }
        return true;
    }
    public bool Right()
    {
        if (loop)
        {
            //0~配列数-1をループする計算
            nowPos.x = (nowPos.x + 1) % cursorlist.Count;
        }
        else
        {
            //最大値以上で最大値-1の代入
            ++nowPos.x;
            if (cursorlist.Count >= nowPos.x)
            {
                nowPos.x = cursorlist.Count - 1;
                return false;
            }
        }
        BeyondArrayCheck();
        return true;
    }
    public bool Left()
    {
        --nowPos.x;
        //位置がマイナスになった時ループなら下に、そうでないなら0にする
        if (nowPos.x < 0)
        {
            if (loop)
            {
                //最大値-マイナス値
                nowPos.x = cursorlist.Count + nowPos.x;
            }
            else
            {
                nowPos.x = 0;
                return false;
            }
        }
        BeyondArrayCheck();
        return true;
    }
    private void BeyondArrayCheck()
    {
        //横に移動したときに配列数よりも大きい値になっていた場合に配列数-1に補正をかける処理
        if(cursorlist[nowPos.x].Length <= nowPos.y)
        {
            nowPos.y = cursorlist[nowPos.x].Length - 1;
        }
    }
}