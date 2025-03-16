using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ReplyWithWordData ", menuName = "LineData/ReplyWithWordData")]
public class ReplyWithWordData : ReplyData
{
    public WordData wordData; //求める単語
    public List<BaseEventData> diffWordEventDataList; //単語の種類は一緒で単語のみが違うときに実行するイベント
    public List<BaseEventData> diffAllEventDataList; //単語の種類も違うときに実行するイベント

    public override void Init()
    {
        base.Init();
        foreach (BaseEventData eventData in diffWordEventDataList) eventData.Init();
        foreach (BaseEventData eventData in diffAllEventDataList) eventData.Init();
    }

    public override ReplyData Copy()
    {
        ReplyWithWordData copy = CreateInstance<ReplyWithWordData>();
        copy = base.Copy(copy) as ReplyWithWordData;
        copy.wordData = wordData;
        copy.diffWordEventDataList = new List<BaseEventData>();
        if (diffWordEventDataList != null) copy.diffWordEventDataList = DeepCopy.DeepCopyList(diffWordEventDataList);

        copy.diffAllEventDataList = new List<BaseEventData>();
        if (diffAllEventDataList != null) copy.diffAllEventDataList = DeepCopy.DeepCopyList(diffAllEventDataList);
        return copy;
    }
}
