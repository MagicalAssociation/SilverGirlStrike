﻿using System.Collections;
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
    /**
     * enum Attaribute
     * brief    攻撃属性
     */ 
    public enum Attaribute
    {
        NON,
    }
    /** 
     * enum ReactionType
     * brief    攻撃時のリアクション
     */ 
    public enum ReactionType
    {
        NON,
    }
    //! 攻撃力
    int power;
    //! 攻撃の向き
    Vector2 direction;
    //! 攻撃データを持つ主のデータ
    CharacterObject characterObject;
    //! 攻撃属性
    Attaribute attaribute;
    //! 攻撃リアクション
    ReactionType reaction;
    /**
     * brief    constructor
     * param[in] ref CharacterObject character 攻撃データの主のデータ
     */ 
    public AttackData(ref CharacterObject character)
    {
        this.attaribute = Attaribute.NON;
        this.reaction = ReactionType.NON;
        this.characterObject = character;
        direction = new Vector2();
    }
    /**
     * brief    攻撃力を設定する
     * param[in] int power 攻撃力値
     */ 
    public void SetAttackPower(int power)
    {
        this.power = power;
    }
    /**
     * brief    攻撃力を取得する
     * return int 攻撃力値
     */ 
    public int GetAttackPower()
    {
        return this.power;
    }
    /**
     * brief    属性を登録する
     * param[in] Attaribute attaribute 攻撃属性
     */ 
    public void SetAttribute(Attaribute attaribute)
    {
        this.attaribute = attaribute;
    }
    /**
     * brief    属性を取得する
     * return Attaribute 攻撃属性
     */ 
    public Attaribute GetAttaribute()
    {
        return this.attaribute;
    }
    /**
     * brief    リアクションを登録する
     * param[in] ReactionType reaction リアクション
     */ 
    public void SetReaction(ReactionType reaction)
    {
        this.reaction = reaction;
    }
    /**
     * brief    リアクションを取得する
     * return ReactionType リアクション
     */
    public ReactionType GetReaction()
    {
        return this.reaction;
    }
    /**
     * brief    攻撃の向きを設定する
     * param[in] Vector2 vector 攻撃の向き
     */ 
    public void SetDirection(Vector2 vector)
    {
        this.direction = vector;
    }
    /**
     * brief    攻撃の向きを取得する
     * return Vector2 攻撃の向き
     */ 
    public Vector2 GetDirection()
    {
        return this.direction;
    }
}


/**
 * brief    キャラの基底クラス
 */ 
public abstract class CharacterObject : MonoBehaviour {
    /**
     * brief    パラメータをまとめておくclass
     */ 
    public class CharaData
    {
        //! HPデータ
        public HitPoint hitPoint;
        //! Stateデータ
        public StateManager manager;
        /**
         * brief    constructor
         */ 
        public CharaData()
        {
            hitPoint = new HitPoint();
            manager = new StateManager();
        }
       
    }
    //! パラメータデータ
    CharaData data;
    /**
     * brief    constructor
     */ 
    public CharacterObject()
    {
        data = new CharaData();
    }
    /**
     * brief    更新処理
     */

    public abstract void UpdateCharacter();
    /**
     * brief    ダメージが発生する処理
     */
    public abstract void Damage(AttackData data);
    /**
     * brief    ダメージを適用する処理
     */
    public abstract void ApplyDamage();
    /**
     * brief    移動系処理
     */
    public abstract void MoveCharacter();
    /**
     * brief    パラメータデータを取得する
     * return CharaData データ
     */ 
    public CharaData GetData()
    {
        return this.data;
    }
    /**
     * brief    Stateを登録する
     * param[in] int stateNum ステートナンバー
     * param[in] StateParameter parameter ステートデータ
     */ 
    public void AddState(int stateNum,StateParameter parameter)
    {
        this.data.manager.SetParameter(stateNum, parameter);
    }
    /**
     * brief    Stateの更新処理
     */
     public void UpdateState()
    {
        this.data.manager.Update();
    }
}