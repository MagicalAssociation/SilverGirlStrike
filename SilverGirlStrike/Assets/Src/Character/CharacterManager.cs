using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//編集履歴
//2018/11/16 板倉：作成
//2018/11/24 金子：CharacterManagerにダメージ処理をテスト設置
//2018/11/30 板倉：予約制を廃止。代わりに、ループの初めに更新が必要なオブジェクトをリストで収集する


//AddCharacterで追加 / DeleteCharacterで削除
//GetCharacterDataでデータだけにアクセス

//キャラクターの追加やアクセスを行うマネージャー
public class CharacterManager : MonoBehaviour
{
    //登録情報とキャラクターへの参照を持つ
    class CharacterData
    {
        public int id;
        public string name;
        public CharacterObject character;
    }

    //初期配置のキャラクターが収まった親オブジェクト、子がキャラクターのものをすべて登録
    public GameObject characters;
    //管理するオブジェクト(走査用)
    List<CharacterData> objectList;
    //更新処理を行うオブジェクトの収集結果を格納するリスト
    List<CharacterData> activeCharacters;
    //削除が必要なオブジェクトを削除するリスト
    List<CharacterData> deadCharacters;

    //管理するオブジェクト(アクセス用)
    Dictionary<string, CharacterData> objects;

    private void Awake()
    {
        this.objectList = new List<CharacterData>();
        this.activeCharacters = new List<CharacterData>();
        this.deadCharacters = new List<CharacterData>();
        this.objects = new Dictionary<string, CharacterData>();

        FindChildren(this.characters);
    }

    private void FindChildren(GameObject root)
    {
        //子供を全て検索
        if (root == null)
        {
            return;
        }

        for (int i = 0; i < root.transform.childCount; ++i)
        {
            //子オブジェクトを獲得
            var obj = root.transform.GetChild(i);

            if (obj.childCount > 0 && obj.gameObject.activeSelf)
            {
                //子がいれば再起
                FindChildren(obj.gameObject);
            }

            //オブジェクトを追加
            if (obj.gameObject.activeSelf)
            {
                AddCharacter(obj.GetComponent<CharacterObject>());
            }
        }
    }

    public void FixedUpdate()
    {

    }

    public void Update()
    {
        //更新する必要があるオブジェクトを収集
        CollectCharacter();

        //全てのオブジェクトの更新を行う
        foreach (var characterData in this.activeCharacters)
        {
            characterData.character.UpdateCharacter();
            characterData.character.ApplyDamage();
            //追加処理：ダメージ適用
            characterData.character.GetData().hitPoint.DamageUpdate();
            characterData.character.MoveCharacter();

            if (characterData.character.IsDead())
            {
                this.deadCharacters.Add(characterData);
            }
        }

        //全てのオブジェクトの削除を行う
        foreach (var characterData in this.deadCharacters)
        {
            DeleteCharacterDirect(characterData);
        }

        //リストを空に
        this.activeCharacters.Clear();
        this.deadCharacters.Clear();
    }

    void CollectCharacter()
    {
        //更新する必要があるオブジェクトを収集
        foreach (var characterData in this.objectList)
        {
            if (characterData != null)
            {
                this.activeCharacters.Add(characterData);
            }
        }
    }

    //実際に生成
    public int AddCharacter(CharacterObject character)
    {
        if(character == null)
        {
            return -1;
        }
        Debug.Log(this.objectList.Count);

        int id = GetUseableID();
        //空きがないので何もできない
        if (id == -1)
        {
            this.objectList.Add(null);
            id = this.objectList.Count - 1;
        }
        //登録用データ作成
        CharacterData data = new CharacterData();
        data.character = character;
        data.id = id;
        data.name = character.gameObject.name;
        //登録
        this.objectList[data.id] = data;
        this.objects[data.name] = data;

        return id;
    }

    //キャラを消去、リストからも外す(名前版)
    public void DeleteCharacter(string characterName)
    {
        var obj = this.objects[characterName];
        DeleteCharacterDirect(obj);
    }
    //キャラを消去、リストからも外す(ID版)
    public void DeleteCharacter(int characterID)
    {
        var obj = this.objectList[characterID];
        DeleteCharacterDirect(obj);
    }
    //キャラを消去、リストからも外す(CharacterData版)
    void DeleteCharacterDirect(CharacterData data)
    {
        var characterData = data.character;

        Object.Destroy(data.character.gameObject);
        this.objectList[data.id] = null;
        this.objects[data.name] = null;

        characterData.Dispose();
    }


    public Transform GetCharacterTrans(string characterName)
    {
        return this.objects[characterName].character.transform;
    }

    //キャラクターへ直接アクセスする
    public CharacterObject.CharaData GetCharacterData(string characterName)
    {
        return this.objects[characterName].character.GetData();
    }
    //キャラクターへ直接アクセスする
    public CharacterObject.CharaData GetCharacterData(int characterID)
    {
        return this.objectList[characterID].character.GetData();
    }

    //現在の空いているIDを検索、空きがない場合は-1を返す
    int GetUseableID()
    {
        int count = this.objectList.Count;
        int index = -1;
        for(int i = 0; i < count; ++i)
        {
            if(this.objectList[i] == null)
            {
                index = i;
                break;
            }
        }
        return index;
    }
}


