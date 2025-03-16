using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "RemoveDeck ", menuName = "EventData/RemoveDeckData")]
public class RemoveDeckData : BaseEventData
{
    public List<int> deckIdList;

    public override void Init()
    {
        
    }

    protected override UniTask DoEvent(CancellationToken cts)
    {
        foreach (var deck in deckIdList) GameManager.decM.RemoveDeck(deck);
        return UniTask.CompletedTask;
    }

    public override BaseEventData Copy()
    {
        RemoveDeckData copy = CreateInstance<RemoveDeckData>();
        copy.deckIdList = deckIdList;
        return copy;
    }
}
