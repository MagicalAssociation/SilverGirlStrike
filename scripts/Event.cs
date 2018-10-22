using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

/// <summary>
/// 2018/10/7 板倉広司
/// イベントのテキストデータを読みこんで、それを文字列の配列として保持するクラス
/// このクラスは文法チェックと構文解析を行う
/// 
/// イベントの文法について
/// ・基本
///     全ての文はセミコロンで区切られている
///     改行やスペース、Tabは無視されている
///     文を区切るのは、すべて「,（カンマ）」
/// 
/// ・タグ
///     イベント記述の内容を解析するのに使用し、タグの間にイベント記述やラベルを挟む
///     「s>（s + 大なり）」から始まる文が開きタグ扱いとなる
///     「e>（e + 大なり）」から始まる文が閉じタグ扱いとなる
///     ラベルとは違って、イベント記述には必須
///     文法）
///     s>term;
///     e>term;
///     s>action;
///     e>action;
/// 
/// ・ラベル
///     主に目印として使われ、「@（アットマーク）」から始まる文はすべてラベル扱いとなる
///     GOTO文なんかで使用
///     文法）
///     @ + ラベル名;
/// 
/// ・イベント記述
///     条件文（termタグ内部の場合）
///     文法）
///     条件名 , 条件を満たすのは判定がどういうときか（'t' or 'f'） , 引数 ;
/// 
///     処理文（actionタグ内部の場合）
///     文法）
///     処理名 , 引数
/// 
/// 
/// </summary>





public class Event
{
    //イベントテキストの構文解析後
    public struct EventText
    {
        public enum EventType
        {
            Term, Action, Tag
        }

        public string eventName;
        public string[] args;
        public EventType type;
    }




    public Event(string filePath)
    {
        TextAsset text = new TextAsset();
        text = Resources.Load(filePath, typeof(TextAsset)) as TextAsset;

        //文字を列ごとに読み出し
        string allText = text.text;
        Debug.Log(allText);

        //スペースと改行を削除
        string fixedText = string.Join("", allText.Split(' ', '\n'));
        //セミコロンで文字列を行ごとに分割
        this.file = fixedText.Split(';');

        this.eventTexts = new EventText[this.file.Length];
        //一列ずつ構文解析
        for (int i = 0; i < this.file.Length; ++i)
        {
            SplitTextLine(ref this.eventTexts[i], ref this.file[i]);
        }
    }

    //メンバ変数にイベントテキストを分解したデータを収める
    private void SplitTextLine(ref EventText writeData, ref string lineText)
    {
        //特定の文字で分割
        char[] splitElements = { ',' };
        List<string> splited = lineText.Split(splitElements).ToList();

        //イベント用のフォーマットに落とし込む

        //イベント名
        writeData.eventName = splited[0];
        splited.RemoveAt(0);

        //イベントの引数
        writeData.args = splited.ToArray();
    }


    public EventText[] GetEventText()
    {
        return this.eventTexts;
    }


    //メンバ変数
    private string[] file;
    private EventText[] eventTexts;


}
