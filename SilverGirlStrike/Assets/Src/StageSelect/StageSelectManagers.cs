using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ここでセーブとステージセレクトとアイテムセレクトの入力管理を行う

public class StageSelectManagers : MonoBehaviour {
    public enum CanvasTag : int
    {
        STAGE = 0,
        SAVE = 1,
        ITEM = 2,
    }
    [System.Serializable]
    public class Target
    {
        public GameObject canvas;
        //入力処理を呼ぶGameObjectをいれる
        //こいつを非アクティブでUpdateを呼ばないようにする
        public GameObject system;
        public CanvasTag tag;
        public Vector2 GetPosition()
        {
            return this.canvas.transform.position;
        }
    }
    [System.Serializable]
    public class Targets
    {
        public Target[] target;
    }
    public class TagHistory
    {
        public TagHistory(CanvasTag init)
        {
            pre = init;
            now = init;
            next = init;
        }
        public CanvasTag pre;
        public CanvasTag now;
        public CanvasTag next;
    }

    public GameObject mainCamera;
    public Easing[] easings = new Easing[2];
    public Targets[] targets;
    public CanvasTag startTag;
    private TagHistory history;

    private Vector2Int nowNumver;
    private void Start()
    {
        for(int y = 0;y < targets.Length;++y)
        {
            for (int x = 0; x < targets[y].target.Length; ++x)
            {
                if (startTag == targets[y].target[x].tag)
                {
                    targets[y].target[x].system.SetActive(true);
                    nowNumver = new Vector2Int(x, y);
                    history = new TagHistory(startTag);
                }
                else
                {
                    targets[y].target[x].system.SetActive(false);
                }
            }
        }

        Sound.PlayBGM("stageSelect", true);
    }
    private void Update()
    {
        //Debug.Log(easings[0].IsPlay() || easings[1].IsPlay());
        //どちらかが移動中なら移動処理をする
        if(easings[0].IsPlay() || easings[1].IsPlay())
        {
            //移動処理
            ObjectMove();
            //終了時に入力を再開する
            if(!easings[0].IsPlay() && !easings[1].IsPlay())
            {
                M_System.input.SetEnableStop(false);
                history.pre = history.now;
                Get(history.pre).system.SetActive(false);
                history.now = history.next;
            }
        }
        //右移動
        //入力を禁止する
        else if(M_System.input.Down(SystemInput.Tag.LSTICK_RIGHT))
        {
            if(Right())
            {
                InputMove();
            }
        }
        //左移動
        //入力を禁止する
        else if(M_System.input.Down(SystemInput.Tag.LSTICK_LEFT))
        {
            if(Left())
            {
                InputMove();
            }
        }
    }
    private void InputMove()
    {
        //入力を禁止します。
        M_System.input.SetEnableStop(true);
        history.next = GetTag(nowNumver);
        //次のCanvasの位置でEasingを登録する
        easings[0].ResetTime();
        easings[1].ResetTime();
        easings[0].Set(mainCamera.transform.position.x, Get(history.next).GetPosition().x - mainCamera.transform.position.x, easings[0].parameter.time, easings[0].parameter.type);
        easings[1].Set(mainCamera.transform.position.y, Get(history.next).GetPosition().y - mainCamera.transform.position.y, easings[1].parameter.time, easings[1].parameter.type);
        Get(history.next).system.SetActive(true);
    }
    private void ObjectMove()
    {
        mainCamera.transform.position = new Vector3(easings[0].Move(), easings[1].Move(), -10);
    }
    private Target Get(CanvasTag tag)
    {
        for(int y = 0;y < targets.Length;++y)
        {
            for (int x = 0; x < targets[y].target.Length; ++x)
            {
                if (targets[y].target[x].tag == tag)
                {
                    return targets[y].target[x];
                }
            }
        }
        return null;
    }
    private Vector2Int GetNumber(CanvasTag tag)
    {
        for (int y = 0; y < targets.Length; ++y)
        {
            for (int x = 0; x < targets[y].target.Length; ++x)
            {
                if (targets[y].target[x].tag == tag)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return new Vector2Int();
    }
    private CanvasTag GetTag(Vector2Int pos)
    {
        return targets[pos.y].target[pos.x].tag;
    }
    private bool Up()
    {
        --nowNumver.y;
        if (0 < nowNumver.y)
        {
            nowNumver.y = 0;
            return false;
        }
        BeyondArrayCheck();
        return true;
    }
    private bool Down()
    {
        ++nowNumver.y;
        if (targets.Length >= nowNumver.y)
        {
            nowNumver.y = targets.Length - 1;
            return false;
        }
        BeyondArrayCheck();
        return true;
    }
    private bool Left()
    {
        --nowNumver.x;
        if (0 > nowNumver.x)
        {
            nowNumver.x = 0;
            return false;
        }
        return true;
    }
    private bool Right()
    {
        ++nowNumver.x;
        if (targets[nowNumver.y].target.Length <= nowNumver.x)
        {
            nowNumver.x = targets[nowNumver.y].target.Length - 1;
            return false;
        }
        return true;
    }
    private void BeyondArrayCheck()
    {
        //横に移動したときに配列数よりも大きい値になっていた場合に配列数-1に補正をかける処理
        if (targets[nowNumver.y].target.Length <= nowNumver.x)
        {
            nowNumver.x = targets[nowNumver.y].target.Length - 1;
        }
    }
    public bool ChangeTag(CanvasTag tag)
    {
        //移動中は判定を無視する
        if(easings[0].IsPlay() || easings[1].IsPlay())
        {
            return false;
        }
        nowNumver = GetNumber(tag);
        InputMove();
        return true;
    }
}
