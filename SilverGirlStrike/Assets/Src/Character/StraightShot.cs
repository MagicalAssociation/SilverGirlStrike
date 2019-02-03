using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//まっすぐ弾を飛ばす弾自体の動作を行うクラス
//・飛ぶ速度
//・向き
//・射程（寿命ではない）
//・耐久値（付随して当たり判定が必要となる）
public class StraightShot : CharacterObject {
    //耐久値
    [SerializeField]
    int hitPoint;
    //無敵時間
    [SerializeField]
    int invincibleTime;

    //速度
    [SerializeField]
    float speed;

    //向きのベクトル
    [SerializeField]
    Vector2 moveVector;

    //攻撃情報
    [SerializeField]
    NarrowAttacker attackData;

    //キャラのムーバー
    [SerializeField]
    CharacterMover mover;

    //破壊エフェクト名（耐久値が切れた場合のエフェクト）
    [SerializeField]
    string endEffectName;

    public override void ApplyDamage()
    {
        //ダメージを適用
        GetData().hitPoint.DamageUpdate();
    }

    public override bool Damage(AttackData attackData)
    {
        return GetData().hitPoint.Damage(attackData.power, attackData.chain);
    }

    public override void MoveCharacter()
    {
        var moveVec = this.moveVector * this.speed;
        //直線に移動
        this.mover.UpdateVelocity(moveVec.x, moveVec.y, 0.0f, false);
    }

    public override void UpdateCharacter()
    {
        this.attackData.StartAttack();

        //設定値によって破壊処理を行うかどうかを分岐
        if(GetData().hitPoint.GetHP() <= 0 && this.hitPoint >= 0)
        {
            //破壊エフェクトを再生して死亡
            if(this.endEffectName != "")
            {
                Effect.Get().CreateEffect(this.endEffectName, this.transform.position, Quaternion.identity, Vector3.one);
            }
            KillMyself();
        }

    }

    // Use this for initialization
    void Start () {
        GetData().hitPoint.SetMaxHP(this.hitPoint);
        //全回復
        GetData().hitPoint.Recover(this.hitPoint);

        GetData().hitPoint.SetInvincible(this.invincibleTime);
        //Inspectorの値を見て、ダメージを発生させるかどうかを指定
        if (this.hitPoint < 0)
        {
            //無敵化
            GetData().hitPoint.SetDamageShutout(true);
        }

        //移動ベクトルを正規化
        this.moveVector.Normalize();
        this.mover.SetActiveGravity(false, false);
    }

    // Update is called once per frame
    void Update () {
		
	}
}


