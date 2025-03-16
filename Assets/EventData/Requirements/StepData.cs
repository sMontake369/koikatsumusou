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

    public override void Init()
    {
        if (isRelative) initStep = GameManager.gamM.GetStep();
        else initStep = 0;
    }

    public override bool IsRequirement()
    {
        return GameManager.gamM.GetStep() - initStep >= step;
    }

    public override BaseRequirementData Copy()
    {
        StepData copy = CreateInstance<StepData>();
        copy.step = step;
        copy.isRelative = isRelative;
        return copy;
    }
}
