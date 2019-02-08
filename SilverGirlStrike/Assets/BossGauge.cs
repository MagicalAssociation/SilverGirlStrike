using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ボス用の演出類を備えたHPゲージ
public class BossGauge : MonoBehaviour {
    //HPを見る対象
    public CharacterObject target;

    //ゲージと目盛り
    public SimpleScaleGauge gauge;
    public HPGaugeScale gaugeScale;

    //状態をステートで管理
    StateManager stateManager;

    enum State {
        Start,
        Start2,
        Neutral,
        End,
    }

    BossGauge()
    {
        this.stateManager = new StateManager();
    }

    // Use this for initialization
    void Start () {
        this.stateManager.SetParameter((int)State.Start, new OpenGaugeState(this));
        this.stateManager.SetParameter((int)State.Start2, new AppearGaugeScaleState(this));
        this.stateManager.SetParameter((int)State.Neutral, new NeautralState(this));
        this.stateManager.SetParameter((int)State.End, new EndState(this));
        this.stateManager.ChengeState((int)State.Start);

	}
	
	// Update is called once per frame
	void Update () {
        this.stateManager.Update();

	}





    //以下ステートを記述

    //ゲージの幅が広がってゆく
    class OpenGaugeState : StateParameter
    {
        BossGauge bossGauge;
        Easing easing;

        public OpenGaugeState(BossGauge bossGauge)
        {
            this.bossGauge = bossGauge;
            this.easing = new Easing();
            //加速度の変化が大きい感じのイージング関数を選択
            this.easing.Use(Easing.Type.Quint);
        }

        public override void Enter(ref StateManager manager)
        {
            //1秒で展開完了する
            this.easing.Set(0.0f, this.bossGauge.target.GetData().hitPoint.GetMaxHP(), 6.0f);
            this.bossGauge.gauge.scale = this.easing.Out();
            this.bossGauge.gaugeScale.currentValue = 0;
        }

        public override void Exit(ref StateManager manager)
        {
        }

        public override bool Transition(ref StateManager manager)
        {
            //イージングが終わったら次のステート
            if (!this.easing.IsPlay())
            {
                manager.SetNextState((int)State.Start2);
                return true;
            }
            return false;
        }

        public override void Update()
        {
            if (GetTime() == 10)
            {
                Sound.PlaySE("power1");
            }
                if (GetTime() > 10)
            {
                this.bossGauge.gauge.scale = this.easing.Out();
            }
        }
    }

    //HP目盛りをどんどん増やす演出
    class AppearGaugeScaleState : StateParameter
    {
        BossGauge bossGauge;

        int currentNumScale;
        int wait;

        public AppearGaugeScaleState(BossGauge bossGauge)
        {
            this.bossGauge = bossGauge;

        }

        public override void Enter(ref StateManager manager)
        {
            this.wait = 2;
            this.bossGauge.gaugeScale.max = this.bossGauge.target.GetData().hitPoint.GetMaxHP();
        }

        public override void Exit(ref StateManager manager)
        {
        }

        public override bool Transition(ref StateManager manager)
        {
            //イージングが終わったら次のステート
            if (this.currentNumScale == this.bossGauge.gaugeScale.max)
            {
                manager.SetNextState((int)State.Neutral);
                return true;
            }
            return false;
        }

        public override void Update()
        {
            //0除算が起こる場合は（ウェイト０）スキップ
            if (wait == 0)
            {
                this.currentNumScale = this.bossGauge.gaugeScale.max;
                this.bossGauge.gaugeScale.currentValue = this.currentNumScale;
                Sound.PlaySE("machineSwitch");
                return;
            }


            if (GetTime() % this.wait == 0 )
            {
                ++this.currentNumScale;
                this.bossGauge.gaugeScale.currentValue = this.currentNumScale;
                Sound.PlaySE("machineSwitch");
            }


        }
    }


    //ゲージを対象に合わせて増減する
    class NeautralState : StateParameter
    {
        BossGauge bossGauge;

        public NeautralState(BossGauge bossGauge)
        {
            this.bossGauge = bossGauge;

        }

        public override void Enter(ref StateManager manager)
        {

        }

        public override void Exit(ref StateManager manager)
        {
        }

        public override bool Transition(ref StateManager manager)
        {

            if (this.bossGauge.target.GetData().hitPoint.GetHP() == 0)
            {
                manager.SetNextState((int)State.End);
                return true;
            }
            return false;
        }

        public override void Update()
        {
            this.bossGauge.gaugeScale.currentValue = this.bossGauge.target.GetData().hitPoint.GetHP();
        }
    }

    //画面外へさようならする
    class EndState : StateParameter
    {
        BossGauge bossGauge;
        Easing easing;

        public EndState(BossGauge bossGauge)
        {
            this.bossGauge = bossGauge;
            this.easing = new Easing();
            //加速度の変化が大きい感じのイージング関数を選択
            this.easing.Use(Easing.Type.Quint);
        }

        public override void Enter(ref StateManager manager)
        {
            this.easing.Set(0.0f, this.bossGauge.target.GetData().hitPoint.GetMaxHP(), 6.0f);
            this.bossGauge.gaugeScale.currentValue = this.bossGauge.target.GetData().hitPoint.GetHP();
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
            this.bossGauge.transform.position += Vector3.up * this.easing.In() * 0.5f;

            //イージングが終わったら死ぬ
            if (!this.easing.IsPlay())
            {
                Destroy(this.bossGauge.gameObject);
            }
        }
    }
}
