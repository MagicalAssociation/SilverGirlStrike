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
        //初期化は一回だけ、複数回オブジェクトが置かれるのを防ぐ
        if (frag)
        {
            Destroy(this.gameObject);
        }

        M_System.frag = true;

        //シーン移行でも消さない
        DontDestroyOnLoad(this);
        Application.targetFrameRate = 60; //60FPSに設定
        M_System.input = new SystemInput();

        //音の設定を行う
        GetComponent<SoundInitializer>().CreateSoundSource();

        Cursor.visible = true;
    }

    private void Start()
    {
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            Application.Quit();
        }
    }

    private void FixedUpdate()
    {
        M_System.input.Update();
    }
}