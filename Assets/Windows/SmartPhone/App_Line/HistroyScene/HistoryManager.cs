using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Linq;
using DG.Tweening;
using Cysharp.Threading.Tasks;

// メッセージ履歴のUIを管理するクラス
public class HistoryManager : BaseAppSceneManager
{
    ListView listElement; // メッセージ履歴の要素

    SmartPhoneManager smaM; // スマホのマネージャー
    LineManager linM; // ラインのマネージャー
    Dictionary<int, MessageData> messageDataDict;// 各友達の最新メッセージリスト

    public void StartGame()
    {
        smaM = GameManager.smaM;
        linM = smaM.GetAppManager<LineManager>();

        // 各友達の最新メッセージを取得
        if(messageDataDict == null) messageDataDict = new Dictionary<int, MessageData>();
        else messageDataDict.Clear();
        foreach (int talkId in linM.talkIdArray) messageDataDict[talkId] = linM.GetTalkManager(talkId).GetMessageDataList().Last();

        rootElement = linM.messageHistoryTree.Instantiate();
        rootElement.style.height = Length.Percent(100);
        listElement = rootElement.Q<ListView>("MessageHistory");

        listElement.makeItem = () => linM.messageHistoryListTree.CloneTree();

        listElement.bindItem += (element, index) =>
        {
            MessageData messageData = messageDataDict[index];
            Button rootElement = element.Q<Button>("MessageHistoryElement");
            rootElement.Q<VisualElement>("UserIcon").style.backgroundImage = linM.GetFriendData(messageData.friendId).icon;
            rootElement.Q<Label>("Time").text = messageData.sendTime;

            VisualElement thumbnailElement = rootElement.Q<VisualElement>("Thumbnail");
            TalkInformationData talkData = linM.GetTalkManager(messageData.talkId).getTalkInfoData();
            rootElement.Q<VisualElement>("UserIcon").style.backgroundImage = talkData.icon;
            thumbnailElement.Q<Label>("UserName").text = talkData.talkName;
            thumbnailElement.Q<Label>("LatestMessage").text = messageData.message;

            rootElement.RegisterCallback<ClickEvent>((e) => Clicked(messageData.talkId));
        };

        listElement.itemsSource = messageDataDict.Values.ToList();

        VisualElement musicElement = rootElement.Q<VisualElement>("Footer").Q<VisualElement>("Menu1");
        musicElement.RegisterCallback<ClickEvent>((e) => 
        { 
            smaM.ChangeApp(smaM.GetAppManager<MusicManager>()); 
            GameManager.audM.PlayNormalSound(NormalSound.select);
        });
        VisualElement tutorialElement = rootElement.Q<VisualElement>("Footer").Q<VisualElement>("Menu3");
        tutorialElement.RegisterCallback<ClickEvent>((e) => 
        {
            GameManager.gamM.ShowTutorial(); 
            GameManager.audM.PlayNormalSound(NormalSound.select);
        });
    }

    protected override void OnBeforeShow()
    {
        UpdateLatestMessage();
    }

    // 最新メッセージを更新
    public void UpdateLatestMessage()
    {
        foreach (int talkId in linM.talkIdArray) messageDataDict[talkId] = linM.GetTalkManager(talkId).GetMessageDataList().Last();
        listElement.Rebuild();
    }

    // クリック時、友達のメッセージリストを表示する処理
    async void Clicked(int talkId)
    {
        await linM.ChangeScene(linM.GetTalkManager(talkId));
    }

    protected override async UniTask Show(VisualElement parentElement, ChangeType changeType)
    {
        // シーンの切り替えアニメーション
        switch (changeType)
        {
            case ChangeType.Enter:
                this.rootElement.style.left = 500;
                parentElement.Add(this.rootElement);
                await DOTween.To(() => 500, (value) => rootElement.style.left = value, 0, 0.5f).SetEase(Ease.OutQuart).AsyncWaitForCompletion();
                break;
            case ChangeType.Back or ChangeType.Notification:
                this.rootElement.style.left = -500;
                parentElement.Add(this.rootElement);
                await DOTween.To(() => -500, (value) => rootElement.style.left = value, 0, 0.5f).SetEase(Ease.OutQuart).AsyncWaitForCompletion();
                break;
        }
    }

    protected override void Hide(VisualElement rootElement)
    {
        rootElement.Remove(this.rootElement);
    }
}
