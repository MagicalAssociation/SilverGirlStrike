using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//編集履歴
//2018/11/16 板倉：作成



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
    //管理するオブジェクト(アクセス用)
    Dictionary<string, CharacterData> objects;

    private void Start()
    {
        this.objectList = new List<CharacterData>();
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
        }
    }

    //キャラクター追加
    public void AddCharacter(CharacterObject character)
    {
        int id = GetUseableID();
        //空きがないので何もできない
        if (id == -1)
        {
            this.objectList.Add(null);
            id = this.objectList.Count - 1;
        }
        //登録
        CharacterData data = new CharacterData();
        data.character = character;
        data.id = id;
        data.name = character.gameObject.name;
        this.objectList[data.id] = data;
        this.objects[data.name] = data;
    }

    //キャラを消去、リストからも外す
    public void DeleteCharacter(string characterName)
    {
        var obj = this.objects[characterName];

        Object.Destroy(obj.character.gameObject);
        this.objectList[obj.id] = null;
        this.objects[obj.name] = null;
    }

    //キャラクターへ直接アクセスする
    public CharacterObject.CharaData GetCharacterData(string characterName)
    {
        return this.objects[characterName].character.GetData();
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


