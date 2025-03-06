using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "StepData", menuName = "Requirements/StepData")]
public class StepData : BaseRequirementData
{
    [Label("必要なステップ数")]
    public int step;
    [Label("登録時からの相対数か")]
    public bool isRelative = false;

    int initStep = 0;

    public override void init()
    {
        if (isRelative) initStep = GameManager.gamM.getStep();
        else initStep = 0;
    }

    public override bool isRequirement()
    {
        return GameManager.gamM.getStep() - initStep >= step;
    }

    public override BaseRequirementData deepCopy()
    {
        StepData copy = CreateInstance<StepData>();
        copy.step = step;
        copy.isRelative = isRelative;
        return copy;
    }
}
