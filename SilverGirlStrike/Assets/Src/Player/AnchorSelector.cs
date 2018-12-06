using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorSelector : MonoBehaviour {
    public float maxAngle;

    CircleDitection ditector;
    Vector3 startPoint;
    Vector3 goalPoint;
    GameObject targetAnchor = null;

    // Use this for initialization
    void Start () {
        this.ditector = this.GetComponent<CircleDitection>();

	}
	
	// Update is called once per frame
	void Update () {
    }


    //アンカーを見つけ出す
    public void FindAnchor(Vector2 start, Vector2 goal, out GameObject result)
    {
        this.startPoint = start;
        this.goalPoint = goal;
        result = null;
        //アンカー選びにおける優先ポイント、低いほど優先する
        float selectScore = Mathf.Infinity;


        var list = this.ditector.FindAnchor();
        for (int i = 0; i < list.Length; ++i)
        {
            //距離
            float angle = Vector3.Angle((this.goalPoint - this.startPoint), (list[i].transform.position - this.startPoint));
            float dis = lineToPointDistance(this.startPoint, this.goalPoint, list[i].transform.position, true);

            //近すぎるもの、角度が一定以上のものを省く
            if (angle > this.maxAngle || (list[i].transform.position - this.startPoint).magnitude < 1.0f)
            {
                continue;
            }

            //レイを飛ばして、壁に激突していないかをチェック
            if(Physics2D.Raycast(this.startPoint,
                (list[i].transform.position - this.startPoint).normalized,
                (list[i].transform.position - this.startPoint).magnitude,
                (int)M_System.LayerName.GROUND | (int)M_System.LayerName.PLAYERWALL))
            {
                continue;
            }

            //デバッグ用
            Debug.DrawRay(this.startPoint, (list[i].transform.position - this.startPoint), Color.yellow, 1);
            Debug.DrawRay(this.startPoint, (this.goalPoint - this.startPoint), Color.yellow, 1);
            Debug.Log("angle" + angle);


            float score = 0.0f;
            //角度がついているほど得点が低くなる
            score += angle;
            //距離があるほど得点が低くなる
            score += dis * 2;

            if (score < selectScore)
            {
                selectScore = score;
                result = list[i].gameObject;
            }
        }
    }



    //数学関数
    Vector2 nearestPointOnLine(Vector2 p1, Vector2 p2, Vector2 p, bool isSegment = true)
    {
        Vector2 d = p2 - p1;
        if (d.sqrMagnitude == 0) return p1;
        float t = (d.x * (p - p1).x + d.y * (p - p1).y) / d.sqrMagnitude;
        if (isSegment)
        {
            if (t < 0) return Vector2.negativeInfinity; if (t > 1) return p2;
        }
        Vector2 c = new Vector2((1 - t) * p1.x + t * p2.x, (1 - t) * p1.y + t * p2.y);
        return c;
    }
    float lineToPointDistance(Vector2 p1, Vector2 p2, Vector2 p, bool isSegment = true)
    {
        return (p - nearestPointOnLine(p1, p2, p, isSegment)).magnitude;
    }


}
