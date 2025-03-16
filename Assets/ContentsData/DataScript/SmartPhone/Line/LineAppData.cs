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

    public void Init()
    {
        ownData.Init();
        foreach (var tInfo in tInfoDataList) tInfo.Init();
        foreach (var friendData in friendDataList) friendData.Init();
    }

    public override BaseAppData Copy()
    {
        LineAppData copy = CreateInstance<LineAppData>();
        copy.appName = appName;
        copy.ownData = ownData.DeepCopy();

        copy.friendDataList = new List<FriendData>();
        foreach (var friendData in friendDataList) copy.friendDataList.Add(friendData.DeepCopy());

        copy.tInfoDataList = new List<TalkInformationData>();
        foreach (var tInfoData in tInfoDataList) copy.tInfoDataList.Add(tInfoData.Copy());

        return copy;
    }
}
