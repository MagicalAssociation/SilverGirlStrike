using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//複数の入力が存在するときにそれらを管理するclass
//同時に動かないよう、入力を制限かけるようにUpdateで呼ぶSystemを変更できるようにする
public class CursorSystemManager : MonoBehaviour
{
    public CursorSystem[] systems;
    public int startSystemNumber;
    private int nowSystem;
    private int preSystem;
    private int nextSystem;
    private void Start()
    {
        nowSystem = startSystemNumber % systems.Length;
    }
    private void Update()
    {
        Change();
        if(nowSystem < 0 || nowSystem >= systems.Length)
        {
            return;
        }
        systems[nowSystem].SystemUpdate(this);
    }
    public void Next(int next)
    {
        nextSystem = next;
    }
    private void Change()
    {
        if(nowSystem != nextSystem)
        {
            preSystem = nowSystem;
            systems[preSystem].Exit();
            systems[nextSystem].Enter();
            nowSystem = nextSystem;
        }
    }
}
