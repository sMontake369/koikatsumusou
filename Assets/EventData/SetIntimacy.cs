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

    public override BaseEventData Copy()
    {
        SetIntimacy copy = CreateInstance<SetIntimacy>();
        copy.friendId = friendId;
        copy.addIntimacy = addIntimacy;
        return copy;
    }

    public override void Init()
    {
        linM = GameManager.smaM.GetAppManager<LineManager>();
    }

    protected override UniTask DoEvent(CancellationToken token)
    {
        linM.SetFriendIntimacy(friendId, addIntimacy);
        return UniTask.CompletedTask;
    }
}
