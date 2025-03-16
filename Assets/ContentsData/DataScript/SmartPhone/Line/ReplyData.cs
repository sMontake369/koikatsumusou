using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "ReplyData ", menuName = "LineData/ReplyData")]
public class ReplyData : ScriptableObject
{
    [Label("求める会話デッキID")]
    public int requirementDeckId; //求める会話デッキID
    public List<BaseEventData> eventDataList; //条件を満たしたときに実行するイベント

    public virtual void Init()
    {
        foreach(BaseEventData eventData in eventDataList) eventData.Init();
    }

    public virtual ReplyData Copy()
    {
        ReplyData copy = CreateInstance<ReplyData>();
        copy.requirementDeckId = requirementDeckId;
        copy.eventDataList = new List<BaseEventData>();
        copy.eventDataList = DeepCopy.DeepCopyList(eventDataList);
        return copy;
    }

    public ReplyData Copy(ReplyData copy)
    {
        copy.requirementDeckId = requirementDeckId;
        copy.eventDataList = new List<BaseEventData>();
        copy.eventDataList = DeepCopy.DeepCopyList(eventDataList);
        return copy;
    }
}
