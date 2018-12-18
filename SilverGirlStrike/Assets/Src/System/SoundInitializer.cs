using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundInitializer : MonoBehaviour {

    [System.Serializable]
    public struct SoundDictionary
    {
        public string key;
        public AudioClip audio;
    }

    public SoundDictionary[] bgmList;
    public SoundDictionary[] seList;

	// Use this for initialization
	public void CreateSoundSource () {
        //読み込み
        foreach(var i in this.bgmList)
        {
            Sound.LoadBGM(i.key, i.audio);
        }
        foreach (var i in this.seList)
        {
            Sound.LoadSE(i.key, i.audio);
        }
    }
}
