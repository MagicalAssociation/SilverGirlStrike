using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//シーン上に配置する、Inspectorで管理可能なイベントマネージャー
public class EventManagerObject : MonoBehaviour {
    public TextEvent.EventGameData gameData;
    public string[] eventPath;


	// Use this for initialization
	void Awake () {
        //シングルトン生み出し
        TextEvent.EventManager.Create(this.gameData);

        //イベント読み込み＆登録
        foreach(var i in this.eventPath)
        {
            TextEvent.EventPerser eventReader = new TextEvent.EventPerser(i);
            var text = eventReader.GetEventText();
            TextEvent.EventManager.Get().AddEvent(text);
        }
	}
	
	// Update is called once per frame
	void Update () {
        //アップデートはここで
        TextEvent.EventManager.Get().Update();
	}
}
