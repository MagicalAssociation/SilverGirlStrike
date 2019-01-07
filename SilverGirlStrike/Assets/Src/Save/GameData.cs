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
    static GameData gameData;
    public void Save()
    {

    }
    public void Load()
    {

    }
    static public void Create()
    {
        if(GameData.gameData == null)
        {
            GameData.gameData = new GameData();
        }
    }
    static public GameData Get()
    {
        return GameData.gameData;
    }
}
