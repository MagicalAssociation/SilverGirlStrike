using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ボスキャラクターのクラス
//「行動パターン」
//・待ち　　　　　　　　　：パラメータに数値を入れることで指定時間の間だけ何もせずその場にとどまる
//・火炎弾ばらまき　　　　：上方向の画面外に飛び、その後画面上部を横切って火炎弾を落とす
//・フレアエッジ　　　　　：その場の真下へスーパーチャクチ、その後広いほうの画面端へダッシュ攻撃、その後通った軌跡から剣が生える
//・火をも焦がすマグマの拳：拳を出して、プレイヤーへ向かって殴りかかる、殴った場所を爆発させて範囲攻撃
//・ヒートアップブロウ　　：必殺技、体力が減った時に、画面中央へ移動した後、回転する弾を撃つ
public class BossCharacterEnn : CharacterObject {
    public enum Direction
    {
        LEFT,
        RIGHT,
    }

    public enum State{
        Neutral,

        Encounter,
        Encounter2,

        Idle,
        FireFallShot,
        FireEdge,

        MagmaKnacle,
        MagmaKnacle2,
        MagmaKnacle3,

        HeatupBlow,
        Return,

        Death,
    }

    //内部でもっとく用
    private class Param
    {
        public Vector2 moveVector;
        //待機ステートの長さ
        public int waitStateTime;
        public State waitNextState;

        public Direction direction;

        //連続パンチフラグ
        public bool comboFlag;

    }
    //Inspectorでいじる用
    [System.Serializable]
    public class InspectorParam
    {
        //攻撃時に参照するターゲットの情報
        public CharacterObject targetCharacter;

        public int hitPoint;
        public int invincibleCount;

        public CharacterMover mover;
        public Animator animator;
        public GameObject fireShooter;

        public NarrowAttacker[] attackCollisions;
    }

    public InspectorParam inspectorParam;
    Param param;



    public override void ApplyDamage()
    {
        GetData().hitPoint.DamageUpdate();
    }

    public override bool Damage(AttackData attackData)
    {
        return GetData().hitPoint.Damage(attackData.power, attackData.chain);
    }

    public override void MoveCharacter()
    {
        this.inspectorParam.mover.UpdateVelocity(this.param.moveVector.x, this.param.moveVector.y, 0.0f, true);
        this.param.moveVector = Vector3.zero;
    }

    public override void UpdateCharacter()
    {
        //プレイヤーが死んだときはターゲットをnullにする
        if (this.inspectorParam.targetCharacter != null && this.inspectorParam.targetCharacter.IsDead())
        {
            this.inspectorParam.targetCharacter = null;
        }

        UpdateState();

        //体のあたり判定の攻撃を出しっぱなし
        if (GetData().stateManager.GetNowStateNum() != (int)State.Death) {
            this.inspectorParam.attackCollisions[0].StartAttack();
        }

        if (GetData().stateManager.GetNowStateNum() != (int)State.Death)
        {
            //死亡条件はHPが０になること
            if (GetData().hitPoint.GetHP() == 0)
            {
                GetData().stateManager.ChengeState((int)State.Death);
            }
        }

        //無敵時間中は色を変える
        if (GetData().hitPoint.IsInvincible())
        {
            GetComponent<SpriteRenderer>().color = new Color(0.9f, 0.5f, 0.5f, 1.0f);
        }
        else
        {
            GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
    }

    // Use this for initialization
    void Start () {
        this.param = new Param();
        this.param.moveVector = Vector3.zero;
        this.param.direction = Direction.RIGHT;
        Func.ChangeDirection(Direction.LEFT, ref this.param.direction, this.transform);

        this.param.waitNextState = State.MagmaKnacle;
        this.param.waitStateTime = 60;

        this.param.comboFlag = false;

        //ヒットポイントに無敵時間と最大HPを設定
        GetData().hitPoint.SetMaxHP(this.inspectorParam.hitPoint);
        GetData().hitPoint.Recover(this.inspectorParam.hitPoint);
        GetData().hitPoint.SetInvincible(this.inspectorParam.invincibleCount);

        //ステートの初期化を行う
        AddState((int)State.Neutral, new NeutralState(this));
        AddState((int)State.Encounter, new EncounterState(this));
        AddState((int)State.Encounter2, new Encounter2State(this));
        AddState((int)State.Idle, new IdleState(this));
        AddState((int)State.FireFallShot, new FireFallShotState(this));
        AddState((int)State.FireEdge, new FireEdgeState(this));
        AddState((int)State.MagmaKnacle, new MagmaKnuckle1State(this));
        AddState((int)State.MagmaKnacle2, new MagmaKnuckle2State(this));
        AddState((int)State.MagmaKnacle3, new MagmaKnuckle3State(this));
        AddState((int)State.HeatupBlow, new HeatupBlowState(this));
        AddState((int)State.Return, new ReturnState(this));
        AddState((int)State.Death, new DeathState(this));
        //初期ステート
        ChangeState((int)State.Neutral);
    }
	
	// Update is called once per frame
	void Update () {
		
	}


    //このキャラクターの全てのステートに機能を提供する基底クラス
    //今のところはステート所有者キャラクターのインスタンス提供してるだけ
    abstract class BaseState : StateParameter{
        public BossCharacterEnn characterData;

        public BaseState(BossCharacterEnn characterData)
        {
            this.characterData = characterData;
        }
    }


    ////////
    //NeutralState
    ////
    //本当になんにもしないステート、外部から別のステートに変えてもらったりいろいろする
    class NeutralState : BaseState
    {
        public NeutralState(BossCharacterEnn character)
            : base(character)
        {
        }

        public override void Enter(ref StateManager manager)
        {
            base.characterData.inspectorParam.animator.Play("stand");
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
        }
    }

    ////////
    //Encounter
    ////
    //Idleステートへとつなげるアニメを再生するステート
    class EncounterState : BaseState
    {
        public EncounterState(BossCharacterEnn character)
            : base(character)
        {
        }

        public override void Enter(ref StateManager manager)
        {
            base.characterData.inspectorParam.animator.Play("standDown");
            Sound.PlaySE("swingSmall");
            Sound.PlaySE("crash1");
        }

        public override void Exit(ref StateManager manager)
        {
        }

        public override bool Transition(ref StateManager manager)
        {
            if (GetTime() > 60)
            {
                manager.SetNextState((int)State.Encounter2);
                return true;
            }
            return false;
        }

        public override void Update()
        {
        }
    }
    ////////
    //Encounter2
    ////
    //Idleステートへとつなげるアニメを再生するステート、ジャンプして定位置に進む
    class Encounter2State : BaseState
    {
        Easing easing;
        //ジャンプするまでの時間
        int jumpWait;

        GameObject effect;

        public Encounter2State(BossCharacterEnn character)
            : base(character)
        {
            this.easing = new Easing();
        }

        public override void Enter(ref StateManager manager)
        {
            //アニメ再生、エフェクトもセットで
            base.characterData.inspectorParam.animator.Play("standBurstStart");
            int effectID = Effect.Get().CreateEffect("fireBurst", base.characterData.transform.position + Vector3.forward * 0.001f, Quaternion.identity, Vector3.one);
            effect = Effect.Get().GetEffectGameObject(effectID);

            Sound.PlaySE("press1");
            Sound.PlaySE("bombMiddle");

            //ジャンプの上方向加速度
            easing.Use(Easing.Type.Linear);
            easing.Set(0.0f, 1.0f, 3.0f);
            //時間設定
            this.jumpWait = 60;
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

            if (GetTime() == this.jumpWait)
            {
                base.characterData.inspectorParam.animator.Play("JumpUp");
            }
            if (GetTime() > this.jumpWait)
            {
                base.characterData.param.moveVector += Vector2.up * (1.0f - easing.In()) * 20.0f;
            }
            //エフェクト追従
            if (GetTime() < 110)
            {
                effect.transform.position = base.characterData.transform.position + Vector3.forward * 0.001f;
            }
        }
    }

    ////////
    //Idle
    ////
    class IdleState : BaseState
    {
        public IdleState(BossCharacterEnn character)
            :base(character)
        {
        }

        public override void Enter(ref StateManager manager)
        {
            base.characterData.inspectorParam.animator.Play("idle");
            if(manager.GetPreStateNum() == (int)State.MagmaKnacle3)
            {
                base.characterData.inspectorParam.animator.Play("turnEnd");
            }
        }

        public override void Exit(ref StateManager manager)
        {
        }

        public override bool Transition(ref StateManager manager)
        {
            if (GetTime() > base.characterData.param.waitStateTime)
            {
                //待ち時間を使いきったら事前に設定されたステートへ移行
                manager.ChengeState((int)base.characterData.param.waitNextState);
                return true;
            }
            return false;
        }

        public override void Update()
        {
        }
    }

    ////////
    //FireFallShotState
    ////
    class FireFallShotState : BaseState
    {
        Easing easing;
        int attackBeginWait;
        int shotCount;
        int direction;

        public FireFallShotState(BossCharacterEnn character)
            : base(character)
        {
            this.easing = new Easing();
        }

        public override void Enter(ref StateManager manager)
        {
            base.characterData.inspectorParam.animator.Play("moveStart");
            Sound.PlaySE("charge1");

            this.easing.Use(Easing.Type.Back);
            this.easing.ResetTime();
            this.easing.Set(0.0f, 1.0f, 3.0f);


            //攻撃が始まるまでの時間を設定
            this.attackBeginWait = 60;
            this.shotCount = 0;

            //左向きなら移動方向反転
            if (base.characterData.param.direction == Direction.LEFT)
            {
                direction = -1;
            }
            else
            {
                direction = 1;
            }
        }

        public override void Exit(ref StateManager manager)
        {
        }

        public override bool Transition(ref StateManager manager)
        {
            if(this.shotCount >= 5)
            {
                base.characterData.param.waitStateTime = 0;
                base.characterData.param.waitNextState = State.Return;
                manager.SetNextState((int)State.Idle);
                return true;
            }

            return false;
        }

        public override void Update()
        {
            float moveX = this.easing.In() * 30.0f;
            if(moveX < 0)
            {
                moveX *= 3.0f;
            }
            moveX *= direction;

            base.characterData.param.moveVector += Vector2.right * moveX;
            base.characterData.param.moveVector += Vector2.up * Mathf.Sin(GetTime() * 0.03f) * 10.0f;

            Func.ChangeDirectionFromMoveX(moveX, ref base.characterData.param.direction, base.characterData.transform);


            if(this.shotCount >= 5)
            {
                return;
            }
            //攻撃するタイミングで攻撃
            if(GetTime() > this.attackBeginWait && GetTime() % 12 == 0)
            {
                int pos = Random.Range(1, 8);

                //火の玉生成、位置もここで調整
                var obj = Instantiate<GameObject>(base.characterData.inspectorParam.fireShooter, base.characterData.transform.parent);
                obj.transform.localPosition = new Vector3(960.0f / 2.0f / 50.0f - pos * 3.0f + 2, 0.0f, 0.0f) + Vector3.up * 100.0f;
                obj.SetActive(true);
                base.characterData.FindManager().AddCharacter(obj.GetComponent<CharacterObject>());
                ++this.shotCount;
            }

        }
    }

    ////////
    //FireEdgeState
    ////
    //自分より前方に無慈悲な炎の刃を放つ、ナックルから派生する
    class FireEdgeState : BaseState
    {
        public FireEdgeState(BossCharacterEnn character)
            : base(character)
        {
        }

        public override void Enter(ref StateManager manager)
        {
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
        }
    }

    ////////
    //MagmaKnuckle1State
    ////
    //ボーっとためて、プレイヤーに向かってドーン！
    //こっちはボーっとためるほう
    class MagmaKnuckle1State : BaseState
    {
        int exitWait;

        public MagmaKnuckle1State(BossCharacterEnn character)
            : base(character)
        {
        }

        public override void Enter(ref StateManager manager)
        {
            base.characterData.inspectorParam.animator.Play("knuckleIdle");
            Sound.PlaySE("eco1");

            this.exitWait = 40;
            base.characterData.param.comboFlag = false;

            if (Random.Range(0, 100) < 50)
            {
                Sound.PlaySE("power2");
                base.characterData.param.comboFlag = true;
                this.exitWait = 90;
            }
        }

        public override void Exit(ref StateManager manager)
        {
        }

        public override bool Transition(ref StateManager manager)
        {
            //時間経過で次に
            if(GetTime() > this.exitWait)
            {
                manager.SetNextState((int)State.MagmaKnacle2);
                return true;
            }

            return false;
        }

        public override void Update()
        {
            Vector2 toTargetVector = Vector2.zero;
            //プレイヤーの方向を監視、向きを合わせる
            if(base.characterData.inspectorParam.targetCharacter != null)
            {
                //自分からターゲットへのベクトル
                toTargetVector = base.characterData.inspectorParam.targetCharacter.transform.position - base.characterData.transform.position;
                toTargetVector.Normalize();

                Func.ChangeDirectionFromMoveX(toTargetVector.x, ref base.characterData.param.direction, base.characterData.transform);
            }
            //じりじりにじり寄る
            base.characterData.param.moveVector += toTargetVector * 0.1f;

        }
    }
    ////////
    //MagmaKnuckle2State
    ////
    //ボーっとためて、プレイヤーに向かってドーン！
    //こっちはドーン！と殴りかかるほう
    class MagmaKnuckle2State : BaseState
    {
        Easing easing;
        //このステートに入った時点でのターゲット位置を使用する
        Vector3 dest;
        //イージングの値を持っておいて判定に仕様
        float easingValue;

        public MagmaKnuckle2State(BossCharacterEnn character)
            : base(character)
        {
            this.easing = new Easing();
            this.easingValue = 0.0f;
        }

        public override void Enter(ref StateManager manager)
        {
            this.easing.Use(Easing.Type.Quad);
            this.easing.ResetTime();
            this.easing.Set(0.0f, 1.0f, 1.0f);

            //自分からターゲットへのベクトル
            if (base.characterData.inspectorParam.targetCharacter != null)
            {
                this.dest = base.characterData.inspectorParam.targetCharacter.transform.position;
            }
            else
            {
                //ターゲットが居ない場合はどうしようもない
                this.dest = base.characterData.transform.position;
            }
            base.characterData.inspectorParam.animator.Play("knuckleMove");
        }

        public override void Exit(ref StateManager manager)
        {
        }

        public override bool Transition(ref StateManager manager)
        {
            //イージングでの移動が終わったら
            if((this.dest - base.characterData.transform.position).magnitude < 0.5f)
            {
                manager.SetNextState((int)State.MagmaKnacle3);
                return true;
            }

            return false;
        }

        public override void Update()
        {
            this.easingValue = this.easing.In();
            //ドーンっと襲ってくる
            base.characterData.param.moveVector += (Vector2)(this.dest - base.characterData.transform.position) * this.easingValue * 10.0f;
            Func.ChangeDirectionFromMoveX(base.characterData.param.moveVector.x, ref base.characterData.param.direction, base.characterData.transform);
        }
    }
    ////////
    //MagmaKnuckle3State
    ////
    //ボーっとためて、プレイヤーに向かってドーン！
    //爆☆発☆四☆散
    class MagmaKnuckle3State : BaseState
    {
        Easing easing;
        //イージングの値を持っておいて判定に仕様
        float movePower;
        int count;

        public MagmaKnuckle3State(BossCharacterEnn character)
            : base(character)
        {
            this.easing = new Easing();
            this.count = -1;
        }

        public override void Enter(ref StateManager manager)
        {
            base.characterData.inspectorParam.animator.Play("knuckleAttack");
            Sound.PlaySE("swingHeavy");

            this.easing.Use(Easing.Type.Cubic);
            this.easing.ResetTime();
            this.easing.Set(0.0f, 1.0f, 5.0f);

            this.movePower = 1.0f;
        }

        public override void Exit(ref StateManager manager)
        {
        }

        public override bool Transition(ref StateManager manager)
        {


            //イージングでの移動が終わったら
            if (this.movePower < 0.01f)
            {
                Func.SelectRandomState(base.characterData);
                manager.SetNextState((int)State.Idle);

                //連続攻撃
                if (base.characterData.param.comboFlag && this.count < 0)
                {
                    this.count = 0;
                }
                if (this.count >= 0)
                {
                    ++this.count;
                    manager.SetNextState((int)State.MagmaKnacle2);
                }
                if(this.count >= 3)
                {
                    manager.SetNextState((int)State.Idle);
                    this.count = -1;
                    base.characterData.param.comboFlag = false;
                }


                return true;
            }
            return false;
        }

        public override void Update()
        {
            if(GetTime() == 5)
            {
                base.characterData.inspectorParam.animator.Play("turn");
                Effect.Get().CreateEffect("explosion", base.characterData.transform.position + Vector3.forward * -0.001f, Quaternion.identity, Vector3.one);
                Effect.Get().CreateEffect("explosion", base.characterData.transform.position + Vector3.forward * -0.001f + Vector3.right * 2.5f, Quaternion.identity, Vector3.one);
                Effect.Get().CreateEffect("explosion", base.characterData.transform.position + Vector3.forward * -0.001f - Vector3.right * 2.5f, Quaternion.identity, Vector3.one);
                Effect.Get().CreateEffect("explosion", base.characterData.transform.position + Vector3.forward * -0.001f + Vector3.up * 2.5f, Quaternion.identity, Vector3.one);
                Effect.Get().CreateEffect("explosion", base.characterData.transform.position + Vector3.forward * -0.001f - Vector3.up * 2.5f, Quaternion.identity, Vector3.one);
                Sound.PlaySE("press1");
                base.characterData.inspectorParam.attackCollisions[1].StartAttack();
            }
            if (GetTime() > 5)
            {
                //反動のノックバック

                this.movePower = (1.0f - easing.Out());
                Vector2 moveVec = Vector3.zero;
                moveVec += Vector2.up * movePower * 5.0f;
                //左向きなら移動方向反転
                if (base.characterData.param.direction == Direction.LEFT)
                {
                    moveVec += Vector2.right * movePower * 15.0f;
                }
                else
                {
                    moveVec += -Vector2.right * movePower * 15.0f;
                }

                base.characterData.param.moveVector += moveVec;
            }
        }
    }
    ////////
    //HeatupBlowState
    ////
    class HeatupBlowState : BaseState
    {
        public HeatupBlowState(BossCharacterEnn character)
            : base(character)
        {
        }

        public override void Enter(ref StateManager manager)
        {
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
        }
    }
    ////////
    //ReturnState
    ////
    //画面外に出るような動きの後に、適切な位置に戻ってこさせるステート
    class ReturnState : BaseState
    {

        //めんば
        Vector2 targetDirection;
        Vector3 targetPos;
        float power;


        public ReturnState(BossCharacterEnn characterData) : base(characterData)
        {
        }

        public override void Enter(ref StateManager manager)
        {
            this.power = 1.0f;
            this.targetPos = Vector3.zero;
            //目標位置の真上へワープ
            base.characterData.transform.localPosition = this.targetPos + Vector3.up * 5;


            this.targetDirection = new Vector2(
                this.targetPos.x - base.characterData.transform.localPosition.x,
                this.targetPos.y - base.characterData.transform.localPosition.y);

            //速度をかけるため正規化
            this.targetDirection.Normalize();
        }

        public override void Exit(ref StateManager manager)
        {
        }

        public override bool Transition(ref StateManager manager)
        {
            //進行方向と、目的地へ向かうベクトルを比較し、後ろにあったら待機に移行
            float dot = Vector2.Dot(this.targetDirection, this.targetPos - base.characterData.transform.localPosition);

            if (dot < 0.0f || this.targetDirection.magnitude < 0.01f)
            {
                Func.SelectRandomState(base.characterData);
                manager.SetNextState((int)State.Idle);
                return true;
            }
            return false;
        }

        public override void Update()
        {
            this.power -= 0.08f;
            if (this.power < 0.1f)
            {
                this.power = 0.1f;
            }
            //移動先への移動
            base.characterData.param.moveVector += this.targetDirection * 45.0f * this.power;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////
    //死亡(プレイヤー追従)
    class DeathState : BaseState
    {
        //メンバー
        const int burstStartTime = 120;
        const int burstTime = 180;

        public DeathState(BossCharacterEnn param) : base(param)
        {
        }

        public override void Enter(ref StateManager manager)
        {
            base.characterData.GetData().hitPoint.SetDamageShutout(true);

            Sound.PlaySE("slashFlash");
            Sound.StopBGM();
            base.characterData.inspectorParam.animator.Play("damage");
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

            base.characterData.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.0f, 0.0f, 1.0f);


            if (base.GetTime() % 7 == 0 && base.GetTime() >= burstStartTime && base.GetTime() <= burstStartTime + burstTime - 10)
            {
                Sound.PlaySE("bombSmall");
                Vector3 randomMove = new Vector3(Random.Range(0.0f, 3.0f) - 1.5f, Random.Range(0.0f, 3.0f) - 1.5f, 0.0f);
                Effect.Get().CreateEffect("manyBombs", base.characterData.transform.position - Vector3.forward + randomMove, Quaternion.identity, Vector3.one * 2);
            }

            if (base.GetTime() == burstStartTime + burstTime - 10)
            {
                Sound.PlaySE("crash1");
            }
            if (base.GetTime() > burstStartTime + burstTime)
            {
                Sound.PlaySE("bombBig");
                Effect.Get().CreateEffect("defeat", base.characterData.transform.position - Vector3.forward, Quaternion.identity, Vector3.one);
                Effect.Get().CreateEffect("flash", base.characterData.transform.position - Vector3.forward, Quaternion.identity, Vector3.one);
                base.characterData.KillMyself();
            }
        }

    }
    //便利関数系
    public class Func
    {

        //ランダムで行動を決定する
        public static void SelectRandomState(BossCharacterEnn characterData)
        {
            int id = Random.Range(0, 2);

            switch (id)
            {
                case 0:
                    characterData.param.waitStateTime = 35;
                    characterData.param.waitNextState = State.FireFallShot;
                    break;
                case 1:
                    characterData.param.waitStateTime = 30;
                    characterData.param.waitNextState = State.MagmaKnacle;
                    break;
            }
        }

        //向き変更関数、スケールを弄る
        public static void ChangeDirection(Direction direction, ref Direction current, Transform transform)
        {
            if (current != direction)
            {
                var scale = transform.transform.localScale;
                scale.x *= -1;
                transform.transform.localScale = scale;
            }

            current = direction;
        }
        //移動の値から方向を振り分ける関数
        public static void ChangeDirectionFromMoveX(float xMove, ref Direction current, Transform transform)
        {
            if (xMove > 0.0f)
            {
                ChangeDirection(Direction.RIGHT, ref current, transform);
            }
            else if (xMove < 0.0f)
            {
                ChangeDirection(Direction.LEFT, ref current, transform);
            }
        }


    }
}
