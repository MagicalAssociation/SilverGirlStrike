using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// サウンド管理
public class Sound
{

    /// SEチャンネル数
    const int SE_CHANNEL = 4;

    /// サウンド種別
    enum eType
    {
        Bgm, // BGM
        Se,  // SE
    }

    // シングルトン
    static Sound singleton = null;
    // インスタンス取得
    public static Sound GetInstance()
    {
        if(singleton != null)
        {
            return singleton;
        }

        return singleton = new Sound();
    }

    // サウンド再生のためのゲームオブジェクト
    GameObject gameObject = null;
    // サウンドリソース
    AudioSource sourceBgm = null; // BGM
    AudioSource sourceSeDefault = null; // SE (デフォルト)
    AudioSource[] sourceSeArray; // SE (チャンネル)

    // BGMにアクセスするためのテーブル
    Dictionary<string, SoundData> poolBgm = new Dictionary<string, SoundData>();
    // SEにアクセスするためのテーブル 
    Dictionary<string, SoundData> poolSe = new Dictionary<string, SoundData>();

    /// 保持するデータ
    class SoundData
    {
        /// アクセス用のキー
        public string key;
        /// AudioClip
        public AudioClip clip;

        /// コンストラクタ
        public SoundData(string key, string res)
        {
            this.key = key;
            string ResName = "Sounds/" + res;
            // AudioClipの取得
            clip = Resources.Load(ResName) as AudioClip;
        }
        /// コンストラクタ
        public SoundData(string key, AudioClip clip)
        {
            this.key = key;
            // AudioClipの取得
            this.clip = clip;
        }
    }

    /// コンストラクタ
    public Sound()
    {
        // チャンネル確保
        sourceSeArray = new AudioSource[SE_CHANNEL];
    }

    // サウンドのロード
    // ※Resources/Soundsフォルダに配置すること
    public static void LoadBGM(string key, string resName)
    {
        GetInstance().LoadBGMPrivate(key, resName);
    }
    public static void LoadBGM(string key, AudioClip audio)
    {
        GetInstance().LoadBGMPrivate(key, audio);
    }
    public static void LoadSE(string key, string resName)
    {
        GetInstance().LoadSEPrivate(key, resName);
    }
    public static void LoadSE(string key, AudioClip audio)
    {
        GetInstance().LoadSEPrivate(key, audio);
    }

    /// BGMの再生
    /// ※事前にLoadBgmでロードしておくこと
    public static bool PlayBGM(string key)
    {
        return GetInstance().PlayBGMPrivate(key);
    }

    /// BGMの停止
    public static bool StopBGM()
    {
        return GetInstance().StopBGMPrivate();
    }

    /// SEの再生
    /// ※事前にLoadSeでロードしておくこと
    public static bool PlaySE(string key, int channel = -1)
    {
        return GetInstance().PlaySEPrivate(key, channel);
    }

    /// AudioSourceを取得する
    AudioSource GetAudioSource(eType type, int channel = -1)
    {
        if (gameObject == null)
        {
            // GameObjectがなければ作る
            gameObject = new GameObject("Sound");
            // 破棄しないようにする
            GameObject.DontDestroyOnLoad(gameObject);
            // AudioSourceを作成
            sourceBgm = gameObject.AddComponent<AudioSource>();
            sourceSeDefault = gameObject.AddComponent<AudioSource>();
            for (int i = 0; i < SE_CHANNEL; i++)
            {
                sourceSeArray[i] = gameObject.AddComponent<AudioSource>();
            }
        }

        if (type == eType.Bgm)
        {
            // BGM
            return sourceBgm;
        }
        else
        {
            // SE
            if (0 <= channel && channel < SE_CHANNEL)
            {
                // チャンネル指定
                return sourceSeArray[channel];
            }
            else
            {
                // デフォルト
                return sourceSeDefault;
            }
        }
    }
    //BGM読み込み
    void LoadBGMPrivate(string key, string resName)
    {
        if (poolBgm.ContainsKey(key))
        {
            // すでに登録済みなのでいったん消す
            poolBgm.Remove(key);
        }
        poolBgm.Add(key, new SoundData(key, resName));
    }
    void LoadBGMPrivate(string key, AudioClip audio)
    {
        if (poolBgm.ContainsKey(key))
        {
            // すでに登録済みなのでいったん消す
            poolBgm.Remove(key);
        }
        poolBgm.Add(key, new SoundData(key, audio));
    }
    //SE読み込み
    void LoadSEPrivate(string key, string resName)
    {
        if (poolSe.ContainsKey(key))
        {
            // すでに登録済みなのでいったん消す
            poolSe.Remove(key);
        }
        poolSe.Add(key, new SoundData(key, resName));
    }
    void LoadSEPrivate(string key, AudioClip audio)
    {
        if (poolSe.ContainsKey(key))
        {
            // すでに登録済みなのでいったん消す
            poolSe.Remove(key);
        }
        poolSe.Add(key, new SoundData(key, audio));
    }

    //BGM再生
    bool PlayBGMPrivate(string key)
    {
        if (poolBgm.ContainsKey(key) == false)
        {
            // 対応するキーがない
            return false;
        }

        // いったん止める
        StopBGM();

        // リソースの取得
        var _data = poolBgm[key];

        // 再生
        var source = GetAudioSource(eType.Bgm);
        source.loop = true;
        source.clip = _data.clip;
        source.Play();

        return true;
    }

    //BGM停止
    bool StopBGMPrivate()
    {
        GetAudioSource(eType.Bgm).Stop();

        return true;
    }

    //SE再生
    bool PlaySEPrivate(string key, int channel = -1)
    {
        if (poolSe.ContainsKey(key) == false)
        {
            // 対応するキーがない
            return false;
        }

        // リソースの取得
        var _data = poolSe[key];

        if (0 <= channel && channel < SE_CHANNEL)
        {
            // チャンネル指定
            var source = GetAudioSource(eType.Se, channel);
            source.clip = _data.clip;
            source.Play();
        }
        else
        {
            // デフォルトで再生
            var source = GetAudioSource(eType.Se);
            source.PlayOneShot(_data.clip);
        }

        return true;
    }
}