using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Linq;
using DG.Tweening;
using System.Threading;
using Cysharp.Threading.Tasks;
using System;

public class SmartPhoneManager : BaseWindowManager
{
    // 各種UIのテンプレート
    public VisualTreeAsset NotificationTree;
    
    VisualElement screenElement; // 今表示されているスマホの要素
    Label clockElement; // スマホの時計要素
    Label batteryElement; // スマホのバッテリー要素
    AudioManager audM;

    BaseAppManager currentApp; // 現在表示しているシーンのマネージャー
    List<VisualElement> notificationElementList; // 通知の要素のリスト

    List<BaseAppManager> appManagerList;

    public override void Init()
    {
        rootElement = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("SmartPhone");
        VisualElement indicatorElement = rootElement.Q<VisualElement>("Indicator");
        clockElement = indicatorElement.Q<Label>("Clock");
        batteryElement = indicatorElement.Q<Label>("Battery");
        screenElement = rootElement.Q<VisualElement>("Screen");
        screenElement.Clear();

        clockElement.RegisterCallback<ClickEvent>(e => { GameManager.gamM.AddStep(); audM.PlayNormalSound(NormalSound.clicked); });

        currentApp = null;

        appManagerList = new List<BaseAppManager>();
        MusicManager musM = this.GetComponent<MusicManager>();
        appManagerList.Add(musM);
        appManagerList.Add(this.GetComponent<LineManager>());
        foreach (BaseAppManager app in appManagerList) app.Init();

        audM = GameManager.audM;

        notificationElementList = new List<VisualElement>();

        SetTime();
        ChangeApp(musM);
    }

    public void StartGame(List<BaseAppData> appDataList)
    {
        BaseAppManager firstAppManager = null;
        foreach (BaseAppManager appManager in appManagerList)
        {
            switch (appManager)
            {
                case LineManager linM:
                    LineAppData lineAppData = appDataList.FirstOrDefault(appData => appData is LineAppData) as LineAppData;
                    if (lineAppData != null) 
                    {
                        linM.StartGame(lineAppData);
                        firstAppManager = linM;
                    }
                    break;
                case MusicManager musicM:
                    musicM.StartGame();
                    break;
            }
        }
        if (firstAppManager == null) throw new Exception("最初のアプリが見つかりませんでした");
        ChangeApp(firstAppManager);
    }

    public void ChangeApp(BaseAppManager baseAppManager, ChangeType changeType = ChangeType.Enter)
    {
        if (currentApp == baseAppManager) return;
        if (currentApp != null) currentApp.HideApp(screenElement);
        currentApp = baseAppManager;
        currentApp.ShowApp(screenElement, changeType);
    }

    public void ChangeApp(NotificationData notificationData)
    {
        if (currentApp != notificationData.appManager)
        {
            if (currentApp != null) currentApp.HideApp(screenElement);
            currentApp = notificationData.appManager;
            currentApp.ShowApp(screenElement, ChangeType.Notification);
        }
        currentApp.Notification(notificationData);
    }

    public void OnStep()
    {
        SetTime();
        batteryElement.text = (100 - (int)(GameManager.gamM.GetStepRatio() * 100)).ToString() + "%";

        foreach (BaseAppManager app in appManagerList) app.OnStep();
    }

    void SetTime()
    {
        clockElement.text = GameManager.gamM.GetTime();
    }

    public void ShowNotification(NotificationData notificationData)
    {        
        VisualElement notificationElement = NotificationTree.Instantiate().Q<VisualElement>("RootNotification");
        notificationElement.style.position = Position.Absolute;
        notificationElement.dataSource = notificationData;
        screenElement.Add(notificationElement);
        notificationElementList.Add(notificationElement);

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        
        notificationElement.RegisterCallback<ClickEvent>(e => { 
            ChangeApp(notificationData);
            cancellationTokenSource.Cancel();
            foreach (VisualElement element in notificationElementList) // 何か通知が押されたら全ての通知を消す
            {
                DOTween.To(() => 30, (value) => element.style.top = value, -60, 0.5f).SetEase(Ease.OutQuart).OnComplete(() =>
                {
                    if (element?.parent == screenElement) 
                    {
                        screenElement.Remove(element);
                        notificationElementList.Remove(element);
                    }
                });
            }
        });

        audM.PlayNormalSound(NormalSound.notification);
        MoveNotification(notificationElement, cancellationTokenSource).Forget();
    }

    async UniTask MoveNotification(VisualElement notificationElement, CancellationTokenSource cancellationTokenSource)
    {
        var sequence = DOTween.Sequence();
        sequence.Append(DOTween.To(() => -60, (value) => notificationElement.style.top = value, 30, 1.0f).SetEase(Ease.OutQuart))
                .Append(DOTween.To(() => 30, (value) => notificationElement.style.top = value, -60, 1.0f).SetEase(Ease.OutQuart).SetDelay(2.0f).OnComplete(() =>
                { if (notificationElement?.parent == screenElement) 
                {
                    screenElement.Remove(notificationElement);
                    notificationElementList.Remove(notificationElement);
                } 
                })
        );

        try { await sequence.AsyncWaitForCompletion(); }
        catch (Exception) { throw; }
    }

    public T GetAppManager<T>() where T : BaseAppManager
    {
        foreach (BaseAppManager sceneM in appManagerList) if (sceneM is T) return (T)sceneM;
        return null;
    }

    public async UniTask ShowResult()
    {
        foreach (BaseAppManager app in appManagerList)
        {
            if (app is LineManager linM) await linM.ShowResult();
        }
    }
}