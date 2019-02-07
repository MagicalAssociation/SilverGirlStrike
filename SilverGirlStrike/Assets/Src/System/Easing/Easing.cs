using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
*@brief	Easingを扱うclass
*
*各詳細はEasing表を確認
*/
[System.Serializable]
public class Easing
{
    public abstract class EasingBase
    {
        public abstract float In(float t, float b, float c, float d);
        public abstract float Out(float t, float b, float c, float d);
        public abstract float InOut(float t, float b, float c, float d);
    }
    [System.Serializable]
    public class Parameter
    {
        public float time;
        public Easing.Type type;
        public Easing.MoveType move;
        public Parameter()
        {
            time = 1.0f;
            type = Type.Linear;
            move = MoveType.InOut;
        }
        public Parameter(float time,Type type,MoveType move)
        {
            this.time = time;
            this.type = type;
            this.move = move;
        }
        public Parameter(Parameter p)
        {
            this.time = p.time;
            this.type = p.type;
            this.move = p.move;
        }
    }
    //! TimeCount
    float cnt;
    //! EnablePlay
    bool toplay = false;
    //! StartValue
    float start;
    //! EndValue
    float end;
    //! Duration
    float duration;
    //! 使用するEasing
    EasingBase use;
    //! Easing設定用
    public Parameter parameter;
    /**
	*@brief	イージング用カウンタ
	*@param[in]	float duration 設定タイム
	*@return 現在タイム
	*/
    public float Time(float duration)
    {
        if (cnt <= duration)
        {
            cnt += 0.1f;
        }
        if (cnt >= duration)
        {
            cnt = duration;
            this.toplay = false;
        }
        return cnt;
    }
    /**
	*@brief	実行中か取得
	*@return bool true 実行中
	*/
    public bool IsPlay()
    {
        return this.toplay;
    }
    /**
    *@brief	タイム初期化
    */
    public void ResetTime()
    {
        this.cnt = 0;
        this.toplay = false;
        this.start = 0.0f;
        this.end = 0.0f;
        this.duration = 0.0f;
    }
    /**
    *@brief	constructor
    */
    public Easing() {
        this.ResetTime();
        this.back = new Back();
        this.bounce = new Bounce();
        this.cubic = new Cubic();
        this.linear = new Linear();
        this.quad = new Quad();
        this.quart = new Quart();
        this.quint = new Quint();
        //DefaultEasing
        this.use = new Linear();
        this.parameter = new Parameter();
    }
    public Easing(Easing easing)
    {
        this.ResetTime();
        this.start = easing.start;
        this.end = easing.end;
        this.duration = easing.duration;
        this.use = easing.use;
        this.parameter = new Parameter(easing.parameter);
        this.back = new Back();
        this.bounce = new Bounce();
        this.cubic = new Cubic();
        this.linear = new Linear();
        this.quad = new Quad();
        this.quart = new Quart();
        this.quint = new Quint();
        //DefaultEasing
        this.use = new Linear();
    }
    public Easing(Easing.Parameter param)
    {
        this.ResetTime();
        this.parameter = new Parameter(param);
        this.back = new Back();
        this.bounce = new Bounce();
        this.cubic = new Cubic();
        this.linear = new Linear();
        this.quad = new Quad();
        this.quart = new Quart();
        this.quint = new Quint();
        //DefaultEasing
        this.use = new Linear();
    }
    /**
    *@brief	開始と終了地点を登録
    *@param[in] float startValue StartValue
    *@param[in] float endValue EndValue
    */
    public void Set(float startValue, float endValue)
    {
        this.start = startValue;
        this.end = endValue;
        this.toplay = true;
    }
    public void Set(float startValue, float endValue, float duration)
    {
        this.Set(startValue, endValue);
        this.duration = duration;
    }
    public void Set(float startValue, float endValue, float duration, EasingBase easing)
    {
        this.Set(startValue, endValue, duration);
        this.Use(easing);
    }
    public void Set(float startValue, float endValue, float duration, Type type)
    {
        this.Set(startValue, endValue, duration);
        this.Use(type);
    }
    public void Use(EasingBase easing)
    {
        this.use = easing;
    }
    public void Use(Type type)
    {
        switch(type)
        {
            case Type.Linear:
                this.use = new Linear();
                break;
            case Type.Back:
                this.use = new Back();
                break;
            case Type.Bounce:
                this.use = new Bounce();
                break;
            case Type.Cubic:
                this.use = new Cubic();
                break;
            case Type.Quad:
                this.use = new Quad();
                break;
            case Type.Quart:
                this.use = new Quart();
                break;
            case Type.Quint:
                this.use = new Quint();
                break;
        }
    }
    public EasingBase Get()
    {
        return this.use;
    }
    public float In()
    {
        return this.use.In(this.Time(duration), start, end, duration);
    }
    public float Out()
    {
        return this.use.Out(this.Time(duration), start, end, duration);
    }
    public float InOut()
    {
        return this.use.InOut(this.Time(duration), start, end, duration);
    }
    public float Move(MoveType move)
    {
        switch(move)
        {
            case Easing.MoveType.In:
                return this.In();
            case Easing.MoveType.Out:
                return this.Out();
            case Easing.MoveType.InOut:
                return this.InOut();
        }
        return this.InOut();
    }
    public float Move()
    {
        switch (parameter.move)
        {
            case Easing.MoveType.In:
                return this.In();
            case Easing.MoveType.Out:
                return this.Out();
            case Easing.MoveType.InOut:
                return this.InOut();
        }
        return this.InOut();
    }
    /**
    *@brief	開始始点を取得
    *@return float StartValue
*/
    public float GetStartValue()
    {
        return this.start;

    }
    /**
    *@brief	終了始点を取得
    *@return float EndValue
    */
    public float GetEndValue()
    {
        return this.end;

    }
    [System.Serializable]
    public enum Type
    {
        Linear,
        Back,
        Bounce,
        Cubic,
        Quad,
        Quart,
        Quint,
    }
    [System.Serializable]
    public enum MoveType
    {
        In,
        Out,
        InOut,
    }
    //t = 時間 d = 始点 c = 終点-始点 d = 経過時間
    public class Linear : EasingBase
    {
        public float None(float t, float b, float c, float d)
        {
            return c * t / d + b;
        }
        public override float In(float t, float b, float c, float d)
        {
            return c * t / d + b;
        }
        public override float Out(float t, float b, float c, float d)
        {
            return c * t / d + b;
        }
        public override float InOut(float t, float b, float c, float d)
        {
            return c * t / d + b;
        }
    }
    public Linear linear;
    //t = 時間 d = 始点 c = 終点-始点 d = 経過時間
    public class Back : EasingBase
    {
        public override float In(float t, float b, float c, float d)
        {
            float s = 1.70158f;
            float postFix = t /= d;
            return c * (postFix) * t * ((s + 1) * t - s) + b;
        }
        public override float Out(float t, float b, float c, float d)
        {

            float s = 1.70158f;

            return c * ((t = t / d - 1) * t * ((s + 1) * t + s) + 1) + b;
        }
        public override float InOut(float t, float b, float c, float d)
        {
            float s = 1.70158f;
            if ((t /= d / 2) < 1) return c / 2 * (t * t * (((s *= (1.525f)) + 1) * t - s)) + b;
            float postFix = t -= 2;
            return c / 2 * ((postFix) * t * (((s *= (1.525f)) + 1) * t + s) + 2) + b;
        }
    }
    public Back back;
    //t = 時間 d = 始点 c = 終点-始点 d = 経過時間
    public class Bounce : EasingBase
    {
        public override float Out(float t, float b, float c, float d)
        {

            if ((t /= d) < (1 / 2.75f))
            {
                return c * (7.5625f * t * t) + b;
            }
            else if (t < (2 / 2.75f))
            {
                float postFix = t -= (1.5f / 2.75f);
                return c * (7.5625f * (postFix) * t + .75f) + b;
            }
            else if (t < (2.5 / 2.75))
            {
                float postFix = t -= (2.25f / 2.75f);
                return c * (7.5625f * (postFix) * t + .9375f) + b;
            }
            else
            {
                float postFix = t -= (2.625f / 2.75f);
                return c * (7.5625f * (postFix) * t + .984375f) + b;
            }
        }
        public override float In(float t, float b, float c, float d)
        {
            return c - Out(d - t, 0, c, d) + b;
        }
        public override float InOut(float t, float b, float c, float d)
        {
            if (t < d / 2) return In(t * 2, 0, c, d) * .5f + b;
            else return Out(t * 2 - d, 0, c, d) * .5f + c * .5f + b;
        }
    }
    public Bounce bounce;
    //t = 時間 d = 始点 c = 終点-始点 d = 経過時間
    public class Cubic : EasingBase
    {
        public override float In(float t, float b, float c, float d)
        {
            return c * (t /= d) * t * t + b;
        }
        public override float Out(float t, float b, float c, float d)
        {
            return c * ((t = t / d - 1) * t * t + 1) + b;
        }
        public override float InOut(float t, float b, float c, float d)
        {
            if ((t /= d / 2) < 1) return c / 2 * t * t * t + b;
            return c / 2 * ((t -= 2) * t * t + 2) + b;
        }
    }
    public Cubic cubic;
    //t = 時間 d = 始点 c = 終点-始点 d = 経過時間	
    public class Quad : EasingBase
    {
        public override float In(float t, float b, float c, float d)
        {
            return c * (t /= d) * t + b;
        }
        public override float Out(float t, float b, float c, float d)
        {
            return -c * (t /= d) * (t - 2) + b;
        }
        public override float InOut(float t, float b, float c, float d)
        {
            if ((t /= d / 2) < 1) return ((c / 2) * (t * t)) + b;
            return -c / 2 * (((t - 2) * (--t)) - 1) + b;
        }
    }
    public Quad quad;
    //t = 時間 d = 始点 c = 終点-始点 d = 経過時間
    public class Quart : EasingBase
    {
        public override float In(float t, float b, float c, float d)
        {
            return c * (t /= d) * t * t * t + b;
        }
        public override float Out(float t, float b, float c, float d)
        {
            return -c * ((t = t / d - 1) * t * t * t - 1) + b;
        }
        public override float InOut(float t, float b, float c, float d)
        {
            if ((t /= d / 2) < 1) return c / 2 * t * t * t * t + b;
            return -c / 2 * ((t -= 2) * t * t * t - 2) + b;
        }
    }
    public Quart quart;
    //t = 時間 d = 始点 c = 終点-始点 d = 経過時間
    public class Quint : EasingBase
    {
        public override float In(float t, float b, float c, float d)
        {
            return c * (t /= d) * t * t * t * t + b;
        }
        public override float Out(float t, float b, float c, float d)
        {
            return c * ((t = t / d - 1) * t * t * t * t + 1) + b;
        }
        public override float InOut(float t, float b, float c, float d)
        {
            if ((t /= d / 2) < 1) return c / 2 * t * t * t * t * t + b;
            return c / 2 * ((t -= 2) * t * t * t * t + 2) + b;
        }
    }
    public Quint quint;
};