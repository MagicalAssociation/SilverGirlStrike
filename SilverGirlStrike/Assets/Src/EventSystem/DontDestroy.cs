using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//現在のシーンを再読み込みしても消えないようにするスクリプト、別のシーンなら普通に消える
public class DontDestroy : MonoBehaviour {
    [SerializeField]
    private GameObject dontDestroyObject;
    private string currentSceneName;


	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(this.dontDestroyObject);
        
        // イベントにイベントハンドラーを追加
        SceneManager.activeSceneChanged += ActiveSceneChanged;

        this.currentSceneName = SceneManager.GetActiveScene().name;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    // イベントハンドラー（イベント発生時に動かしたい処理）
    void ActiveSceneChanged(Scene thisScene, Scene nextScene)
    {
        //自分のシーンを再読み込みした場合は削除しない
        if (currentSceneName != nextScene.name)
        {
            Debug.Log(currentSceneName);
            Debug.Log(nextScene.name);
            Destroy(this.dontDestroyObject);
        }
    }
}
