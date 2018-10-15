using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Physics_ : MonoBehaviour {
    //任意のオブジェクトとだけの衝突を行う
    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope;
        public float slopeAngle, slopeAngleOld;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }
    public LayerMask layerMask;
    public float skinWitdh = 0.015f;
    public int xAxisCount = 4;
    public int yAxisCount = 4;
    public float maxClimbAngle = 80;
    float xAxis;
    float yAxis;
    BoxCollider2D boxCollider;
    RaycastOrigins raycast;
    public CollisionInfo collision;
	// Use this for initialization
	void Start () {
        this.boxCollider = GetComponent<BoxCollider2D>();
        this.CalculateRaySpacing();
        
	}
	void CalculateRaySpacing()
    {
        Bounds bounds = this.boxCollider.bounds;
        bounds.Expand(skinWitdh * -2);
        this.xAxisCount = Mathf.Clamp(xAxisCount, 2, int.MaxValue);
        this.yAxisCount = Mathf.Clamp(yAxisCount, 2, int.MaxValue);
        this.xAxis = bounds.size.x / (this.xAxisCount - 1);
        this.yAxis = bounds.size.y / (this.yAxisCount - 1);
    }
    void UpdateRaycast()
    {
        Bounds bounds = this.boxCollider.bounds;
        bounds.Expand(skinWitdh * -2);

        this.raycast.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        this.raycast.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        this.raycast.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        this.raycast.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }
    void XCollision(ref Vector3 v)
    {
        float dir = Mathf.Sign(v.x);
        float ray = Mathf.Abs(v.x) + this.skinWitdh;
        for(int i = 0;i < this.xAxisCount;++i)
        {
            Vector2 rayOrigin = (dir == -1) ? raycast.bottomLeft : raycast.bottomRight;
            //rayOrigin += Vector2.up * ()
        }
    }
    public void Move(Vector3 v)
    {
        this.UpdateRaycast();
        collision.Reset();
        if(v.x != 0)
        {

        }
        if(v.y != 0)
        {

        }
    }
	// Update is called once per frame
	void Update () {
		
	}
}
