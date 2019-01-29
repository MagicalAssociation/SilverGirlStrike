using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentDataInit : MonoBehaviour
{
    public void Create()
    {
        CurrentData.CreateData();
    }
}
public class CurrentData
{
    public enum ItemDirection : int
    {
        UP = 0,
        DOWN = 1,
        LEFT = 2,
        RIGHT = 3,
    }

    static Data instance = null;

    public static Data GetDataInstance()
    {
        return instance;
    }
    public static void CreateData()
    {
        if (instance == null)
        {
            instance = new Data();
        }
    }
    static SGS.Item[] startItemData = new SGS.Item[4];
    public static void CreateStartItemData()
    {
        for (int i = 0; i < startItemData.Length; ++i)
        {
            if (startItemData[i] == null)
            {
                startItemData[i] = new SGS.Item();
            }
        }
    }
    public static void ResetItemData()
    {
        for (int i = 0; i < startItemData.Length; ++i)
        {
            startItemData[i] = null;
        }
    }
    public static SGS.Item GetItemData(int num)
    {
        return startItemData[num];
    }
    public static void SetItemData(int num,SGS.Item item)
    {
        startItemData[num] = item;
    }
    public class Data
    {
        Save.DataParameter data;
        public Data()
        {
            data = new Save.DataParameter();
        }
        public void SetData(Save.DataParameter parameter)
        {
            data = parameter;
        }
        public Save.DataParameter GetData()
        {
            return this.data;
        }
    }
}
