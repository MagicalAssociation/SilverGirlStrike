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
        Debug.Log("MapHP->" + this.maxHP + ":NowHP->" + this.currentHP + ":DP->" + this.damagePoint + ":Inv->" + this.invincible + ":Chain->" + this.chain);
    }
    /**
     * brief    最大HPの設定
     * param[in] int maxHP MaxHP
     */ 
     public void SetMaxHP(int maxHP)
    {
        this.maxHP = maxHP;
    }
}

/**
 * brief    こうげきデータ
 */
 [System.Serializable]
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
    public int power;
    //! 攻撃の向き
    public Vector2 direction;
    //! 攻撃データを持つ主のデータ
    public CharacterObject characterObject;
    //! 攻撃属性
    public Attaribute attaribute;
    //! 攻撃リアクション
    public ReactionType reaction;
    /**
     * brief    constructor
     * param[in] ref CharacterObject character 攻撃データの主のデータ
     */ 
    public AttackData(CharacterObject character)
    {
        this.attaribute = Attaribute.NON;
        this.reaction = ReactionType.NON;
        this.characterObject = character;
        direction = new Vector2();
    }
}


/**
 * brief    キャラの基底クラス
 */ 
 [System.Serializable]
public abstract class CharacterObject : MonoBehaviour
{
    /**
     * brief    CharacterParameterData
     */ 
     [System.Serializable]
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
