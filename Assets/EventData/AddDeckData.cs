using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "AddDeckData ", menuName = "EventData/AddDeckData")]
public class AddDeckData : BaseEventData
{
    public ConversationDeckData conversationDeckData;
    public override void Init()
    {
        
    }

    protected override UniTask DoEvent(CancellationToken cts)
    {
        GameManager.decM.AddDeck(conversationDeckData);
        GameManager.solM.SetSoliloquy("会話デッキ「" + conversationDeckData.title + "」を思いついた!").Forget();
        return UniTask.CompletedTask;
    }

    public override BaseEventData Copy()
    {
        AddDeckData copy = CreateInstance<AddDeckData>();
        copy.conversationDeckData = conversationDeckData.Copy();
        // copy.delay = delay;
        return copy;
    }
}
