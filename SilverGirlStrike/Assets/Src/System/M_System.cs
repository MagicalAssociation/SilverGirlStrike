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
    int screenFrag;


    static bool frag = false;

    static M_System myself;

    //! 入力を扱う公開変数
    static public SystemInput input;

    public enum LayerName : int
    {
        PLAYER = 1 << 8,
        ENEMY = 1 << 9,
        GROUND = 1 << 10,
        ANCHOR = 1 << 11,
        ITEM = 1 << 12,
        FOOT = 1 << 13,
        BULLET = 1 << 14,
        PLAYERWALL = 1 << 15,
    }


    public const string characterManagerObjectName = "CharacterManager";

    /**
     * brief    初期化
     */
    private void Awake()
    {
        this.screenFrag = 0;

        //初期化は一回だけ、複数回オブジェクトが置かれるのを防ぐ
        if (frag)
        {
            Destroy(this.gameObject);
            return;
        }

        M_System.frag = true;

        Screen.SetResolution(960, 540, false);

        //シーン移行でも消さない
        DontDestroyOnLoad(this);
        Application.targetFrameRate = 60; //60FPSに設定
        M_System.input = new SystemInput();
        GetComponent<CurrentDataInit>().Create();
        //音の設定を行う
        GetComponent<SoundInitializer>().CreateSoundSource();

        Cursor.visible = true;
    }

    private void Start()
    {
    }

    private void Update()
    {
        M_System.input.Update();
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SwitchScreen();
        }
    }

    //F1を推すたびにスクリーン設定が入れ替わる
    void SwitchScreen()
    {
        ++this.screenFrag;
        if(this.screenFrag > 2)
        {
            this.screenFrag = 0;
        }

        switch (this.screenFrag)
        {
            case 0:
                Screen.SetResolution(960, 540, false);
                break;
            case 1:
                Screen.SetResolution(960, 540, true);
                break;
            case 2:
                Screen.SetResolution(1920, 1080, true);
                break;
        }
    }
}