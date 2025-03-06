using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "AddDeckData ", menuName = "EventData/AddDeckData")]
public class AddDeckData : BaseEventData
{
    public ConversationDeckData conversationDeckData;
    public override void init()
    {
        
    }

    protected override UniTask doEvent(CancellationToken cts)
    {
        GameManager.decM.addDeck(conversationDeckData);
        GameManager.solM.setSoliloquy("会話デッキ「" + conversationDeckData.title + "」を思いついた!").Forget();
        return UniTask.CompletedTask;
    }

    public override BaseEventData deepCopy()
    {
        AddDeckData copy = CreateInstance<AddDeckData>();
        copy.conversationDeckData = conversationDeckData.deepCopy();
        // copy.delay = delay;
        return copy;
    }
}
