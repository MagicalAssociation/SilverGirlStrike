using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//近接攻撃システム
//攻撃関数を呼ぶと、エフェクトを再生し、当たり判定を一定時間放つ
public class NarrowAttacker : MonoBehaviour {
    public CharacterObject userCharacter;
    public Collider2D attackCollition;
    public int attackPower;//攻撃力
    public int chainCount;//無敵を無視する連鎖値
    public int attackTime;//攻撃判定が出続ける時間

    //攻撃対象のレイヤー
    public M_System.LayerName targetLayer;

    //ヒット時の効果音
    public string hitSound;

    //ヒット時のエフェクト
    public string hitEffectName;
    public Vector3 hitEffectScale;
    //エフェクトの回転
    public int effectRotation;
    //回転の振れ幅
    public int effectRotationRange;


    //当たった相手
    Collider2D[] hitResult;

    AttackData attackData;

    int timeCnt;
    bool isAttack;
    bool isHit;


	// Use this for initialization
	void Start () {
        EndAttack();
        this.attackData = new AttackData(this.userCharacter);
        this.attackData.power = this.attackPower;
        this.attackData.chain = this.chainCount;
        //流石に50以上の敵にはヒットしないっしょ・・・
        this.hitResult = new Collider2D[50];
    }
	
	// Update is called once per frame
	void Update () {
        //攻撃していないときは処理をしない
        if (!this.isAttack)
        {
            return;
        }

        AttackJudge((int)this.targetLayer);

        ++this.timeCnt;
        //攻撃終了
        if(this.timeCnt >= this.attackTime)
        {
            EndAttack();
        }
	}


    //判定を無効化
    public void EndAttack()
    {
        this.timeCnt = 0;
        this.isAttack = false;
        this.attackCollition.enabled = false;
    }
    //判定を有効化
    public void StartAttack()
    {
        this.timeCnt = 0;
        this.isAttack = true;
        this.attackCollition.enabled = true;
        this.isHit = false;
    }
    public bool IsHit()
    {
        return this.isHit;
    }

    //実際のダメージ処理
    public bool AttackJudge(int targetLayerID)
    {
        bool isHitCollition = false;
        this.isHit = false;

        //レイヤーマスク設定
        LayerMask layer = new LayerMask();
        layer.value = targetLayerID;

        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.SetLayerMask(layer);
        contactFilter2D.useTriggers = true;
        int resultLength = Physics2D.OverlapCollider(this.attackCollition, contactFilter2D, this.hitResult);

        isHitCollition = resultLength > 0;
        //ダメージ処理
        for (int i = 0; i < resultLength; ++i)
        {
            var obj = this.hitResult[i].GetComponent<CharacterObject>();
            if (obj == null)
            {
                continue;
            }

            bool result = obj.Damage(this.attackData);

            if (result)
            {
                //ヒットしたのでフラグを立てる
                this.isHit = true;
                if (this.hitEffectName != "")
                {
                    Vector3 pos = obj.transform.position + new Vector3(0.0f, 0.0f, -1.0f);
                    float a = Random.Range(0.0f, this.effectRotationRange);
                    a -= a * 0.5f;
                    Quaternion rot = Quaternion.AngleAxis(this.effectRotation + a, Vector3.forward);
                    Effect.Get().CreateEffect(this.hitEffectName, pos, rot, this.hitEffectScale);
                }
                if (this.hitSound != "")
                {
                    Sound.PlaySE(this.hitSound);
                }
            }
        }

        return isHitCollition;
    }
}
