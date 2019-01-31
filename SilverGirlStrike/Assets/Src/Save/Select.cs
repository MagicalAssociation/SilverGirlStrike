using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Select : MonoBehaviour
{
    Save.DataParameter dataParameter;
    Text text;
    private int stageNumver;
    private void Awake()
    {
        text = GetComponentInChildren<Text>();
    }
    public void TextChange(string text)
    {
        this.text.text = text;
    }
    public void SetData(Save.DataParameter data)
    {
        this.dataParameter = data;
    }
    public Save.DataParameter GetData()
    {
        return this.dataParameter;
    }
    public void SetStageNumber(int number)
    {
        this.stageNumver = number;
    }
    public int GetStageNumver()
    {
        return this.stageNumver;
    }
}
