using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/**
 * アイテムのデータはすべて外部ファイルを使用して行う
 * ファイルにはID,名前,説明,リソースID,固有パラメータ値の順でいれる
 * 回復アイテムの場合 [id]0 [name]Elixir [remarks]HPを少し回復する [resourceid]0 [回復値]5
 */ 

/**
 * file     Item.cs
 * brief    Item関連
 * author   Shou Kaneko
 * date     2018/11/24
*/

/**
 * brief    アイテムの基底クラス
 */
namespace SGS
{
    /**
     * enum ItemMode
     * brief    状態管理を行う定数
     */ 
    public enum ItemMode
    {
        //! Destroy
        KILL,
        //! Normal
        NORMAL,
        //! Stop Update
        STOP,
    }
    /**
     * brief アイテムの基底クラス
     */ 
     [System.Serializable]
    public class Item
    {
        [System.Serializable]
        public class ResourceData
        {
            public Sprite sprite;
            public int id;
            public static Sprite GetSprite(ResourceData[] resources,int id)
            {
                for(int i = 0;i < resources.Length;++i)
                {
                    if(resources[i].id == id)
                    {
                        return resources[i].sprite;
                    }
                }
                return null;
            }
        }
        //使用者
        public CharacterObject master;        
        //個別ID
        private int id;
        //アイテム名
        public string name;
        //説明
        public string remarks;
        //リソースID
        private int resourceID;
        //実際のリソース情報
        private Sprite sprite;
        //個数
        private int num;
        //無限
        private bool infinite;
        /**
         * enum LinesItemNumber
         */ 
         public enum Type
        {
            ID = 0,
            NAME = 1,
            REMARKS = 2,
            R_ID = 3,
        }
        /**
         * brief    使用時の効果を記述
         */ 
        public virtual void Use() { }
        /**
         * brief    初期化
         */
         public virtual void Init() { }
        public void SetData(int id,string name,string remarks,int resourceid)
        {
            this.id = id;
            this.name = name;
            this.remarks = remarks;
            this.resourceID = resourceid;
        }
        public void SetData(string[] data)
        {
            this.id = int.Parse(data[0]);
            this.name = data[1];
            this.remarks = data[2];
            this.resourceID = int.Parse(data[3]);
        }
        public static string[] Load(int id)
        {
            string[] textMassage = Item.Load();
            if (textMassage != null)
            {
                for (int i = 0; i < textMassage.Length; ++i)
                {
                    string[] text = textMassage[i].Split(' ');
                    if (int.Parse(text[0]) == id)
                    {
                        return text;
                    }
                }
            }
            return null;
        }
        public static string[] Load()
        {
            string path = Application.dataPath + @"\Resources\ItemData.txt";
            string textasset = "";
            string[] error = new string[4];
            error[0] = "-1";
            error[1] = "Error";
            error[2] = "Failed to read data";
            error[3] = "-1";
            try
            {
                textasset = File.ReadAllText(path, System.Text.Encoding.Unicode);
            }
            catch
            {
                return null;
            }
            return textasset.Split('\n');
        }
        public static int GetID(string line)
        {
            string[] text = line.Split(' ');
            return int.Parse(text[0]);
        }
        public static string GetName(string line)
        {
            string[] text = line.Split(' ');
            return text[1];
        }
        public static int GetResourceID(string line)
        {
            string[] text = line.Split(' ');
            return int.Parse(text[3]);
        }
        public void DebugLog()
        {
            Debug.Log("ID" + this.id + " Name" + this.name + " S" + this.remarks + " RID" + this.resourceID);
        }
        public int GetID()
        {
            return this.id;
        }
        public int GetResourceID()
        {
            return this.resourceID;
        }
        public void SetSprite(Sprite image)
        {
            sprite = image;
        }
        public Sprite GetSprite()
        {
            return sprite;
        }
        public void SetNumver(int n)
        {
            this.num = n;
        }
        public int GetNumver()
        {
            return this.num;
        }
    }
    /**
     * brief    設置するItem
     */
     [System.Serializable]
    public abstract class ItemObject : MonoBehaviour
    {
        public M_System.LayerName[] targets;
        //! 使用時効果等ある場合のための変数
        Item item;
        //! 状態管理
        ItemMode mode;
        //! 当たり判定を行うためのCollider
        public Collider2D collider;
        //名前とかを取得するときのID
        public int id;
        /**
         * @brief   初期化処理
         */
         public void Init(Item item = null)
        {
            this.mode = ItemMode.NORMAL;
            this.item = item;
        }
        /**
         * brief    更新処理
         * フィールド上で動きを付ける時などに使用
         */
        public abstract void UpdateItem();
        /**
         * brief    当たった時の動作を記述する
         */
        public abstract void Enter(GameObject hitObject);
        /**
         * brief    アイテムデータを登録する
         * param[in] ref Item item アイテムデータ
         */
        public void SetItemData(Item item)
        {
            this.item = item;
        }
        /**
         * brief    アイテムデータを取得する
         * return Item 登録されているアイテムデータ
         */
        public Item GetItemData()
        {
            return this.item;
        }
        /**
         * brief    状態を取得
         * return ItemMode Mode
         */
        public ItemMode GetItemMode()
        {
            return this.mode;
        }
        /**
         * brief    状態を変更する
         * param[in] ItemMode mode 指定Mode
         */
        public void ChangeMode(ItemMode mode)
        {
            this.mode = mode;
        }
        public void Destory()
        {
            this.mode = ItemMode.KILL;
        }
    }
}