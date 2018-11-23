using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

/// <summary>
/// 2018/10/7 板倉広司
/// 2018/11/15 板倉広司 ：　読み込んだ後のデータをもっと扱いやすいデータへ変更
/// 
/// 
/// 
/// イベントのテキストデータを読みこんで、それを文字列の配列として保持するクラス
/// このクラスは文法チェックと構文解析を行う
/// 
/// イベントの文法について
/// ・基本
///     全ての文はセミコロンで区切られている
///     改行やスペース、Tabは無視されている
///     文を区切るのは、すべて「,（カンマ）」
///     
///・コメント
///     先頭にスラッシュが付いているものをコメント（イベント読み込みスキップ）を表す
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
///     この文はactionタグ以外で使うとエラーになる
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


namespace TextEvent
{
    public class EventPerser
    {
        //イベント文の種類ごとにまとまったデータ
        public class EventData
        {
            public EventData()
            {
                this.termText = new List<TermText>();
                this.actionText = new List<ActionText>();
                this.labelData = new Dictionary<string, int>();
            }

            //条件
            public List<TermText> termText;
            //イベント内容
            public List<ActionText> actionText;
            //イベントを飛ぶ時のactionTextの位置を、ラベルの名前と結び付けている
            public Dictionary<string, int> labelData;
        }

        //イベントテキストの構文解析後(一文ごとにまとまっている)
        public struct ActionText
        {
            //タグの種類
            public enum TagType
            {
                TagStart,//開きタグ
                TagEnd,//閉じタグ
                None,//タグじゃない
            }
            //タグ内部のイベントの種類
            public enum EventType
            {
                Term,
                Action,
                None,
            }

            public string eventName;
            public string[] args;
        }
        //イベントテキストの構文解析後(一文ごとにまとまっている)
        public struct TermText
        {
            public string eventName;
            public bool requestedBoolean;
            public string[] args;
        }

        //メンバ変数
        private string[] file;
        private EventData data;

        //コンストラクタにて構文解析
        public EventPerser(string filePath)
        {
            TextAsset text = Resources.Load(filePath, typeof(TextAsset)) as TextAsset;
            Debug.Log(filePath);
            Debug.Log(text);

            //文字を列ごとに読み出し
            string allText = text.text;

            //スペースと改行を削除
            string fixedText = string.Join("", allText.Split(' ', '\t'));
            fixedText = string.Join("", fixedText.Split('\n'));
            fixedText = string.Join("", fixedText.Split('\r'));
            //セミコロンで文字列を行ごとに分割
            this.file = fixedText.Split(';');


            //初期化
            this.data = new EventData();
            this.data.actionText.Clear();
            this.data.termText.Clear();
            this.data.labelData.Clear();

            //タグの種類を記録
            ActionText.EventType tagType = ActionText.EventType.None;
            //一列ずつ構文解析(最後の部分は、分割の関係上必ず文字数０の要素になるので-1をする)
            for (int i = 0; i < this.file.Length - 1; ++i)
            {
                var tag = CheckType(this.file[i]);
                //タグが来るまで文を進める
                if (tag == ActionText.TagType.TagStart)
                {
                    tagType = CheckTag(this.file[i]);
                    //次回から読み込み
                    continue;
                }
                if (tag == ActionText.TagType.TagEnd)
                {
                    tagType = ActionText.EventType.None;
                    //次回から読み込み
                    continue;
                }

                //タグの内容によって読み込み
                ReadEvent(tagType, this.file[i]);

                //終わったらファイル読み込み終了
                Resources.UnloadAsset(text);
            }
        }

        //メンバ変数にイベントテキストを分解したデータを収める
        private void SplitTextLine(ref ActionText writeData, string lineText)
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

        //構文解析後のテキストを獲得
        public EventData GetEventText()
        {
            return this.data;
        }

        //イベント文の種類を判別
        private ActionText.TagType CheckType(string lineText)
        {
            //タグを見つけた場合
            if (lineText.IndexOf(">") != -1)
            {
                //sが先頭についてくるのが開きタグ
                if (lineText.ElementAt(0) == 's')
                {
                    return ActionText.TagType.TagStart;
                }
                //eが先頭についてくるのが閉じタグ
                if (lineText.ElementAt(0) == 'e')
                {
                    return ActionText.TagType.TagEnd;
                }
                //開きタグでも閉じタグでもないので例外
                throw new System.Exception("syntax error: " + lineText);
            }
            return ActionText.TagType.None;
        }

        //タグの内部を解析
        private ActionText.EventType CheckTag(string lineText)
        {
            if (lineText.IndexOf("term") != -1)
            {
                return ActionText.EventType.Term;
            }
            if (lineText.IndexOf("action") != -1)
            {
                return ActionText.EventType.Action;
            }
            //そんなタグの名前は存在しねえ！
            throw new System.Exception("syntax error: invalid tagName: " + lineText);
        }

        //イベント文をタグの種類ごとに解釈
        private void ReadEvent(ActionText.EventType eventType, string lineText)
        {
            //コメント文を見つけた場合
            if (lineText.ElementAt(0) == '/')
            {
                return;
            }

            switch (eventType)
            {
                case ActionText.EventType.Action:
                    {
                        //ラベルを見つけた場合
                        if (lineText.ElementAt(0) == '@')
                        {
                            //現在のaction配列の位置のその次を指定して今回の読み込みをスキップ
                            this.data.labelData[lineText.Substring(1)] = this.data.actionText.Count;
                            return;
                        }
                        //イベントの具体的な内容として文を解釈
                        ActionText actionEvent = new ActionText();
                        SplitTextLine(ref actionEvent, lineText);
                        //追加
                        this.data.actionText.Add(actionEvent);
                        break;
                    }
                case ActionText.EventType.Term:
                    {
                        //イベントの具体的な内容として文を解釈
                        ActionText actionEvent = new ActionText();
                        TermText termEvent = new TermText();

                        SplitTextLine(ref actionEvent, lineText);

                        //イベント追加
                        termEvent.eventName = actionEvent.eventName;
                        //引数の一つ目が必ず、「trueの時に条件を満たすか」「falseの時に条件を満たすか」を示す
                        termEvent.requestedBoolean = TextEvent.TextParser.ParseBoolean(actionEvent.args[0]);
                        //引数をコピー（一つ目はスキップ）
                        termEvent.args = new string[actionEvent.args.Length - 1];
                        System.Array.Copy(actionEvent.args, 1, termEvent.args, 0, actionEvent.args.Length - 1);

                        //追加
                        this.data.termText.Add(termEvent);
                        break;
                    }
                default:
                    //Noneとかタグとかは何もしない
                    break;
            }


        }
    }
}
