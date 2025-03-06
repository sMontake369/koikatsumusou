using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "RemoveDeck ", menuName = "EventData/RemoveDeckData")]
public class RemoveDeckData : BaseEventData
{
    public List<int> deckIdList;

    public override void init()
    {
        
    }

    protected override UniTask doEvent(CancellationToken cts)
    {
        foreach (var deck in deckIdList) GameManager.decM.removeDeck(deck);
        return UniTask.CompletedTask;
    }

    public override BaseEventData deepCopy()
    {
        RemoveDeckData copy = CreateInstance<RemoveDeckData>();
        copy.deckIdList = deckIdList;
        return copy;
    }
}
