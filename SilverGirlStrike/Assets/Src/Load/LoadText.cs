using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LoadText : MonoBehaviour
{
    Text text;   
    private void Awake()
    {
        text = GetComponentInChildren<Text>();
    }
    public void TextChange(string text)
    {
        this.text.text = text;
    }
}
