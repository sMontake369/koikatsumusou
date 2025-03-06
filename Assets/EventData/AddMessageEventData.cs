using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using System.Threading;
using System;

[CreateAssetMenu(fileName = "AddMessageEventData", menuName = "EventData/AddMessageEventData")]
public class AddMessageEventData : BaseEventData
{
    public List<MessageData> messageDataList;
    CancellationTokenSource cts2;

    public override void init()
    {

    }

    public override void next()
    {
        cts2.Cancel();
    }

    protected override async UniTask doEvent(CancellationToken token)
    {
        foreach (MessageData messageData in messageDataList)
        {
            cts2 = new CancellationTokenSource();
            TalkManager talM = GameManager.smaM.getAppManager<LineManager>().GetTalkManager(messageData.talkId);
            if (talM.canReceiveMessage(messageData))
            {
                try { await UniTask.Delay((int)math.lerp(1000, 3000, (float)math.min(20, messageData.message.Length) / 20), cancellationToken: cts2.Token); }
                catch (Exception) { }

                token.ThrowIfCancellationRequested();
                if (talM.canReceiveMessage(messageData)) 
                {
                    talM.addMessage(messageData);
                    GameManager.gamM.addStep();
                }
            }
        }
    }

    public override BaseEventData deepCopy()
    {
        AddMessageEventData copy = CreateInstance<AddMessageEventData>();

        copy.messageDataList = new List<MessageData>();
        if (messageDataList != null) copy.messageDataList = DeepCopy.DeepCopyList(messageDataList);
        return copy;
    }
}
