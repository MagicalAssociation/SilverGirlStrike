﻿using System.Collections;
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
        Application.targetFrameRate = 60; //60FPSに設定
        M_System.input = new SystemInput();
    }

    private void Update()
    {
        M_System.input.Update();
    }
}