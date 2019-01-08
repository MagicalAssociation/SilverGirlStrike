using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataParameter
{
    int money;
    bool[] stageClearFlag;
}
public class GameData
{
    public enum File
    {
        SAVE1,
        SAVE2,
        SAVE3,
    }
    static public void Save(string filePath)
    {

    }
    static public GameData Load(string filePath)
    {
        return new GameData();
    }
}
