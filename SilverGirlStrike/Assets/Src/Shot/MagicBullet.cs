using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBullet : CharacterObject {
    AttackData data;
    public float movePower;
    /**
     * brief    AttackData登録
     * param[in] AttackData data 攻撃データ
     */
     public void SetAttackData(AttackData data)
    {
        this.data = data;
    }
    /**
     * brief    AttackDataの取得
     * return AttackData 攻撃データ
     */
     public AttackData GetAttackData()
    {
        return this.data;
    }

    public override void UpdateCharacter()
    {
    }

    public override void Damage(AttackData attackData)
    {
    }

    public override void ApplyDamage()
    {
    }

    public override void MoveCharacter()
    {
    }
}
