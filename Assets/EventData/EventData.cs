using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[System.Serializable]
[CreateAssetMenu(fileName = "EventDataList ", menuName = "EventDataList")]
public class EventData : ScriptableObject
{
    [Label("イベントを識別するユニークな番号")]
    public int id;
    public List<BaseEventData> eventDataList;
    public List<BaseRequirementData> requirementDataList; //スキルの発生条件(条件を満たすとこのスキルを発動できる)
    [HideInInspector]
    public bool isUsed = false; //このイベントを実行したかどうか

    public void init()
    {
        foreach(BaseEventData eventData in eventDataList) eventData.Init();
        foreach(BaseRequirementData requirementData in requirementDataList) requirementData.Init();
        isUsed = false;
    }

    public bool isRequirements()
    {
        if (isUsed) return false;
        foreach(BaseRequirementData requirementData in requirementDataList) if(!requirementData.IsRequirement()) return false;
        return true;
    }

    public EventData deepCopy()
    {
        EventData copy = CreateInstance<EventData>();
        copy.id = id;

        copy.eventDataList = new List<BaseEventData>();
        if (eventDataList != null) foreach(BaseEventData eventData in eventDataList) copy.eventDataList.Add(eventData.Copy());
        
        copy.requirementDataList = new List<BaseRequirementData>();
        if (requirementDataList != null) foreach(BaseRequirementData requirementData in requirementDataList) copy.requirementDataList.Add(requirementData.Copy());
        return copy;
    }
}