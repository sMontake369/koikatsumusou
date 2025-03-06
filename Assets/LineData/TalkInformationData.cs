using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;

[System.Serializable]
[CreateAssetMenu(fileName = "TalkInformationData", menuName = "LineData/TalkInformationData")]
public class TalkInformationData : ScriptableObject // フレンドの個人チャットorグループを統括するデータ
{
    [Label("トークID")]
    public int id; // トークID
    [Label("参加している友達データ")]
    public List<int> joinFriendIdList; //参加している友達のIDリスト
    [Label("フレンド又はグループの名前")]
    public string talkName; //フレンドの名前orグループ名
    [Label("フレンド又はグループの説明")]
    public string description; //フレンドの説明orグループの説明
    [Label("フレンド又はグループのアイコン")]
    public Texture2D icon; //フレンドのアイコンorグループのアイコン
    [Label("メッセージデータ")]
    public List<MessageData> messageDataList; //メッセージデータ

    [Label("質問データ")]
    public ReplyData questionData; //キャラクタが質問等で特定の返答を欲している時に入る。ここにデータがるときは返答はせず、こちらを使用する。
    [Label("返答データ")]
    public List<ReplyData> replyDataList; //返答データ
    [Label("デフォルト質問返答")]
    public List<BaseEventData> defaultQuestionReply; //質問データと関係ない会話デッキを受け取った時に実行するイベント
    [Label("デフォルト返答")]
    public List<BaseEventData> defaultReply; //返答データに存在しない単語を受け取った時に実行するイベント
    [Label("イベントデータ")]
    public List<EventData> eventDataList; //特定のタイミングで実行されるイベントデータ

    public void init()
    {
        if (messageDataList != null) foreach (MessageData messageData in messageDataList) messageData.init();
        if (questionData != null) questionData.init();
        if (replyDataList != null) foreach (ReplyData replyData in replyDataList) replyData.init();
        if (defaultQuestionReply != null) foreach (BaseEventData eventData in defaultQuestionReply) eventData.init();
        if (defaultReply != null) foreach (BaseEventData eventData in defaultReply) eventData.init();
        if (eventDataList != null) foreach (EventData eventData in eventDataList) eventData.init();

        foreach (MessageData messageData in messageDataList) messageData.talkId = id;

        if (joinFriendIdList.Count == 0) Debug.LogError("参加している友達がいません。talkName:" + talkName);
    }

    public TalkInformationData deepCopy()
    {
        TalkInformationData copy = CreateInstance<TalkInformationData>();
        copy.id = id;
        copy.talkName = talkName;
        copy.description = description;
        copy.icon = icon;
        copy.messageDataList = DeepCopy.DeepCopyList(messageDataList);

        copy.joinFriendIdList = new List<int>();
        copy.joinFriendIdList = joinFriendIdList;

        if (joinFriendIdList != null) copy.joinFriendIdList = joinFriendIdList;

        if (questionData != null) copy.questionData = questionData.deepCopy();

        copy.replyDataList = new List<ReplyData>();
        if (replyDataList != null) foreach (ReplyData replyData in replyDataList) copy.replyDataList.Add(replyData.deepCopy());

        copy.defaultQuestionReply = new List<BaseEventData>();
        if (defaultQuestionReply != null) foreach (BaseEventData eventData in defaultQuestionReply) copy.defaultQuestionReply.Add(eventData.deepCopy());

        copy.defaultReply = new List<BaseEventData>();
        if (defaultReply != null) foreach (BaseEventData eventData in defaultReply) copy.defaultReply.Add(eventData.deepCopy());

        copy.eventDataList = new List<EventData>();
        if (eventDataList != null) foreach (EventData eventData in eventDataList) copy.eventDataList.Add(eventData.deepCopy());
        return copy;
    }
}
