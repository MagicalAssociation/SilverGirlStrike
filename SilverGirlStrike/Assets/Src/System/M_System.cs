using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * file     M_System.cs
 * brief    自分たちで作ったシステムの宣言
 * date     2018/10/09
 * author   Shou Kaneko
 * 
 */
public class M_System : MonoBehaviour
{
    //! 入力を扱う公開変数
    static public SystemInput input;

    public enum LayerName : int
    {
        PLAYER = 1 << 8,
        ENEMY = 1 << 9,
        GROUND = 1 << 10,
        ANCHOR = 1 << 11,
    }

    /**
     * brief    初期化
     */
    private void Awake()
    {
        //シーン移行でも消さない
        DontDestroyOnLoad(this);
        Application.targetFrameRate = 60; //60FPSに設定
        M_System.input = new SystemInput();

        //音の設定を行う
        GetComponent<SoundInitializer>().CreateSoundSource();

        Time.fixedDeltaTime = 1.0f / 60.0f;
        Time.maximumDeltaTime = 1.0f / 30.0f;
        QualitySettings.vSyncCount = 0;
    }

    private void Start()
    {
        Sound.PlayBGM("opStageBGM");
    }

    private void Update()
    {
        M_System.input.Update();
    }
}