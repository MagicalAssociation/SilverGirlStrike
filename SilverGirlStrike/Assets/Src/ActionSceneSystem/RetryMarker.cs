using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//リトライ地点のアニメ処理やらリスポーン位置変更やら
public class RetryMarker : MonoBehaviour {
    public Animator markerAnim;
    public RestartEvent restartEvent;
    int arrayIndex;

    //一度でも触れたかどうかのフラグ
    bool activeFrag;

    private void Awake()
    {
        this.activeFrag = false;
        this.arrayIndex = -1;
    }
    private void Start()
    {
        this.arrayIndex = this.restartEvent.AddRestartPoint(this.transform);
        if (this.activeFrag)
        {
            markerAnim.Play("RetryMarkerGet");
        }
    }

    // Update is called once per frame
    void Update () {
		
	}

    //Enterはたまに貫通するのでとりあえずStayでやってる
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag != "Player")
        {
            return;
        }

        if (this.activeFrag)
        {
            return;
        }

        //最初の一回だけ処理
        this.activeFrag = true;
        markerAnim.Play("RetryMarkerGet");
        restartEvent.SetRestartPointIndex(this.arrayIndex);


        CharacterObject obj = other.gameObject.GetComponent<CharacterObject>();
        //全回復
        obj.GetData().hitPoint.Recover(obj.GetData().hitPoint.GetMaxHP());

        Sound.PlaySE("machineSwitch");
        Sound.PlaySE("power1");
    }
}
