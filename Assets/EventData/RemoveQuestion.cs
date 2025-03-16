using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "RemoveQuestion ", menuName = "EventData/RemoveQuestion")]
public class RemoveQuestion : BaseEventData
{
    public int talkId;

    public override void Init()
    {
        
    }

    public override BaseEventData Copy()
    {
        RemoveQuestion copy = CreateInstance<RemoveQuestion>();
        copy.talkId = talkId;
        return copy;
    }

    protected override UniTask DoEvent(CancellationToken token)
    {
        GameManager.smaM.GetAppManager<LineManager>().GetTalkManager(talkId).removeQuestion();
        return UniTask.CompletedTask;
    }
}
