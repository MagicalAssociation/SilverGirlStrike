using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Effectクラスの初期化を行うクラス
public class EffectInitializer : MonoBehaviour {

    [System.Serializable]
    public struct EffectList
    {
        public string key;
        public GameObject eff;
    }

    public EffectList[] effectlist;     //effectを格納するリスト

    // Use this for initialization
    void Start () {
        var effectManaegr = Effect.Get();
        foreach(var i in this.effectlist)
        {
            effectManaegr.AddEffect(i.key, i.eff);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
