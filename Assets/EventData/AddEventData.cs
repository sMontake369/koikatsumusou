using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "AddEvent ", menuName = "EventData/AddEvent")]
public class AddEventData : BaseEventData
{
    public int talkId;
    public List<EventData> eventDataList;
    public override BaseEventData deepCopy()
    {
        AddEventData copy = CreateInstance<AddEventData>();
        copy.talkId = talkId;
        copy.eventDataList = new List<EventData>();
        if (eventDataList != null) copy.eventDataList = DeepCopy.DeepCopyList(eventDataList);
        return copy;
    }

    public override void init()
    {
        
    }

    protected override UniTask doEvent(CancellationToken token)
    {
        TalkManager talM = GameManager.smaM.getAppManager<LineManager>().GetTalkManager(talkId);
        foreach (EventData eventData in eventDataList) talM.addEvent(eventData);
        return UniTask.CompletedTask;
    }
}
