using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using NaughtyAttributes;
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

    public override void init()
    {
        rootElement = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("SmartPhone");
        VisualElement indicatorElement = rootElement.Q<VisualElement>("Indicator");
        clockElement = indicatorElement.Q<Label>("Clock");
        batteryElement = indicatorElement.Q<Label>("Battery");
        screenElement = rootElement.Q<VisualElement>("Screen");
        screenElement.Clear();

        currentApp = null;

        appManagerList = new List<BaseAppManager>();
        MusicManager musM = this.GetComponent<MusicManager>();
        appManagerList.Add(musM);
        appManagerList.Add(this.GetComponent<LineManager>());
        foreach (BaseAppManager app in appManagerList) app.init();

        audM = GameManager.audM;

        notificationElementList = new List<VisualElement>();

        setTime();
        changeApp(musM);
    }

    public void startGame(List<BaseAppData> appDataList)
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
                        linM.startGame(lineAppData);
                        firstAppManager = linM;
                    }
                    break;
                case MusicManager musicM:
                    musicM.startGame();
                    break;
            }
        }
        if (firstAppManager == null) throw new Exception("最初のアプリが見つかりませんでした");
        changeApp(firstAppManager);
    }

    public void changeApp(BaseAppManager baseAppManager, ChangeType changeType = ChangeType.Enter)
    {
        if (currentApp == baseAppManager) return;
        if (currentApp != null) currentApp.closeApp(screenElement);
        currentApp = baseAppManager;
        currentApp.openApp(screenElement, changeType);
    }

    public void changeApp(NotificationData notificationData)
    {
        if (currentApp != notificationData.appManager)
        {
            if (currentApp != null) currentApp.closeApp(screenElement);
            currentApp = notificationData.appManager;
            currentApp.openApp(screenElement, ChangeType.Notification);
        }
        currentApp.notification(notificationData);
    }

    public void onStep()
    {
        setTime();
        batteryElement.text = (100 - (int)(GameManager.gamM.getStepRatio() * 100)).ToString() + "%";

        foreach (BaseAppManager app in appManagerList) app.onStep();
    }

    void setTime()
    {
        clockElement.text = GameManager.gamM.getTime();
    }

    // TODO リストを持たせ、順番に通知。表示中の通知を全て消せるようにする
    public void showNotification(NotificationData notificationData)
    {        
        VisualElement notificationElement = NotificationTree.Instantiate().Q<VisualElement>("RootNotification");
        notificationElement.style.position = Position.Absolute;
        notificationElement.dataSource = notificationData;
        screenElement.Add(notificationElement);
        notificationElementList.Add(notificationElement);

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        
        notificationElement.RegisterCallback<ClickEvent>(e => { 
            changeApp(notificationData);
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
        moveNotification(notificationElement, cancellationTokenSource).Forget();
    }

    async UniTask moveNotification(VisualElement notificationElement, CancellationTokenSource cancellationTokenSource)
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

    public T getAppManager<T>() where T : BaseAppManager
    {
        foreach (BaseAppManager sceneM in appManagerList) if (sceneM is T) return (T)sceneM;
        return null;
    }

    public async UniTask showResult()
    {
        foreach (BaseAppManager app in appManagerList)
        {
            if (app is LineManager linM) await linM.showResult();
        }
    }
}