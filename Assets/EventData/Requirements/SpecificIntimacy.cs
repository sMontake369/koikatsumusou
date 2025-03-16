using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "SpecificIntimacy ", menuName = "Requirements/SpecificIntimacy")]
public class SpecificIntimacy : BaseRequirementData
{
    [Label("フレンドID")]
    public int friendId;
    [Label("条件の親密度")]
    public int reqIntimacy;
    [Label("一定の親密度未満の場合ON")]
    public bool isLessThan;

    LineManager linM;
    public override BaseRequirementData Copy()
    {
        SpecificIntimacy copy = CreateInstance<SpecificIntimacy>();
        copy.friendId = friendId;
        copy.reqIntimacy = reqIntimacy;
        copy.isLessThan = isLessThan;
        copy.linM = linM;
        return copy;
    }

    public override void Init()
    {
        linM = GameManager.smaM.GetAppManager<LineManager>();
    }

    public override bool IsRequirement()
    {
        int nowIntimacy = linM.GetFriendIntimacy(friendId);
        if (isLessThan) 
        {
            return nowIntimacy < reqIntimacy;
        }
        else return nowIntimacy >= reqIntimacy;
    }
}
