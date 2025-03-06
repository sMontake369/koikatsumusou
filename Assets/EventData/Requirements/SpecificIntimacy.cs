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
    public override BaseRequirementData deepCopy()
    {
        SpecificIntimacy copy = CreateInstance<SpecificIntimacy>();
        copy.friendId = friendId;
        copy.reqIntimacy = reqIntimacy;
        copy.isLessThan = isLessThan;
        copy.linM = linM;
        return copy;
    }

    public override void init()
    {
        linM = GameManager.smaM.getAppManager<LineManager>();
    }

    public override bool isRequirement()
    {
        int nowIntimacy = linM.getFriendIntimacy(friendId);
        if (isLessThan) 
        {
            return nowIntimacy < reqIntimacy;
        }
        else return nowIntimacy >= reqIntimacy;
    }
}
