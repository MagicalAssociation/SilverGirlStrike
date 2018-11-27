using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//編集履歴
//2018/11/21 板倉　：　CharacterObjectにChangeState()を行う手段がなかったのでメソッドを追加
//2018/11/24 金子　：　HitPointにダメージ処理や値修正処理を追記


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
     * constructor
     */
     public HitPoint()
    {
        this.maxHP = 10;
        this.currentHP = 10;
        this.damagePoint = 0;
        this.invincible = 0;
        this.chain = 0;
    }
    /**
     * brief    constructor
     * param[in] int maxhp 最大HP
     */
     public HitPoint(int maxhp)
    {
        this.maxHP = maxhp;
        this.currentHP = maxhp;
        this.damagePoint = 0;
        this.invincible = 0;
        this.chain = 0;
    }
    /**
     * brief    ダメージ処理
     */
     public void DamageUpdate()
    {
        //HPにダメージを与える
        this.currentHP += this.damagePoint;
        //ダメージポイントを初期化
        this.ResetDamagePoint();
        //HPが最大値を超えた場合最大値に変更する
        if(this.currentHP > this.maxHP)
        {
            this.currentHP = this.maxHP;
        }
        //HPが最低値以下になった時最低値にする
        if(this.currentHP < 0)
        {
            this.currentHP = 0;
        }
    }
    /**
     * 蓄積ダメージを初期化する
     */ 
    public void ResetDamagePoint()
    {
        this.damagePoint = 0;
    }
    /**
     * brief    ダメージポイントを蓄積する
     * param[in] int damage ダメージ量
     * マイナスを与えれば回復になる
     */ 
     public void Damage(int damage)
    {
        this.damagePoint -= damage;
    }
    /**
     * brief    現在HPを取得する
     * return int nowHitPoint
     */
     public int GetHP()
    {
        return this.currentHP;
    }
    /**
     * brief    最大HPを取得する
     * return int MaxHitPoint
     */
     public int GetMaxHP()
    {
        return this.maxHP;
    }
    /**
     * brief    DebugLog
     */
     public void DebugLog()
    {
        Debug.Log(this.maxHP + ":" + this.currentHP + ":" + this.damagePoint + ":" + this.invincible + ":" + this.chain);
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
public abstract class CharacterObject : MonoBehaviour
{
    /**
     * brief    CharacterParameterData
     */ 
    public class CharaData
    {
        //! HP関連
        public HitPoint hitPoint;
        //! 状態管理
        public StateManager manager;
        /**
         * brief    constructor
         */ 
        public CharaData()
        {
            hitPoint = new HitPoint();
            manager = new StateManager();
        }
        /**
        * brief    constructor
        * param[in] int maxHP HP最大値
        */
        public CharaData(int maxHP)
        {
            hitPoint = new HitPoint(maxHP);
            manager = new StateManager();
        }

    }
    //! CharacterData
    CharaData data;
    /**
     * brief    constructor
     */ 
    public CharacterObject()
    {
        data = new CharaData();
    }
    /**
     * brief    constructor
     * param[in] int maxHP 最大HP
     */
    public CharacterObject(int maxHP)
    {
        data = new CharaData(maxHP);
    }
    /**
     * brief    更新処理
     */
    public abstract void UpdateCharacter();
    /**
     * brief    ダメージ処理
     */ 
    public abstract void Damage(AttackData attackData);
    /**
     * brief    ダメージを適用する
     */ 
    public abstract void ApplyDamage();
    /**
     * brief    キャラクターの移動処理
     */ 
    public abstract void MoveCharacter();
    /**
     * brief    データの取得
     */ 
    public CharaData GetData()
    {
        return this.data;
    }
    /**
     * brief    Stateの更新処理
     */ 

    public void UpdateState()
    {
        this.data.manager.Update();
    }
    /**
     * brief    Stateを登録する
     */ 
    public void AddState(int stateNum, StateParameter parameter)
    {
        this.data.manager.SetParameter(stateNum, parameter);
    }
    /**
     * brief    Stateを変更する
     */ 
    public void ChangeState(int stateNum)
    {
        this.data.manager.ChengeState(stateNum);
    }

}
