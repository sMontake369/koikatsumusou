using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "ConversationWithWord ", menuName = "Deck/ConversationWithWordData")]
public class DeckWithWordData : ConversationDeckData
{
    [Label("ワードの挿入位置")]
    public int insertPos;
    
    [HideInInspector]
    public WordData wordData;

    public override string GetMessageText()
    {
        if (wordData == null) return messageText.Insert(insertPos, "○○○");
        else return messageText.Insert(insertPos, wordData.word);
    }

    public override ConversationDeckData Copy()
    {
        DeckWithWordData copy = CreateInstance<DeckWithWordData>();
        copy = base.Copy(copy) as DeckWithWordData;
        copy.wordData = wordData;
        copy.insertPos = insertPos;
        return copy;
    }
}
