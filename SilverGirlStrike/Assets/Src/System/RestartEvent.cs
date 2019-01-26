using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//ゲーム開始時の初期配置
//プレハブによって開始時に指定位置へ召喚する
public class RestartEvent : MonoBehaviour {
    private static bool isExist = false;

    private List<Transform> startPoints;

    [SerializeField]
    private Transform[] serializeStartPoint;
    [SerializeField]
    private int startPointIndex;

    //配置するプレイヤーキャラ
    [SerializeField]
    private CharacterObject playerObj;
    private CharacterManager characterManager;
    [SerializeField]
    private string cameraObjName;
    private ObjectChaser cameraObj;

    //登録されているシーンの名前（起動時に自動取得）
    private string currentSceneName;

    private void Awake()
    {
        //複数のオブジェクトがある場合は削除して一つになることを保証
        if (isExist)
        {
            Destroy(this.gameObject);
            return;
        }
        this.startPoints = new List<Transform>();
        //Inspectorの値を配列に追加
        foreach (var i in this.serializeStartPoint)
        {
            AddRestartPoint(i);
        }
    }
    void Start () {
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

        var obj = this.characterManager.GetCharacterTrans("Player");


        //z軸の座標は、初期値のまま動かさないので保持しておく
        float zPos = obj.position.z;
        Transform point = GetStartPoint(this.startPointIndex);
        obj.position = new Vector3(point.position.x, point.position.y, zPos);

        //登録
        var character = obj.gameObject.GetComponent<CharacterObject>();
        character.ChangeState((int)Fuchan.PlayerObject.State.START);


        //プレイヤーをカメラが追従するように設定
        var chaseObj = this.characterManager.GetCharacterTrans("PlayerCameraMan");
        cameraObj.SetTarget(chaseObj);
        cameraObj.gameObject.transform.position = character.transform.position + Vector3.up * 30.0f;
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

    //リスタート地点登録
    public int AddRestartPoint(Transform trans)
    {
        this.startPoints.Add(trans);
        return this.startPoints.Count - 1;
    }

    //リスタート地点設定
    public void SetRestartPointIndex(int index)
    {
        if(index < 0 || index >= this.startPoints.Count)
        {
            //配列外は無視
            return;
        }

        this.startPointIndex = index;
    }

    Transform GetStartPoint(int index)
    {
        return this.startPoints[index];
    }

}
