using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "ConversationDeck ", menuName = "Deck/ConversationDeckData")]
public class ConversationDeckData : ScriptableObject
{
    public int id;
    [Label("カードのタイトル")]
    public string title;
    [Label("送信するメッセージ")]
    public string messageText;
    [Label("カードの説明テキスト")]
    public string description;
    [Label("送信可能なトークidリスト")]
    public List<int> sendableTalkIdList;

    public virtual string GetMessageText()
    {
        return messageText;
    }

    public virtual ConversationDeckData Copy()
    {
        ConversationDeckData copy = CreateInstance<ConversationDeckData>();
        copy.id = id;
        copy.title = title;
        copy.messageText = messageText;
        copy.description = description;
        copy.sendableTalkIdList = new List<int>();
        if (sendableTalkIdList != null) copy.sendableTalkIdList = sendableTalkIdList;
        return copy;
    }

    public ConversationDeckData Copy(ConversationDeckData copy)
    {
        copy.id = id;
        copy.title = title;
        copy.messageText = messageText;
        copy.description = description;
        copy.sendableTalkIdList = new List<int>();
        if (sendableTalkIdList != null) copy.sendableTalkIdList = sendableTalkIdList;
        return copy;
    }
}
