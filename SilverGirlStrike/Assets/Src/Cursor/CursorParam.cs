using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class CursorParam : MonoBehaviour
{
    // 決定時の処理
    public abstract void Decision();
    public abstract void Enter();
    public abstract void Exit();
}
namespace SGS
{
    [System.Serializable]
    public class CursorColor
    {
        public Color selectImageColor;
        public Color notSelectcImageColor;
        public Color selectBackColor;
        public Color notSelectBackColor;
    }
}
