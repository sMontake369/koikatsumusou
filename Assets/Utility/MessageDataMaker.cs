# if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using NaughtyAttributes;
using UnityEditor;

public class MessageDataMaker : MonoBehaviour {

    public TextAsset MessageDataCSV;
    public TextAsset ReplyDataCSV;
    public string folderName;

    List<string[]> LoadCSV(TextAsset textAsset)
    {
        List<string[]> csvDataList = new List<string[]>(); 
        StringReader reader = new StringReader(textAsset.text);

        while (reader.Peek() != -1)
        {
            string line = reader.ReadLine();
            csvDataList.Add(line.Split(','));
        }
        return csvDataList;
    }

    [Button("MakeMessageData")]
    void MakeMessageData()
    {
        string folderName = this.folderName + "/MessageData";
        if (!AssetDatabase.IsValidFolder("Assets/" + folderName)) AssetDatabase.CreateFolder("Assets/" + this.folderName, "MessageData");

        List<string[]> messageDataList = LoadCSV(MessageDataCSV);

        string groupName = "";
        AddMessageEventData addMessageEventData = null;

        for (int i = 1; i < messageDataList.Count; i++)
        {
            MessageData messageData = ScriptableObject.CreateInstance<MessageData>();
            string fileName = messageDataList[i][0] + "-" + messageDataList[i][1] + "-" + messageDataList[i][2] + "-" + messageDataList[i][3] + "-" + messageDataList[i][4];
            messageData.friendId = int.Parse(messageDataList[i][5]);
            messageData.talkId = int.Parse(messageDataList[i][6]);
            messageData.sendTime = messageDataList[i][7];
            messageData.message = messageDataList[i][8];

            if (groupName != messageDataList[i][3])
            {
                if (addMessageEventData != null) 
                {
                    AssetDatabase.CreateAsset(addMessageEventData, "Assets/" + folderName + "/Message-" + groupName + "/AddMessageEventData-" + groupName + ".asset");
                }
                addMessageEventData = ScriptableObject.CreateInstance<AddMessageEventData>();
                addMessageEventData.messageDataList = new List<MessageData>();

                groupName = messageDataList[i][3];
                if (!AssetDatabase.IsValidFolder("Assets/" + folderName + groupName))
                {
                    Debug.Log("createFolder:" + "Assets/" + folderName + groupName);
                    if (AssetDatabase.CreateFolder("Assets/" + folderName, "Message-" + groupName) == "") throw new System.Exception("フォルダの作成に失敗しました");
                }
            }
            AssetDatabase.CreateAsset(messageData, "Assets/" + folderName + "/Message-" + groupName + "/" + fileName + ".asset");
            addMessageEventData.messageDataList.Add(messageData);
        }
        if (addMessageEventData != null) AssetDatabase.CreateAsset(addMessageEventData, "Assets/" + folderName + "/Message-" + groupName + "/AddMessageEventData-" + groupName + ".asset");
        Debug.Log("MessageData作成完了");
    }

    [Button("MakeReplyData")]
    void MakeReplyData()
    {   
        string folderName = this.folderName + "/ReplyData";
        if (!AssetDatabase.IsValidFolder("Assets/" + folderName)) AssetDatabase.CreateFolder("Assets/" + this.folderName, "ReplyData");

        List<string[]> replyDataList = LoadCSV(ReplyDataCSV);

        for (int i = 1; i < replyDataList.Count; i++)
        {
            string fileName = replyDataList[i][0] + "-" + replyDataList[i][1] + "-" + replyDataList[i][2] + "-" + replyDataList[i][3];
            ReplyData replyData = ScriptableObject.CreateInstance<ReplyData>();
            replyData.requirementDeckId = int.Parse(replyDataList[i][4]);
            AssetDatabase.CreateAsset(replyData, "Assets/" + folderName + "/" + fileName + ".asset");
        }
    }
}
#endif