using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataParameter
{
    int money;
    bool[] stageClearFlag;
}
public class GameData : MonoBehaviour
{
    public string[] filePaths;
    static public void Save(string filePath)
    {

    }
    static public GameData Load(string filePath)
    {
        return new GameData();
    }
    public string GetFilePath(int n)
    {
        if (n < 0 || filePaths.Length <= n)
        {
            return null;
        }
        return filePaths[n];
    }
}
