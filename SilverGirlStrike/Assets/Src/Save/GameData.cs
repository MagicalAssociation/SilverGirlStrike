using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class DataParameter
{
    public DataParameter()
    {
        money = 0;
        stageClearFlag = new int[6];
    }
    public void DebugLog()
    {
        Debug.Log("money:" + this.money);
        for (int i = 0; i < stageClearFlag.Length;++i)
        {
            Debug.Log("flag:" + i.ToString() + ":" + stageClearFlag[i].ToString());
        }
    }
    public int money;
    public int[] stageClearFlag;
}
public class GameData : MonoBehaviour
{
    static public void SaveNameCreate()
    {

        string path = Application.dataPath + @"\Resources\savename.txt";
        string reset = "save1\nsave2\nsave3\n";
        File.WriteAllText(path, reset);
    }
    static public void Save(string filePath,DataParameter dataParameter)
    {
        //ファイルの中身をとりあえず空にする
        string path = Application.dataPath + @"\Resources\" + filePath + @".txt";
        string reset = "";
        File.WriteAllText(path, reset);
        //そこからデータを保存していく
        string text = "money " + dataParameter.money.ToString() + "\n"; 
        File.AppendAllText(path, text);
        for(int i = 0;i < dataParameter.stageClearFlag.Length;++i)
        {
            text = "stage " + (i + 1).ToString() + " " + dataParameter.stageClearFlag[i].ToString() + "\n";
            File.AppendAllText(path, text);
        }
    }
    static public DataParameter Load(string filePath)
    {
        DataParameter gameData = new DataParameter();
        //Unityの機能で読み込む場合、Unityに登録するまでのラグでファイル欠損扱いされる場合があるのでC#の機能で読み込む
        string path = Application.dataPath + @"\Resources\" + filePath + @".txt";
        string textasset = "";
        try
        {
            textasset = File.ReadAllText(path, System.Text.Encoding.Unicode);
        }
        catch
        {
            return null;
        }
        //1列ごとに分ける
        string[] textMassage = textasset.Split('\n');
        for (int i = 0; i < textMassage.Length; ++i)
        {
            //空白ごとに切り分ける、その後最初の文字列に応じて処理を行う
            string[] text = textMassage[i].Split(' ');
            switch (text[0])
            {
                case "money":
                    gameData.money = int.Parse(text[1]);
                    break;
                case "stage":
                    gameData.stageClearFlag[int.Parse(text[1]) - 1] = int.Parse(text[2]);
                    break;
            }
        }
        return gameData;
    }
    static public string[] GetSaveFilePath()
    {
        string path = Application.dataPath + @"\Resources\savename.txt";
        string textasset = File.ReadAllText(path);
        //1列ごとに分ける
       return textasset.Split('\n');
    }
}
