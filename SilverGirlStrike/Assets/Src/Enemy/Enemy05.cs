using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy05 : CharacterObject {
    /**
 * enum State
 */
    public enum State
    {
        //! 待機
        WAIT,
        //! 移動
        MOVE,
        //! 攻撃
        ATTACK,
        //! 攻撃後
        AFTERATTACK,
        //! 死亡
        DEATH,
    }
    /**
     * enum Direction 向き
     */
    public enum Direction
    {
        LEFT = 1,
        RIGHT = -1
    }
    /**
     * brief    エネミー05用パラメータデータ
     */
    [System.Serializable]
    public class Enemy05Parameter
    {
        //! 最大HP
        public int maxHP;
        //! アニメーション用class
        public Animator animation;
        //! ダメージ量
        public int power;
        //! 向き情報
        public Direction direction;
        //! 移動用class
        public CharacterMover charactermover;

        public NarrowAttacker[] narrowAttacker;

        //playerの情報を持ってくる
        public GameObject player;

        //無敵時間
        public int damageInvincible;
    }
    /**
     * brief    移動用変数をまとめたclass
     */
    [System.Serializable]
    public class Moves
    {
        //! 移動先ターゲット
        public GameObject targets;
        //! 待ち時間
        public int waitTime;
        //! ターゲットまでの移動時間
        public float moveTime;
    }

    [System.Serializable]
    public class Attack
    {
        //!弾を発射できるかどうか
        public bool canAttack;
        //!プレイヤと座標があっているかどうか
        public bool isHit;
        //!当たり判定に使用するBox2D
        public Collider2D range;
        //!プレイヤとの当たり判定情報の取得用
        public GameObject hitRange;
        //!攻撃用の弾の情報格納
        public GameObject bullet;
    }
    public Attack attackinf;

    //! 固有パラメータデータ
    public Enemy05Parameter parameter;
    //! 移動用データの配列
    public Moves[] moves;
    //! 現在の移動データ番号
    private int nowNum;
    //! 位置情報 !移動はこの値を書き変え、そのままGameObjectに渡します! ※Easingを使って移動しようとしたらこの形が計算少なく済んだ
    private Vector2 pos;
    //! 前回の位置データ
    private Vector2 prePos;
    //! 攻撃データ
    private AttackData attackData;
    //! 自身のBoxの当たり判定
    private BoxCollider2D collider;
    public Enemy05()
        //! life
        : base(10)
    {
        this.pos = new Vector2();
        this.nowNum = 0;
        this.attackData = new AttackData(this);
    }
    private void Start()
    {
        //今の位置をいれておく
        this.pos = this.transform.localPosition;
        this.prePos = this.pos;
        this.collider = GetComponent<BoxCollider2D>();
        this.attackData.power = this.parameter.power;
        this.parameter.animation = GetComponent<Animator>();
        this.parameter.charactermover = GetComponent<CharacterMover>();
        this.GetData().hitPoint.SetMaxHP(this.parameter.maxHP);
        this.GetData().hitPoint.Recover(this.parameter.maxHP);
        this.GetData().hitPoint.SetInvincible(this.parameter.damageInvincible);
        //各ステートを登録&適用
        base.AddState((int)State.MOVE, new MoveState(this));
        base.AddState((int)State.WAIT, new WaitState(this));
        base.AddState((int)State.DEATH, new DeathState(this));

        base.AddState((int)State.ATTACK, new AttackState(this));
        base.AddState((int)State.AFTERATTACK, new AfterAttackState(this));
        attackinf.canAttack = true;
        attackinf.isHit = false;

        base.ChangeState((int)State.WAIT);
    }

    public override void UpdateCharacter()
    {
        //当たり判定
        if (this.GetData().stateManager.GetNowStateNum() != (int)State.DEATH)
        {
            //this.parameter.narrowAttacker[0].StartAttack();
        }
        this.UpdateState();
    }

    //テスト追加
    public GameObject AttackBullet(GameObject bullet)
    {
        GameObject bulletinf;
        bulletinf = Instantiate(bullet);
        return bulletinf;
    }


    public override bool Damage(AttackData attackData)
    {
        return this.GetData().hitPoint.Damage(attackData.power, attackData.chain);
    }

    public override void ApplyDamage()
    {
        if (this.GetData().hitPoint.GetHP() <= 0 && base.GetData().stateManager.GetNowStateNum() != (int)State.DEATH)
        {
            base.ChangeState((int)State.DEATH);
        }
        else
        {
            this.GetData().hitPoint.DamageUpdate();
        }
    }

    public override void MoveCharacter()
    {
        this.prePos = this.transform.localPosition;
        if (this.GetData().stateManager.GetNowStateNum() != (int)State.DEATH)
        {
            this.transform.localPosition = new Vector3(pos.x, pos.y, this.transform.position.z);
        }
        else
        {
            //ここでバグってた
            //this.parameter.charactermover.UpdateVelocity(pos.x, pos.y, 3.0f / 5.0f, false);
        }

    }
    /**
     * brief    固有データを取得する
     * return Enemy02Parameter ThisParameter
     */
    public Enemy05Parameter GetParameter()
    {
        return this.parameter;
    }
    /**
     * brief    現在の配列番号
     * return int 配列番号
     */
    public int GetNowNum()
    {
        return this.nowNum;
    }
    /**
     * brief    配列番号を指定
     * param[in] int num 指定値
     */
    public void SetNowNum(int num)
    {
        this.nowNum = num;
    }
    /**
     * brief    位置を指定
     * param[in] Vector2 move 移動後位置
     */
    public void SetPos(Vector2 move)
    {
        this.pos = move;
    }
    /**
     * brief    当たり判定
     * return Collider2D 当たったオブジェクト
     */
    private Collider2D Hit()
    {
        return Physics2D.OverlapBox(
                this.collider.transform.position,
                this.collider.size,
                this.transform.eulerAngles.z,
                (int)M_System.LayerName.PLAYER
                );
    }
    //// Use this for initialization
    //void Start () {

    //}

    //// Update is called once per frame
    //void Update () {

    //}
}



/**
 * brief    元となるState
 */
public abstract class BaseState : StateParameter
{
    public Enemy05 enemy;
    public BaseState(Enemy05 enemy)
    {
        this.enemy = enemy;
    }
}
/**
 * brief    移動State
 */
public class MoveState : BaseState
{
    //! x座標Easing
    Easing move_x;
    //! y座標Easing
    Easing move_y;
    //! 移動量とかターゲット位置とかの取得用
    Enemy05.Moves moveData;

    public MoveState(Enemy05 enemy)
        : base(enemy)
    {
        this.move_x = new Easing();
        this.move_y = new Easing();
    }

    public override void Enter(ref StateManager manager)
    {

        //次自分が向かうMovesのデータを取得
        this.moveData = base.enemy.moves[base.enemy.GetNowNum()];
        //Easingを登録
        move_x.Set(this.enemy.transform.position.x, this.moveData.targets.transform.position.x - this.enemy.transform.position.x);
        move_y.Set(this.enemy.transform.position.y, this.moveData.targets.transform.position.y - this.enemy.transform.position.y);
        this.enemy.parameter.animation.Play("Enemy05Anim");

    }

    public override void Exit(ref StateManager manager)
    {
        //各値リセット
        this.move_x.ResetTime();
        this.move_y.ResetTime();
        //次の移動配列へ、配列の最後だったら0に戻す
        base.enemy.SetNowNum(base.enemy.GetNowNum() + 1);
        if (base.enemy.moves.Length <= base.enemy.GetNowNum())
        {
            base.enemy.SetNowNum(0);
        }
    }

    public override bool Transition(ref StateManager manager)
    {
        //!プレイヤーが攻撃範囲に入っているかどうかの判定に使用する
        ChildeColliderTrigger05 range = base.enemy.attackinf.hitRange.GetComponent<ChildeColliderTrigger05>();

        //移動が終わったらWAITへ移行
        if (!this.move_x.IsPlay() && !this.move_y.IsPlay())
        {
            manager.SetNextState((int)Enemy05.State.WAIT);
            return true;
        }

        //攻撃範囲内に入ったかどうかのデータを持ってくる
        enemy.attackinf.isHit = range.GetPlayerHit();

        if (base.enemy.attackinf.isHit)
        {
            manager.SetNextState((int)Enemy05.State.ATTACK);
            return true;
        }

            //プレイヤーとY軸の座標が重なったら
            //manager.SetNextState((int)Enemy05.State.Attack);

            return false;
    }

    public override void Update()
    {
        //移動値を登録
        base.enemy.SetPos(new Vector2(this.move_x.linear.None(this.move_x.Time(this.moveData.moveTime), this.move_x.GetStartValue(), this.move_x.GetEndValue(), this.moveData.moveTime),
            this.move_y.linear.None(this.move_y.Time(this.moveData.moveTime), this.move_y.GetStartValue(), this.move_y.GetEndValue(), this.moveData.moveTime)));
    }
}
/**
 * brief    待機State
 */
public class WaitState : BaseState
{
    public WaitState(Enemy05 enemy)
        : base(enemy)
    {
    }

    public override void Enter(ref StateManager manager)
    {
        //アニメーション現在未実装
        this.enemy.parameter.animation.Play("Enemy05Anim");
    }

    public override void Exit(ref StateManager manager)
    {
    }

    public override bool Transition(ref StateManager manager)
    {
        if (base.enemy.moves.Length == 0)
        {
            return false;
        }
        if (base.enemy.moves[base.enemy.GetNowNum()].waitTime <= base.GetTime())
        {
            manager.SetNextState((int)Enemy05.State.MOVE);
            return true;
        }
        return false;
    }

    public override void Update()
    {
    }
}

public class DeathState : BaseState
{
    public DeathState(Enemy05 enemy) : base(enemy)
    {
    }

    public override void Enter(ref StateManager manager)
    {
        this.enemy.parameter.animation.Play("Enemy05Break");
        //this.enemy.GetComponentInChildren<MagicTeam>().NotActive();
        //this.enemy.parameter.charactermover.Jump(8.0f);
        //if (this.enemy.transform.localScale.x == 1)
        //{
        //    this.enemy.SetPos(new Vector2(3, 0));
        //}
        //else if (this.enemy.transform.localScale.x == -1)
        //{
        //    this.enemy.SetPos(new Vector2(-3, 0));
        //}
        //if (this.enemy.transform.localScale.x == 1)
        //{
        //    this.enemy.transform.Rotate(new Vector3(0, 0, -30));
        //}
        //else if (this.enemy.transform.localScale.x == -1)
        //{
        //    this.enemy.transform.Rotate(new Vector3(0, 0, 30));
        //}
        //Sound.PlaySE("slashFlash");
        //Effect.Get().CreateEffect("defeat", this.enemy.transform.position - Vector3.forward, Quaternion.identity, Vector3.one);
        //this.enemy.GetData().hitPoint.SetDamageShutout(true);
    }

    public override void Exit(ref StateManager manager)
    {
    }

    public override bool Transition(ref StateManager manager)
    {
        return false;
    }

    public override void Update()
    {

        if (base.GetTime() >= 30)
        {
            base.enemy.KillMyself();
        }
    }
}

//移動処理は引き継ぎつつ、攻撃を行う
public class AttackState : BaseState
{
    //! x座標Easing
    Easing move_x;
    //! y座標Easing
    Easing move_y;
    //! 移動量とかターゲット位置とかの取得用
    Enemy05.Moves moveData;

    public AttackState(Enemy05 enemy)
        : base(enemy)
    {
        this.move_x = new Easing();
        this.move_y = new Easing();
    }

    public override void Enter(ref StateManager manager)
    {

        //次自分が向かうMovesのデータを取得
        this.moveData = base.enemy.moves[base.enemy.GetNowNum()];
        //Easingを登録
        move_x.Set(this.enemy.transform.position.x, this.moveData.targets.transform.position.x - this.enemy.transform.position.x);
        move_y.Set(this.enemy.transform.position.y, this.moveData.targets.transform.position.y - this.enemy.transform.position.y);
        //this.enemy.parameter.animation.Play("Normal");

    }

    public override void Exit(ref StateManager manager)
    {
        //各値リセット
        this.move_x.ResetTime();
        this.move_y.ResetTime();
        //次の移動配列へ、配列の最後だったら0に戻す
        base.enemy.SetNowNum(base.enemy.GetNowNum() + 1);
        if (base.enemy.moves.Length <= base.enemy.GetNowNum())
        {
            base.enemy.SetNowNum(0);
        }
    }

    public override bool Transition(ref StateManager manager)
    {
        //AFTERATTACKにstateを遷移させたい
        if (!base.enemy.attackinf.canAttack)
        {
            manager.SetNextState((int)Enemy05.State.AFTERATTACK);
            return true;
        }
        return false;
    }

    public override void Update()
    {
        //テスト追加
        //!攻撃時に生成する球の情報
        BulletCreate05 bulletData = base.enemy.attackinf.bullet.GetComponent<BulletCreate05>();

        //一度きりの攻撃を使用する
        if (base.enemy.attackinf.canAttack)
        {
            Vector3 bulletpos;
            bulletpos.x = enemy.transform.position.x - 0.5f;
            bulletpos.y = enemy.transform.position.y - 0.5f;
            bulletpos.z = enemy.transform.position.z;

            //テスト追加
            GameObject bulletobj;
            bulletobj = base.enemy.AttackBullet(base.enemy.attackinf.bullet);
            bulletData.Create(bulletpos,bulletobj);

            base.enemy.attackinf.canAttack = false;
        }

        //移動値を登録
        base.enemy.SetPos(new Vector2(this.move_x.linear.None(this.move_x.Time(this.moveData.moveTime), this.move_x.GetStartValue(), this.move_x.GetEndValue(), this.moveData.moveTime),
            this.move_y.linear.None(this.move_y.Time(this.moveData.moveTime), this.move_y.GetStartValue(), this.move_y.GetEndValue(), this.moveData.moveTime)));
    }
}

public class AfterAttackState : BaseState
{
    //! x座標Easing
    Easing move_x;
    //! y座標Easing
    Easing move_y;
    //! 移動量とかターゲット位置とかの取得用
    Enemy05.Moves moveData;
    public AfterAttackState(Enemy05 enemy)
        : base(enemy)
    {
        this.move_x = new Easing();
        this.move_y = new Easing();
    }

    public override void Enter(ref StateManager manager)            //移動、アニメーション、SE、エフェクトなど
    {

        //次自分が向かうMovesのデータを取得
        this.moveData = base.enemy.moves[base.enemy.GetNowNum()];
        //Easingを登録
        move_x.Set(this.enemy.transform.position.x, this.moveData.targets.transform.position.x - this.enemy.transform.position.x);
        move_y.Set(this.enemy.transform.position.y, this.moveData.targets.transform.position.y - this.enemy.transform.position.y);
        this.enemy.parameter.animation.Play("Enemy05AfterAttackAnim");

    }

    public override void Exit(ref StateManager manager)             //値をリセット
    {
        //各値リセット
        this.move_x.ResetTime();
        this.move_y.ResetTime();
        //次の移動配列へ、配列の最後だったら0に戻す
        base.enemy.SetNowNum(base.enemy.GetNowNum() + 1);
        if (base.enemy.moves.Length <= base.enemy.GetNowNum())
        {
            base.enemy.SetNowNum(0);
        }
    }

    public override bool Transition(ref StateManager manager)       //Stateの遷移
    {
        ////DEATHにstateを移行させたい
        if (!this.move_x.IsPlay() && !this.move_y.IsPlay())
        {
            manager.SetNextState((int)Enemy05.State.DEATH);
            return true;
        }
        return false;
    }

    public override void Update()                                   //毎フレームごとに更新が必要な処理
    {
        //移動値を登録
        base.enemy.SetPos(new Vector2(this.move_x.linear.None(this.move_x.Time(this.moveData.moveTime), this.move_x.GetStartValue(), this.move_x.GetEndValue(), this.moveData.moveTime),
            this.move_y.linear.None(this.move_y.Time(this.moveData.moveTime), this.move_y.GetStartValue(), this.move_y.GetEndValue(), this.moveData.moveTime)));
    }
}