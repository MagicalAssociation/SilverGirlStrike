﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
*@brief	Easingを扱うclass
*
*各詳細はEasing表を確認
*/
public class Easing
{
    public abstract class _Easing
    {
        public abstract float In(float t, float b, float c, float d);
        public abstract float Out(float t, float b, float c, float d);
        public abstract float InOut(float t, float b, float c, float d);
    }
    //! TimeCount
    float cnt;
    //! EnablePlay
    bool toplay = false;
    //! StartValue
    float start;
    //! EndValue
    float end;
    //! 使用するEasing
    _Easing use;
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
        this.toplay = true;
        this.start = 0;
        this.end = 0;
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
    }
    public void Use(_Easing easing)
    {
        this.use = easing;
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
    //t = 時間 d = 始点 c = 終点-始点 d = 経過時間
    public class Linear : _Easing
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
    public class Back
    {
        public float In(float t, float b, float c, float d)
        {
            float s = 1.70158f;
            float postFix = t /= d;
            return c * (postFix) * t * ((s + 1) * t - s) + b;
        }
        public float Out(float t, float b, float c, float d)
        {

            float s = 1.70158f;

            return c * ((t = t / d - 1) * t * ((s + 1) * t + s) + 1) + b;
        }
        public float InOut(float t, float b, float c, float d)
        {
            float s = 1.70158f;
            if ((t /= d / 2) < 1) return c / 2 * (t * t * (((s *= (1.525f)) + 1) * t - s)) + b;
            float postFix = t -= 2;
            return c / 2 * ((postFix) * t * (((s *= (1.525f)) + 1) * t + s) + 2) + b;
        }
    }
    public Back back;
    //t = 時間 d = 始点 c = 終点-始点 d = 経過時間
    public class Bounce
    {
        public float Out(float t, float b, float c, float d)
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
        public float In(float t, float b, float c, float d)
        {
            return c - Out(d - t, 0, c, d) + b;
        }
        public float InOut(float t, float b, float c, float d)
        {
            if (t < d / 2) return In(t * 2, 0, c, d) * .5f + b;
            else return Out(t * 2 - d, 0, c, d) * .5f + c * .5f + b;
        }
    }
    public Bounce bounce;
    //t = 時間 d = 始点 c = 終点-始点 d = 経過時間
    public class Cubic
    {
        public float In(float t, float b, float c, float d)
        {
            return c * (t /= d) * t * t + b;
        }
        public float Out(float t, float b, float c, float d)
        {
            return c * ((t = t / d - 1) * t * t + 1) + b;
        }
        public float InOut(float t, float b, float c, float d)
        {
            if ((t /= d / 2) < 1) return c / 2 * t * t * t + b;
            return c / 2 * ((t -= 2) * t * t + 2) + b;
        }
    }
    public Cubic cubic;
    //t = 時間 d = 始点 c = 終点-始点 d = 経過時間	
    public class Quad
    {
        public float In(float t, float b, float c, float d)
        {
            return c * (t /= d) * t + b;
        }
        public float Out(float t, float b, float c, float d)
        {
            return -c * (t /= d) * (t - 2) + b;
        }
        public float InOut(float t, float b, float c, float d)
        {
            if ((t /= d / 2) < 1) return ((c / 2) * (t * t)) + b;
            return -c / 2 * (((t - 2) * (--t)) - 1) + b;
        }
    }
    public Quad quad;
    //t = 時間 d = 始点 c = 終点-始点 d = 経過時間
    public class Quart
    {
        public float In(float t, float b, float c, float d)
        {
            return c * (t /= d) * t * t * t + b;
        }
        public float Out(float t, float b, float c, float d)
        {
            return -c * ((t = t / d - 1) * t * t * t - 1) + b;
        }
        public float InOut(float t, float b, float c, float d)
        {
            if ((t /= d / 2) < 1) return c / 2 * t * t * t * t + b;
            return -c / 2 * ((t -= 2) * t * t * t - 2) + b;
        }
    }
    public Quart quart;
    //t = 時間 d = 始点 c = 終点-始点 d = 経過時間
    public class Quint
    {
        public float In(float t, float b, float c, float d)
        {
            return c * (t /= d) * t * t * t * t + b;
        }
        public float Out(float t, float b, float c, float d)
        {
            return c * ((t = t / d - 1) * t * t * t * t + 1) + b;
        }
        public float InOut(float t, float b, float c, float d)
        {
            if ((t /= d / 2) < 1) return c / 2 * t * t * t * t * t + b;
            return c / 2 * ((t -= 2) * t * t * t * t + 2) + b;
        }
    }
    public Quint quint;
};