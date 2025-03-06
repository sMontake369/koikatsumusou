using UnityEngine;

public class NotificationData
{
    public Texture2D icon;
    public string appName;
    public string message;
    public string sendTime;
    public BaseAppManager appManager;
    public Object userData;

    public NotificationData(MessageData messageData, BaseAppManager appManager)
    {
        FriendData friendData = GameManager.smaM.getAppManager<LineManager>().GetFriendData(messageData.friendId);
        icon = friendData.icon;
        appName = friendData.userName;
        message = messageData.message;
        sendTime = GameManager.gamM.getTime();
        this.appManager = appManager;
    }
}
