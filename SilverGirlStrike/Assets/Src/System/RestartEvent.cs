using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//ゲーム開始時の初期配置
//プレハブによって開始時に指定位置へ召喚する
public class RestartEvent : MonoBehaviour {
    private static bool isExist = false;

    [SerializeField]
    private Transform startPoint;

    //配置するプレイヤーキャラ
    [SerializeField]
    private CharacterObject playerObj;
    private CharacterManager characterManager;
    [SerializeField]
    private string cameraObjName;
    private ObjectChaser cameraObj;

    //登録されているシーンの名前（起動時に自動取得）
    private string currentSceneName;

    void Start () {
        //複数のオブジェクトがある場合は削除して一つになることを保証
        if (isExist)
        {
            Destroy(this.gameObject);
            return;
        }
        //初期化
        DontDestroySetting();
        CreatePlayerObject();
    }

    // イベントハンドラー（イベント発生時に動かしたい処理）
    void ActiveSceneChanged(Scene thisScene, Scene nextScene)
    {
        //自分のシーンを再読み込みした場合は削除しない
        if (currentSceneName != nextScene.name)
        {
            Debug.Log(currentSceneName);
            Debug.Log(nextScene.name);
            Destroy(this.gameObject);
        }
    }


    private void CreatePlayerObject()
    {
        //仕方がないのでFindで取得しておく
        this.characterManager = GameObject.Find(M_System.characterManagerObjectName).GetComponent<CharacterManager>();
        this.cameraObj = GameObject.Find(this.cameraObjName).GetComponent<ObjectChaser>();

        //生成
        var obj = Instantiate(this.playerObj);

        //z軸の座標は、初期値のまま動かさないので保持しておく
        float zPos = obj.transform.position.z;
        obj.transform.position = new Vector3(this.startPoint.position.x, this.startPoint.position.y, zPos);

        //登録
        var character = obj.GetComponent<CharacterObject>();
        this.characterManager.AddCharacter(character);

        //プレイヤーをカメラが追従するように設定
        cameraObj.SetTarget(obj.transform);
    }

    private void DontDestroySetting()
    {
        //リトライしても破棄されない（自分のシーンを再読み込みしたときのみ）
        DontDestroyOnLoad(this.gameObject);

        // イベントにイベントハンドラーを追加
        SceneManager.activeSceneChanged += ActiveSceneChanged;
        SceneManager.sceneLoaded += Init;

        this.currentSceneName = SceneManager.GetActiveScene().name;
        isExist = true;
    }

    private void Init(Scene scene, LoadSceneMode mode)
    {
        //初期化
        CreatePlayerObject();
    }
}
