using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "AddEvent ", menuName = "EventData/AddEvent")]
public class AddEventData : BaseEventData
{
    public int talkId;
    public List<EventData> eventDataList;
    public override BaseEventData Copy()
    {
        AddEventData copy = CreateInstance<AddEventData>();
        copy.talkId = talkId;
        copy.eventDataList = new List<EventData>();
        if (eventDataList != null) copy.eventDataList = DeepCopy.DeepCopyList(eventDataList);
        return copy;
    }

    public override void Init()
    {
        
    }

    protected override UniTask DoEvent(CancellationToken token)
    {
        TalkManager talM = GameManager.smaM.GetAppManager<LineManager>().GetTalkManager(talkId);
        foreach (EventData eventData in eventDataList) talM.addEvent(eventData);
        return UniTask.CompletedTask;
    }
}
