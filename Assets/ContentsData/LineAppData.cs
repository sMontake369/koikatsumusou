using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "LineAppData ", menuName = "AppData/LineAppData")]
public class LineAppData : BaseAppData
{
    [Label("会話データ")]
    public List<TalkInformationData> tInfoDataList;
    [Label("フレンドデータ")]
    public List<FriendData> friendDataList;
    [Label("自分のデータ")]
    public FriendData ownData;

    public void init()
    {
        ownData.init();
        foreach (var tInfo in tInfoDataList) tInfo.init();
        foreach (var friendData in friendDataList) friendData.init();
    }

    public override BaseAppData deepCopy()
    {
        LineAppData copy = CreateInstance<LineAppData>();
        copy.appName = appName;
        copy.ownData = ownData.deepCopy();

        copy.friendDataList = new List<FriendData>();
        foreach (var friendData in friendDataList) copy.friendDataList.Add(friendData.deepCopy());

        copy.tInfoDataList = new List<TalkInformationData>();
        foreach (var tInfoData in tInfoDataList) copy.tInfoDataList.Add(tInfoData.deepCopy());

        return copy;
    }
}
