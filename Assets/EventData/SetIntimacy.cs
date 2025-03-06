using System.Threading;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "SetIntimacy ", menuName = "EventData/SetIntimacy")]
public class SetIntimacy : BaseEventData
{
    [Label("フレンドID")]
    public int friendId;
    [Label("変化させる親密度スコア")]
    public int addIntimacy;

    LineManager linM;

    public override BaseEventData deepCopy()
    {
        SetIntimacy copy = CreateInstance<SetIntimacy>();
        copy.friendId = friendId;
        copy.addIntimacy = addIntimacy;
        return copy;
    }

    public override void init()
    {
        linM = GameManager.smaM.getAppManager<LineManager>();
    }

    protected override UniTask doEvent(CancellationToken token)
    {
        linM.setFriendIntimacy(friendId, addIntimacy);
        return UniTask.CompletedTask;
    }
}
