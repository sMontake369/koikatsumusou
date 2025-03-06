using System.Threading;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "DeleteEvent ", menuName = "EventData/DeleteEvent")]
public class DeleteEvent : BaseEventData
{
    [Label("トークID")]
    public int talkId;
    [Label("削除するイベントID")]
    public int id;
    
    public override void init()
    {

    }

    protected override UniTask doEvent(CancellationToken token)
    {
        TalkManager talM = GameManager.smaM.getAppManager<LineManager>().GetTalkManager(talkId);
        talM.removeEvent(id);
        return UniTask.CompletedTask;
    }

    public override BaseEventData deepCopy()
    {
        DeleteEvent copy = CreateInstance<DeleteEvent>();
        copy.talkId = talkId;
        copy.id = id;
        return copy;
    }
}
