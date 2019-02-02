using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//範囲内に入ったプレイヤーに、ひたすら炎を落とす人
//プレイヤーのY座標を一定距離に保って、一定間隔で発射
//倒せない
public class FallFireShooter : CharacterObject {
    [SerializeField]
    int attackIntervalCount;

    [SerializeField]
    int chargeCount;

    [SerializeField]
    CharacterObject bulletObject;

    //上方向距離
    [SerializeField]
    float upDistance;

    //感知コリジョン
    [SerializeField]
    Collider2D sensor;

    int count;
    //当たった相手
    Collider2D[] hitResult;
    Transform target;

    public override void ApplyDamage()
    {
    }

    public override bool Damage(AttackData attackData)
    {
        //ダメージとかない
        return false;
    }

    public override void MoveCharacter()
    {

    }

    public override void UpdateCharacter()
    {
        if (!CheckOverlapPlayer())
        {
            //範囲内に入ってない場合は処理しない
            this.count = 0;
            return;
        }
        this.target = this.hitResult[0].transform;
        ++this.count;

        //チャージ開始
        if(this.count == this.attackIntervalCount)
        {
            Effect.Get().CreateEffect("fireFallCharge", this.transform.position - Vector3.forward, Quaternion.AngleAxis(180.0f, Vector3.forward), Vector3.one);
            Sound.PlaySE("charge1");
        }

        //一定間隔に達した
        if (this.count > this.attackIntervalCount + this.chargeCount)
        {
            this.count = 0;
            var obj = Instantiate<CharacterObject>(this.bulletObject, new Vector3(this.transform.position.x, this.target.transform.position.y + this.upDistance, this.target.position.z), Quaternion.identity);
            FindManager().AddCharacter(obj);
            Sound.PlaySE("impact1");
            Sound.PlaySE("shot2");
        }
    }


    bool CheckOverlapPlayer()
    {
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.SetLayerMask((int)M_System.LayerName.PLAYER);
        contactFilter2D.useTriggers = true;
        int resultLength = Physics2D.OverlapCollider(this.sensor, contactFilter2D, this.hitResult);
        return resultLength > 0;
    }


    // Use this for initialization
    void Start () {
        this.count = 0;
        this.hitResult = new Collider2D[10];
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
