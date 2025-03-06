using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ReplyWithWordData ", menuName = "LineData/ReplyWithWordData")]
public class ReplyWithWordData : ReplyData
{
    public WordData wordData; //求める単語
    public List<BaseEventData> diffWordEventDataList; //単語の種類は一緒で単語のみが違うときに実行するイベント
    public List<BaseEventData> diffAllEventDataList; //単語の種類も違うときに実行するイベント

    public override void init()
    {
        base.init();
        foreach (BaseEventData eventData in diffWordEventDataList) eventData.init();
        foreach (BaseEventData eventData in diffAllEventDataList) eventData.init();
    }

    public override ReplyData deepCopy()
    {
        ReplyWithWordData copy = CreateInstance<ReplyWithWordData>();
        copy = base.deepCopy(copy) as ReplyWithWordData;
        copy.wordData = wordData;
        copy.diffWordEventDataList = new List<BaseEventData>();
        if (diffWordEventDataList != null) copy.diffWordEventDataList = DeepCopy.DeepCopyList(diffWordEventDataList);

        copy.diffAllEventDataList = new List<BaseEventData>();
        if (diffAllEventDataList != null) copy.diffAllEventDataList = DeepCopy.DeepCopyList(diffAllEventDataList);
        return copy;
    }
}
