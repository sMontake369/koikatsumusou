using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "RemoveQuestion ", menuName = "EventData/RemoveQuestion")]
public class RemoveQuestion : BaseEventData
{
    public int talkId;

    public override void init()
    {
        
    }

    public override BaseEventData deepCopy()
    {
        RemoveQuestion copy = CreateInstance<RemoveQuestion>();
        copy.talkId = talkId;
        return copy;
    }

    protected override UniTask doEvent(CancellationToken token)
    {
        GameManager.smaM.getAppManager<LineManager>().GetTalkManager(talkId).removeQuestion();
        return UniTask.CompletedTask;
    }
}
