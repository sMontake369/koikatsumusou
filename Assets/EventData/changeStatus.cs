using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "changeStatus ", menuName = "EventData/changeStatus")]
public class changeStatus : BaseEventData
{
    public int friendId;
    public EFriendStatus status;

    LineManager lineM;

    public override BaseEventData Copy()
    {
        changeStatus copy = CreateInstance<changeStatus>();
        copy.friendId = friendId;
        copy.status = status;
        return copy;
    }

    public override void Init()
    {
        lineM = GameManager.smaM.GetAppManager<LineManager>();
    }

    protected override UniTask DoEvent(CancellationToken token)
    {
        lineM.SetFriendStatus(friendId, status);
        lineM.CheckGameEnd();
        lineM.OnChangeFriendStatus(friendId);
        return UniTask.CompletedTask;
    }
}
