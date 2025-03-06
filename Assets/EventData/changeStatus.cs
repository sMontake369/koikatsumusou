using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "changeStatus ", menuName = "EventData/changeStatus")]
public class changeStatus : BaseEventData
{
    public int friendId;
    public EFriendStatus status;

    LineManager lineM;

    public override BaseEventData deepCopy()
    {
        changeStatus copy = CreateInstance<changeStatus>();
        copy.friendId = friendId;
        copy.status = status;
        return copy;
    }

    public override void init()
    {
        lineM = GameManager.smaM.getAppManager<LineManager>();
    }

    protected override UniTask doEvent(CancellationToken token)
    {
        lineM.setFriendStatus(friendId, status);
        lineM.checkGameEnd();
        lineM.onChangeFriendStatus(friendId);
        return UniTask.CompletedTask;
    }
}
