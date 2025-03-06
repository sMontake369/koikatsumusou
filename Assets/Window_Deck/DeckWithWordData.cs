using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "ConversationWithWord ", menuName = "Deck/ConversationWithWordData")]
public class DeckWithWordData : ConversationDeckData
{
    [Label("ワードの挿入位置")]
    public int insertPos;
    
    [HideInInspector]
    public WordData wordData;

    public void setWordData(WordData wordData)
    {
        this.wordData = wordData;
    }

    public WordData GetWordData()
    {
        return wordData;
    }

    public override string getMessageText()
    {
        if (wordData == null) return messageText.Insert(insertPos, "○○○");
        else return messageText.Insert(insertPos, wordData.word);
    }

    public override ConversationDeckData deepCopy()
    {
        DeckWithWordData copy = CreateInstance<DeckWithWordData>();
        copy = base.deepCopy(copy) as DeckWithWordData;
        copy.wordData = wordData;
        copy.insertPos = insertPos;
        return copy;
    }
}
