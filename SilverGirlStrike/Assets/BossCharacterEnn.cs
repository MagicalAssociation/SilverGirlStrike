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
        HeatupBlow,
    }

    //内部でもっとく用
    private class Param
    {
        public Vector3 moveVector;
        //待機ステートの長さ
        public int waitStateTime;
        public State waitNextState;

        public Direction direction;



    }
    //Inspectorでいじる用
    [System.Serializable]
    public class InspectorParam
    {
        public float gravity;
        public CharacterMover mover;
        public Animator animator;
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
        UpdateState();
    }

    // Use this for initialization
    void Start () {
        this.param = new Param();
        this.param.waitStateTime = 0;
        this.param.moveVector = Vector3.zero;
        this.param.direction = Direction.RIGHT;
        Func.ChangeDirection(Direction.LEFT, ref this.param.direction, this.transform);

        //ステートの初期化を行う

        AddState((int)State.Neutral, new NeutralState(this));
        AddState((int)State.Encounter, new EncounterState(this));
        AddState((int)State.Encounter2, new Encounter2State(this));
        AddState((int)State.Idle, new IdleState(this));
        AddState((int)State.FireFallShot, new FireFallShotState(this));
        AddState((int)State.FireEdge, new FireEdgeState(this));
        AddState((int)State.MagmaKnacle, new MagmaKnuckleState(this));
        AddState((int)State.HeatupBlow, new HeatupBlowState(this));
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
            this.characterData.inspectorParam.animator.Play("stand");
        }

        public override void Exit(ref StateManager manager)
        {
        }

        public override bool Transition(ref StateManager manager)
        {
            if (GetTime() > 60)
            {
                manager.ChengeState((int)State.Encounter);
                return true;
            }
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
            this.characterData.inspectorParam.animator.Play("standDown");
        }

        public override void Exit(ref StateManager manager)
        {
        }

        public override bool Transition(ref StateManager manager)
        {
            if (GetTime() > 30)
            {
                manager.ChengeState((int)State.Encounter2);
                return true;
            }
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
    class Encounter2State : BaseState
    {
        Easing easing;

        public Encounter2State(BossCharacterEnn character)
            : base(character)
        {
            this.easing = new Easing();
        }

        public override void Enter(ref StateManager manager)
        {
            this.characterData.inspectorParam.animator.Play("standBurstStart");
            Effect.Get().CreateEffect("fireBurst", this.characterData.transform.position + Vector3.forward * 0.001f, Quaternion.identity, Vector3.one);

            easing.Use(Easing.Type.Linear);
            easing.Set(0.0f, 1.0f, 6.0f);
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

            if (GetTime() == 60)
            {
                this.characterData.inspectorParam.animator.Play("JumpUp");
            }
            if (GetTime() > 60)
            {
                this.characterData.param.moveVector += Vector3.up * (1.0f - easing.In()) * 4.0f;
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
            this.characterData.inspectorParam.animator.Play("idle");
        }

        public override void Exit(ref StateManager manager)
        {
            throw new System.NotImplementedException();
        }

        public override bool Transition(ref StateManager manager)
        {
            if(this.characterData.param.waitStateTime > GetTime())
            {
                //待ち時間を使いきったら事前に設定されたステートへ移行
                manager.ChengeState((int)this.characterData.param.waitNextState);
                return true;
            }
            return false;
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
        }
    }

    ////////
    //FireFallShotState
    ////
    class FireFallShotState : BaseState
    {
        public FireFallShotState(BossCharacterEnn character)
            : base(character)
        {
        }

        public override void Enter(ref StateManager manager)
        {
            throw new System.NotImplementedException();
        }

        public override void Exit(ref StateManager manager)
        {
            throw new System.NotImplementedException();
        }

        public override bool Transition(ref StateManager manager)
        {
            throw new System.NotImplementedException();
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
        }
    }

    ////////
    //FireEdgeState
    ////
    class FireEdgeState : BaseState
    {
        public FireEdgeState(BossCharacterEnn character)
            : base(character)
        {
        }

        public override void Enter(ref StateManager manager)
        {
            throw new System.NotImplementedException();
        }

        public override void Exit(ref StateManager manager)
        {
            throw new System.NotImplementedException();
        }

        public override bool Transition(ref StateManager manager)
        {
            throw new System.NotImplementedException();
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
        }
    }

    ////////
    //MagmaKnuckleState
    ////
    class MagmaKnuckleState : BaseState
    {
        public MagmaKnuckleState(BossCharacterEnn character)
            : base(character)
        {
        }

        public override void Enter(ref StateManager manager)
        {
            throw new System.NotImplementedException();
        }

        public override void Exit(ref StateManager manager)
        {
            throw new System.NotImplementedException();
        }

        public override bool Transition(ref StateManager manager)
        {
            throw new System.NotImplementedException();
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }

        public override void Exit(ref StateManager manager)
        {
            throw new System.NotImplementedException();
        }

        public override bool Transition(ref StateManager manager)
        {
            throw new System.NotImplementedException();
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
        }
    }

    //便利関数系
    public class Func
    {
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
    }
}
