using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorSystemManager : MonoBehaviour
{
    public CursorSystem[] systems;
    private CursorSystem nowSystem;
    private void Start()
    {
        
    }
    private void Update()
    {
        nowSystem.SystemUpdate();
    }
}
