using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "StageData", menuName = "ContentsData/StageData")]
public class StageData : ScriptableObject
{
    [Tooltip("ステージ名")]
    public string stageName;

    [Label("デッキデータ")]
    public List<ConversationDeckData> deckDataList;
    public Texture2D stageImage;
    [Label("アプリケーションデータ")]
    public List<BaseAppData> applicationDataList;

    [Label("最大ステップ数")]
    public int maxStep = 500;

    public StageData Copy()
    {
        StageData stageData = CreateInstance<StageData>();
        stageData.stageName = stageName;
        stageData.stageImage = stageImage;
        stageData.maxStep = maxStep;

        stageData.applicationDataList = new List<BaseAppData>();
        if (applicationDataList != null) foreach (BaseAppData appData in applicationDataList) stageData.applicationDataList.Add(appData.Copy());

        stageData.deckDataList = new List<ConversationDeckData>();
        foreach (var deckData in deckDataList) stageData.deckDataList.Add(deckData.Copy());
        
        return stageData;
    }
}