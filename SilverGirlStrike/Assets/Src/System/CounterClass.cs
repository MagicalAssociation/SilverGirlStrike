using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//デルタタイムに対応した汎用カウンタクラス
//数値のカウントアップ、カウントダウンに加え、可変フレームに対応できる判定関数を提供する
public class CounterClass {
    public float count;
    public float prevCount;

    public float maxCount;
    public float minCount;

    CounterClass(int minCount, int maxCount)
    {
        this.count = 0.0f;
        this.prevCount = this.count;

        this.minCount = minCount;
        this.maxCount = maxCount;
    }

    //最大値と最小値を設定（範囲外に現在のカウントが行ったらクランプ）
    public void SetMinMaxCount(int minCount, int maxCount)
    {
        if (maxCount < minCount)
        {
            throw new System.Exception("最小値よりも小さな最大値を入れないでください");
        }
        if (minCount > maxCount)
        {
            throw new System.Exception("最大値よりも大きな最小値を入れないでください");
        }

        this.maxCount = maxCount;
        this.minCount = minCount;
        ClampCountMax();
        ClampCountMin();
    }

    //最大値を設定（範囲外に現在のカウントが行ったらクランプ）
    public void SetMaxCount(int maxCount)
    {
        if(maxCount < this.minCount)
        {
            throw new System.Exception("最小値よりも小さな最大値を入れないでください");
        }

        this.maxCount = maxCount;
        ClampCountMax();
        ClampCountMin();
    }
    //最小値を設定（範囲外に現在のカウントが行ったらクランプ）
    public void SetMinCount(int minCount)
    {
        if (minCount > this.maxCount)
        {
            throw new System.Exception("最大値よりも大きな最小値を入れないでください");
        }

        this.minCount = minCount;
        ClampCountMax();
        ClampCountMin();
    }

    //カウントを増やす
    public void CountUp()
    {
        this.prevCount = this.count;
        this.count += Time.deltaTime;
        ClampCountMax();
    }
    //カウントを減らす
    public void CountDown()
    {
        this.prevCount = this.count;
        this.count -= Time.deltaTime;
        ClampCountMin();
    }

    //カウントを最大値に(通過判定はない)
    public void ToCountMax()
    {
        this.count = this.maxCount;
        this.prevCount = this.maxCount;
    }
    //カウントを最少値に(通過判定はない)
    public void ToCountMin()
    {
        this.count = this.minCount;
        this.prevCount = this.minCount;
    }

    //指定のカウントを通過したかどうか
    public bool IsPassCount(int count)
    {
        bool judgeResult = this.count < this.prevCount;
        float minCount = judgeResult  ? this.count : this.prevCount;
        float maxCount = !judgeResult ? this.count : this.prevCount;

        //前回のカウントと今回のカウントで指定のカウントを挟めれば通過したと判定できる
        float judgeFactor = (float)count;
        if(minCount < judgeFactor && maxCount > judgeFactor)
        {
            return true;
        }

        return false;
    }

    //端数を無視したカウントを獲得
    public int GetCount()
    {
        return (int)this.count;
    }

    //最小値から最大値までのカウントの進みの比率を獲得する
    public float GetCountRatio()
    {
        if (this.maxCount == 0.0f)
        {
            return 0.0f;
        }
        return this.count / this.maxCount;
    }

    //最大値にクランプ
    private void ClampCountMax()
    {
        if(this.count > this.maxCount)
        {
            this.count = this.maxCount;
        }
    }
    //最小値にクランプ
    private void ClampCountMin()
    {
        if (this.count < this.minCount)
        {
            this.count = this.minCount;
        }
    }

}
