using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * brief    HP関連を管理するclass
 */ 
public class HitPoint
{
    //! 最大HP
    int maxHP;
    //! 現在HP
    int currentHP;
    //! 蓄積ダメージ
    int damagePoint;
    //! 無敵時間
    int invincible;
    //! 連鎖
    int chain;
    /**
     * brief    ダメージ処理
     */
     public void DamageUpdate()
    {
        this.currentHP -= this.damagePoint;
        this.ResetDamagePoint();
    }
    /**
     * 蓄積ダメージを初期化する
     */ 
    public void ResetDamagePoint()
    {
        this.damagePoint = 0;
    }
}

/**
 * brief    こうげきデータ
 */
 public class AttackData
{
    public enum Attaribute
    {
        NON,
    }
    public enum ReactionType
    {
        NON,
    }
    int power;
    Vector2 direction;
    CharacterObject characterObject;
    Attaribute attaribute;
    ReactionType reaction;
    public AttackData(ref CharacterObject character)
    {
        this.attaribute = Attaribute.NON;
        this.reaction = ReactionType.NON;
        this.characterObject = character;
        direction = new Vector2();
    }
    public void SetAttackPower(int power)
    {
        this.power = power;
    }
    public int GetAttackPower()
    {
        return this.power;
    }
    public void SetAttribute(Attaribute attaribute)
    {
        this.attaribute = attaribute;
    }
    public Attaribute GetAttaribute()
    {
        return this.attaribute;
    }
    public void SetReaction(ReactionType reaction)
    {
        this.reaction = reaction;
    }
    public ReactionType GetReaction()
    {
        return this.reaction;
    }
    public void SetDirection(Vector2 vector)
    {
        this.direction = vector;
    }
    public Vector2 GetDirection()
    {
        return this.direction;
    }
}
/**
 * brief    キャラの基底クラス
 */ 
public class CharacterObject : MonoBehaviour {
    HitPoint hitPoint;
    StateManager manager;
    public CharacterObject()
    {
        hitPoint = new HitPoint();
        manager = new StateManager();
    }
    public HitPoint GetHitPoint()
    {
        return this.hitPoint;
    }
    public virtual void UpdateCharacter()
    {

    }
    public virtual void Damage()
    {

    }
    public virtual void ApplyDamage()
    {

    }
    public virtual void MoveCharacter()
    {

    }
    public StateManager GetStateManager()
    {
        return this.manager;
    }
    public void AddState(int stateNum,StateParameter parameter)
    {
        this.manager.SetParameter(stateNum, parameter);
    }

}
