using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using System.Linq;
using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;

public class LineManager : BaseAppManager
{
    AudioManager audM;

    // 各種UIを管理するコンポーネント
    [HideInInspector]
    Dictionary<int, TalkManager> talMDict; // トークマネージャーの辞書
    public int[] talkIdArray { get { return talMDict.Keys.ToArray(); } }
    public int talkCount { get { return talMDict.Count; } }
    [HideInInspector]
    public HistoryManager hisM;
    
    LineAppData lineAppData;
    public FriendData ownData { get { return lineAppData.ownData; } }

    // 各種UIのテンプレート
    public VisualTreeAsset messageTree; // メッセージ画面のUIテンプレート
    public VisualTreeAsset messageListTree; // メッセージのUIテンプレート
    public VisualTreeAsset messageHistoryTree; // メッセージ履歴のUIテンプレート
    public VisualTreeAsset messageHistoryListTree; // メッセージ履歴の要素のUIテンプレート

    int currentTalkId = 0; // 現在表示しているトークId
    BaseAppSceneManager currentSceneM; // 現在表示しているシーンのマネージャー
    List<BaseAppSceneManager> historySceneMList; // ラインに表示された要素のマネージャーのリスト

    protected override void InitM()
    {
        if (hisM != null) Destroy(hisM);
        hisM = this.AddComponent<HistoryManager>();

        audM = GameManager.audM;

        if (talMDict == null) talMDict = new Dictionary<int, TalkManager>();
        else 
        {
            foreach (int key in talMDict.Keys) Destroy(talMDict[key]);
            talMDict.Clear();
        }

        if (historySceneMList == null) historySceneMList = new List<BaseAppSceneManager>();
        else 
        {
            foreach (BaseAppSceneManager sceneManager in historySceneMList) Destroy(sceneManager);
            historySceneMList.Clear();
        }

        currentSceneM = null;
        currentTalkId = 0;
    }

    public async void StartGame(LineAppData lineAppData)
    {
        this.lineAppData = lineAppData;
        lineAppData.Init();
        
        // メッセージリストの初期化(イベントを正しく初期化できるよう、全てのtalkMを生成してから初期化)
        foreach (TalkInformationData tInfoData in lineAppData.tInfoDataList) talMDict[tInfoData.id] = this.AddComponent<TalkManager>();
        foreach (TalkInformationData tInfoData in lineAppData.tInfoDataList) talMDict[tInfoData.id].startGame(tInfoData);

        // 各種Lineに関わるマネージャーの初期化
        hisM.StartGame();

        // シーンを変更
        await ChangeScene(hisM);
    }

    public override void OnStep()
    {
        foreach (int key in talMDict.Keys) talMDict[key].onStep();
        CheckGameEnd();
    }

    public void OnChangeFriendStatus(int friendId)
    {
        foreach (TalkInformationData tInfoDataList in lineAppData.tInfoDataList)
        {
            if (tInfoDataList.joinFriendIdList.Contains(friendId))
            {
                talMDict[tInfoDataList.id].setSendMessage(null, false);
            }
        }
    }

    public void CheckGameEnd()
    {
        foreach (int key in talMDict.Keys) if (!talMDict[key].isTalkEnd()) return;
        GameManager.gamM.Finish().Forget();
    }

    // 一つ前の画面に戻る
    public async Task ChangePreviousScene()
    {
        if (historySceneMList.Count == 0) return;

        currentSceneM.HideScene(rootAppElement);
        historySceneMList.Remove(currentSceneM);

        currentSceneM = historySceneMList.Last();
        if (currentSceneM is TalkManager talkManager) currentTalkId = talkManager.talkId;
        else currentTalkId = -1;

        audM.PlayNormalSound(NormalSound.clicked);
        await currentSceneM.ShowScene(rootAppElement, ChangeType.Back);
        
        GameManager.gamM.AddStep();
    }

    // シーンを変更
    public async UniTask ChangeScene(BaseAppSceneManager sceneM, ChangeType changeType = ChangeType.Enter)
    {
        if (sceneM == null) return;
        
        if(currentSceneM != null) currentSceneM.HideScene(rootAppElement);

        currentSceneM = sceneM;
        if (currentSceneM is TalkManager talkManager) currentTalkId = talkManager.talkId;
        else currentTalkId = -1;

        historySceneMList.Add(currentSceneM);
        audM.PlayNormalSound(NormalSound.clicked);
        await currentSceneM.ShowScene(rootAppElement, changeType);

        if (historySceneMList.Count > 5) historySceneMList.RemoveAt(0);

        if(changeType != ChangeType.Notification) GameManager.gamM.AddStep();
    }

    public TalkManager GetCurrentTalkManager()
    {
        if (currentTalkId == -1) return null; // トーク画面でない場合
        return talMDict[currentTalkId];
    }

    public TalkManager GetTalkManager(int talkId)
    {
        if (!talMDict.Keys.Contains(talkId)) throw new Exception("不正なtalkId。talkId:" + talkId);
        return talMDict[talkId];
    }

    public FriendData GetFriendData(int friendId)
    {
        if (ownData.id == friendId) return ownData;
        foreach (FriendData friendData in lineAppData.friendDataList)
        {
            if (friendData.id == friendId) return friendData;
        }
        throw new Exception("指定されたfriendIdが見つかりません。 friendId: " + friendId);
    }


    public void SetFriendIntimacy(int friendId, int score)
    {
        foreach (FriendData friendData in lineAppData.friendDataList)
        {
            if (friendData.id == friendId)
            {
                math.clamp(friendData.intimacyScore += score, 0, 100);
                return;
            }
        }
        throw new Exception("指定されたfriendIdが見つかりません。 friendId: " + friendId);
    }

    public void SetFriendStatus(int friendId, EFriendStatus status)
    {
        if (friendId == ownData.id)
        {
            Debug.LogError("自分のステータスは変更できません。friendId:" + friendId);
            return;
        }
        foreach (FriendData friendData in lineAppData.friendDataList)
        {
            if (friendData.id == friendId)
            {
                friendData.status = status;
                return;
            }
        }
        throw new Exception("指定されたfriendIdが見つかりません。 friendId: " + friendId);
    }

    public EFriendStatus GetFriendStatus(int friendId)
    {
        if (friendId == ownData.id) throw new Exception("自分のステータスは取得できません。friendId:" + friendId);
        foreach (FriendData friendData in lineAppData.friendDataList)
        {
            if (friendData.id == friendId) return friendData.status;
        }
        throw new Exception("指定されたfriendIdが見つかりません。 friendId: " + friendId);
    }

    public int GetFriendIntimacy(int friendId)
    {
        if (friendId == ownData.id) throw new Exception("自分のステータスは取得できません。friendId:" + friendId);
        foreach (FriendData friendData in lineAppData.friendDataList)
        {
            if (friendData.id == friendId) return friendData.intimacyScore;
        }
        throw new Exception("指定されたfriendIdが見つかりません。 friendId: " + friendId);
    }

    public override async void Notification(NotificationData notificationData)
    {
        if (notificationData.userData is BaseAppSceneManager baseSmartPhoneManager)
        {
            if (currentTalkId != -1) await ChangeScene(hisM, ChangeType.Notification);
            await ChangeScene(baseSmartPhoneManager);
        }
    }

    // ゲーム終了時の処理
    public async UniTask ShowResult()
    {
        TalkInformationData talkInformationData = ScriptableObject.CreateInstance<TalkInformationData>();
        talkInformationData.messageDataList = new List<MessageData>();
        talkInformationData.talkName = "みんなの感想";
        talkInformationData.id = -10;
        talkInformationData.joinFriendIdList = new List<int>();
        foreach (FriendData friendData in lineAppData.friendDataList) talkInformationData.joinFriendIdList.Add(friendData.id);

        TalkManager talkManager = this.AddComponent<TalkManager>();
        talMDict[talkInformationData.id] = talkManager;
        talkManager.startGame(talkInformationData);
        talkManager.setButtonEnabled(false);
        await ChangeScene(talkManager, ChangeType.Enter);
        await UniTask.Delay(1500);

        foreach (FriendData friendData in lineAppData.friendDataList)
        {
            if (friendData.id == ownData.id) continue;
            MessageData messageData = ScriptableObject.CreateInstance<MessageData>();
            messageData.friendId = friendData.id;
            if (friendData.status == EFriendStatus.Success) messageData.message = friendData.successResultMessage;
            else if (friendData.status == EFriendStatus.Block) messageData.message = friendData.blockResultMessage;
            else messageData.message = friendData.progressResultMessage;
            talkManager.addMessage(messageData);
            await UniTask.Delay(2000);
        };
    }

    public LineAppData GetLineAppData()
    {
        return lineAppData.Copy() as LineAppData;
    }
}