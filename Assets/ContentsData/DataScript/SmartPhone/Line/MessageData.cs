using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "MessageData ", menuName = "LineData/MessageData")]
public class MessageData : ScriptableObject
{
    public int friendId; //送った相手のID
    public int talkId; //トークID
    public string sendTime; //送信時間
    public string message; //メッセージ
    [Label("このメッセージを押したときに起こるイベント")]
    public List<BaseEventData> eventDataList = new List<BaseEventData>(); //このメッセージを押したときに起こるイベント
    [HideInInspector]
    public bool isUsed = false; //このメッセージを押したかどうか
    [HideInInspector]
    public ConversationDeckData deckData; //このメッセージを送信したデッキデータ

    public MessageData DeepCopy()
    {
        MessageData copy = CreateInstance<MessageData>();
        copy.friendId = friendId;
        copy.sendTime = sendTime;
        copy.message = message;
        copy.talkId = talkId;
        copy.isUsed = isUsed;
        copy.eventDataList = new List<BaseEventData>();
        if (deckData != null) copy.deckData = deckData.Copy();
        if (eventDataList != null) copy.eventDataList = global::DeepCopy.DeepCopyList(eventDataList);
        return copy;
    }

    public void Init()
    {
        isUsed = false;
        if (eventDataList == null) return;
        foreach(BaseEventData eventData in eventDataList) eventData.Init();
    }

    public bool CanExecuteEvent()
    {
        if (isUsed) 
        {
            if (!GameManager.solM.doSoliloquy) GameManager.solM.SetSoliloquy("このメッセージからはもう何も浮かばないよ、、、").Forget();
            return false;
        }
        else 
        {
            if (eventDataList.Count == 0) 
            {
                if (!GameManager.solM.doSoliloquy) GameManager.solM.SetSoliloquy("何も思いつかないや、、、").Forget();
                return true;
            }
            return true;
        }
    }
}
