using UnityEngine;

[System.Serializable]
public abstract class BaseRequirementData : ScriptableObject //発生条件(条件を満たすとこのスキルを発動できる)
{
    public abstract void init(); //初期化
    public abstract bool isRequirement(); //発生条件を満たしているか
    public abstract BaseRequirementData deepCopy(); //ディープコピー
}