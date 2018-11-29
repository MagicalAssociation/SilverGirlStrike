using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//編集履歴
//2018/11/16 板倉：作成
//2018/11/24 金子：CharacterManagerにダメージ処理をテスト設置


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
    //登録予約、削除予約
    List<CharacterObject> nextAdd;//インスタンスで
    List<int> nextDelete;//IDで
    //管理するオブジェクト(アクセス用)
    Dictionary<string, CharacterData> objects;

    private void Awake()
    {
        this.objectList = new List<CharacterData>();
        this.nextAdd = new List<CharacterObject>();
        this.nextDelete = new List<int>();
        this.objects = new Dictionary<string, CharacterData>();

        if (this.characters)
        {
            for(int i = 0; i < this.characters.transform.childCount; ++i)
            {
                AddCharacter(this.characters.transform.GetChild(i).GetComponent<CharacterObject>());
            }
        }

    }

    public void FixedUpdate()
    {
        //全てのオブジェクトの更新を行う
        foreach (var characterData in this.objectList)
        {
            //物理系に干渉する動作はFixedUpdate()でやる
            characterData.character.MoveCharacter();
        }
    }

    public void Update()
    {
        //全てのオブジェクトの更新を行う
        foreach(var characterData in this.objectList)
        {
            characterData.character.UpdateCharacter();
            characterData.character.ApplyDamage();
            //追加処理：ダメージ適用
            characterData.character.GetData().hitPoint.DamageUpdate();
        }
        //予約されている登録処理を行う
        foreach(var addCharacter in this.nextAdd)
        {
            AddCharacterDirect(addCharacter);
        }
        //予約されている削除処理を行う
        foreach (var deleteCharacter in this.nextDelete)
        {
            DeleteCharacterDirect(deleteCharacter);
        }

        //リストを空に
        this.nextAdd.Clear();
        this.nextDelete.Clear();
    }

    //キャラクター追加予約
    public void AddCharacter(CharacterObject character)
    {
        this.nextAdd.Add(character);
    }
    //実際に生成
    void AddCharacterDirect(CharacterObject character)
    {
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
    }

    //キャラを消去予約
    public void DeleteCharacter(string characterName)
    {
        this.nextDelete.Add(this.objects[characterName].id);
    }
    //キャラを消去、リストからも外す(名前版)
    void DeleteCharacterDirect(string characterName)
    {
        var obj = this.objects[characterName];
        DeleteCharacterDirect(obj);
    }
    //キャラを消去、リストからも外す(ID版)
    void DeleteCharacterDirect(int characterID)
    {
        var obj = this.objectList[characterID];
        DeleteCharacterDirect(obj);
    }
    //キャラを消去、リストからも外す(CharacterData版)
    void DeleteCharacterDirect(CharacterData data)
    {
        Object.Destroy(data.character.gameObject);
        this.objectList[data.id] = null;
        this.objects[data.name] = null;
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


