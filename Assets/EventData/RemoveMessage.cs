using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "RemoveMessage", menuName = "EventData/RemoveMessage")]
public class RemoveMessage : BaseEventData
{
    public int talkId;
    public List<MessageData> messageDataList;

    LineManager linM;

    public override BaseEventData deepCopy()
    {
        RemoveMessage copy = CreateInstance<RemoveMessage>();
        copy.messageDataList = new List<MessageData>();
        copy.messageDataList = DeepCopy.DeepCopyList(messageDataList);
        return copy;
    }

    public override void init()
    {
        linM = GameManager.smaM.getAppManager<LineManager>();
    }

    protected override UniTask doEvent(CancellationToken token)
    {
        TalkManager talkM = linM.GetTalkManager(talkId);
        foreach (MessageData messageData in messageDataList)
        {
            talkM.removeMessage(messageData);
        }
        return UniTask.CompletedTask;
    }
}
