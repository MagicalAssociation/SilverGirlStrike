using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorSelector : MonoBehaviour {

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
        //真横に直線作成
        this.startPoint = this.transform.position;
        this.goalPoint = this.startPoint + new Vector3(1.0f, 0.0f, 0.0f);

        if (this.targetAnchor != null)
        {
            this.targetAnchor.gameObject.GetComponent<SpriteRenderer>().color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        }

        FindAnchor(this.startPoint, this.goalPoint, out this.targetAnchor);

        Debug.Log(targetAnchor);

        if (this.targetAnchor != null)
        {
            this.targetAnchor.gameObject.GetComponent<SpriteRenderer>().color = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
        }
    }


    public void FindAnchor(Vector2 start, Vector2 goal, out GameObject result)
    {
        result = null;
        float distance = Mathf.Infinity;

        var list = this.ditector.FindAnchor();
        for (int i = 0; i < list.Length; ++i)
        {
            float dis = lineToPointDistance(this.startPoint, this.goalPoint, list[i].transform.position, false);
            if (dis < distance && dis >= 0.0f)
            {
                distance = dis;
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
            if (t < 0) return p1; if (t > 1) return p2;
        }
        Vector2 c = new Vector2((1 - t) * p1.x + t * p2.x, (1 - t) * p1.y + t * p2.y);
        return c;
    }
    float lineToPointDistance(Vector2 p1, Vector2 p2, Vector2 p, bool isSegment = true)
    {
        return (p - nearestPointOnLine(p1, p2, p, isSegment)).magnitude;
    }


}
