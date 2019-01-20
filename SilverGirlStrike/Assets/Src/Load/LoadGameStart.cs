﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LoadGameStart : CursorParam
{
    public Image image;
    public Text text;
    private void Start()
    {
    }
    IEnumerator GameStart()
    {
        yield return new WaitForSeconds(0.0f);
        SceneManager.LoadScene("GameScene");
    }
    public override void Decision()
    {
        if (GetComponent<Select>().GetData() != null)
        {
            M_System.currentData.SetData(GetComponent<Select>().GetData());
            StartCoroutine(GameStart());
        }
    }
}
