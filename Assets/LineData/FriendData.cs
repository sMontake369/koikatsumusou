using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "FriendData", menuName = "LineData/FriendData")]
public class FriendData : ScriptableObject
{
    public int id;
    public string userName;
    public Texture2D icon;
    public string successResultMessage;
    public string blockResultMessage;
    public string progressResultMessage;

    [HideInInspector]
    public EFriendStatus status; //進行状況
    [HideInInspector]
    public int intimacyScore; //親密度

    public void init()
    {
        status = EFriendStatus.Progress;
        intimacyScore = 50;
    }

    public FriendData deepCopy()
    {
        FriendData copy = CreateInstance<FriendData>();
        copy.id = id;
        copy.userName = userName;
        copy.icon = icon;
        copy.status = status;
        copy.intimacyScore = intimacyScore;
        copy.successResultMessage = successResultMessage;
        copy.blockResultMessage = blockResultMessage;
        copy.progressResultMessage = progressResultMessage;
        return copy;
    }
}

public enum EFriendStatus
{
    Progress, // 進行中
    Block, // ブロック
    Success, // 成功
}