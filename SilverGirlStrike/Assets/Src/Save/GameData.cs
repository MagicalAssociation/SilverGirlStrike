using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
namespace Save
{
    [System.Serializable]
    public class DataParameter
    {
        [System.Serializable]
        public class ItemData
        {
            public int id;
            public int num;
            public ItemData()
            {

            }
            public ItemData(int id,int num)
            {
                this.id = id;
                this.num = num;
            }
        }
        public DataParameter()
        {
            gold = 0;
            stageClearFlag = new int[6];
            filePath = "";
            itemData = new List<ItemData>();
        }
        public int gold;
        public int[] stageClearFlag;
        public string filePath;
        public List<ItemData> itemData;
    }
}
public class CurrentData
{
    static CurrentData singleton;
    public static void Create()
    {
        if(singleton == null)
        {
            singleton = new CurrentData();
        }
    }
    public static CurrentData Get()
    {
        return CurrentData.singleton;
    }
}
public class GameData : MonoBehaviour
{
    //セーブデータファイル名を保存してるファイルが破損したときに使う用
    static public void SaveNameCreate()
    {
        string path = Application.dataPath + @"\Resources\savename.txt";
        string reset = "save1\nsave2\nsave3\n";
        File.WriteAllText(path, reset, System.Text.Encoding.Unicode);
    }
    static public void Save(Save.DataParameter dataParameter)
    {
        //ファイルの中身をとりあえず空にする
        string path = Application.dataPath + @"\Resources\" + dataParameter.filePath + ".txt";
        string reset = "";
        File.WriteAllText(path, reset, System.Text.Encoding.Unicode);
        
        //そこからデータを保存していく
        string text = "gold " + dataParameter.gold.ToString() + "\n"; 
        File.AppendAllText(path, text,System.Text.Encoding.Unicode);
        for(int i = 0;i < dataParameter.stageClearFlag.Length;++i)
        {
            text = "stage " + (i + 1).ToString() + " " + dataParameter.stageClearFlag[i].ToString() + "\n";
            File.AppendAllText(path, text, System.Text.Encoding.Unicode);
        }
        for(int i = 0;i < dataParameter.itemData.Count;++i)
        {
            text = "item " + dataParameter.itemData[i].id.ToString() + " " + dataParameter.itemData[i].num.ToString() + "\n";
            File.AppendAllText(path, text, System.Text.Encoding.Unicode);
        }
    }
    static public Save.DataParameter Load(string filePath)
    {
        Save.DataParameter gameData = new Save.DataParameter();
        //Unityの機能で読み込む場合、Unityに登録するまでのラグでファイル欠損扱いされる場合があるのでC#の機能で読み込む
        string path = Application.dataPath + @"\Resources\" + filePath + ".txt";
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
                case "gold":
                    gameData.gold = int.Parse(text[1]);
                    break;
                case "stage":
                    gameData.stageClearFlag[int.Parse(text[1]) - 1] = int.Parse(text[2]);
                    break;
                case "item":
                    gameData.itemData.Add(new Save.DataParameter.ItemData(int.Parse(text[1]), int.Parse(text[2])));
                    break;
            }
        }
        gameData.filePath = filePath;
        return gameData;
    }
    static public string[] GetSaveFilePath()
    {
        string path = Application.dataPath + @"\Resources\savename.txt";
        string textasset = File.ReadAllText(path,System.Text.Encoding.Unicode);
        //1列ごとに分ける
       return textasset.Split('\n');
    }
}
