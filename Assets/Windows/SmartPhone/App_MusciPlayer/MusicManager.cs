using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MusicManager : BaseAppManager
{
    GameManager gamM;
    AudioManager audM;

    public VisualTreeAsset musicPlayerTree; // タイトルのUIアセット

    VisualElement stageSelector; // ステージ選択のUI要素
    VisualElement gameController; // ゲームコントローラーのUI要素
    VisualElement footer; // フッターのUI要素

    VisualElement backButton; // 戻るボタンのUI要素
    VisualElement forwardButton; // 進むボタンのUI要素
    VisualElement enterButton; // 開始ボタンのUI要素
    VisualElement exitButton; // 終了ボタンのUI要素
    Label stageNameElement; // ステージ名のUI要素
    VisualElement stageImageElement; // ステージ画像のUI要素

    int stageId = 0;
    float angle = 0;

    protected override void InitM()
    {
        this.gamM = GameManager.gamM;
        VisualElement rootMusicElement = musicPlayerTree.Instantiate();
        rootMusicElement.style.height = Length.Percent(100);
        rootAppElement.Add(rootMusicElement);

        // ボタンの設定
        stageSelector = rootMusicElement.Q<VisualElement>("StageSelector");
        backButton = stageSelector.Q<VisualElement>("Back");
        forwardButton = stageSelector.Q<VisualElement>("Forward");
        enterButton = stageSelector.Q<VisualElement>("Enter");

        gameController = rootMusicElement.Q<VisualElement>("GameController");
        exitButton = gameController.Q<VisualElement>("Exit");

        backButton.RegisterCallback<ClickEvent>((e) => { changeStageInfo(--stageId); });
        forwardButton.RegisterCallback<ClickEvent>((e) => { changeStageInfo(++stageId); });
        enterButton.RegisterCallback<ClickEvent>((e) => { gamM.StartGame(stageId); });
        exitButton.RegisterCallback<ClickEvent>((e) => { gamM.Init(); });

        // ステージ名と画像の設定
        VisualElement musicPlayerElement = rootAppElement.Q<VisualElement>("MusicPlayer");
        stageNameElement = musicPlayerElement.Q<Label>("StageName");
        stageImageElement = musicPlayerElement.Q<VisualElement>("MusicThumbnail").Q("StageImage").Q("Icon");
        angle = 0;

        // 音量コントローラの設定
        audM = GameManager.audM;
        VisualElement audioSelector = musicPlayerElement.Q<VisualElement>("AudioSelector");

        Slider BGMSlider = audioSelector.Q<VisualElement>("BGM").Q<Slider>("Slider");
        BGMSlider.RegisterValueChangedCallback(e => { audM.SetBGMVolume(e.newValue); });
        BGMSlider.value = audM.GetBGMVolume() * 100;

        Slider SESlider = audioSelector.Q<VisualElement>("SE").Q<Slider>("Slider");
        SESlider.RegisterValueChangedCallback(e => { audM.SetSEVolume(e.newValue); });
        SESlider.value = audM.GetSEVolume() * 100;

        footer = rootMusicElement.Q<VisualElement>("Footer");
        footer.Q<VisualElement>("Menu2").RegisterCallback<ClickEvent>((e) => 
        {
            audM.PlayNormalSound(NormalSound.select);
            smaM.ChangeApp(smaM.GetAppManager<LineManager>()); 
        });

        VisualElement tutorialElement = rootMusicElement.Q<VisualElement>("Footer").Q<VisualElement>("Menu3");
        tutorialElement.RegisterCallback<ClickEvent>((e) => 
        { 
            audM.PlayNormalSound(NormalSound.select);
            gamM.ShowTutorial(); 
        });

        footer.style.display = DisplayStyle.None;
        stageSelector.style.display = DisplayStyle.Flex;
        gameController.style.display = DisplayStyle.None;
        changeStageInfo(stageId);
    }

    void changeStageInfo(int stageId)
    {
        StageData stageData = gamM.GetStageData(stageId);
        stageNameElement.text = stageData.stageName;
        stageImageElement.style.backgroundImage = stageData.stageImage;

        if (stageId >= gamM.stageDataList.Count - 1) forwardButton.SetEnabled(false);
        else forwardButton.SetEnabled(true);

        if (stageId <= 0) backButton.SetEnabled(false);
        else backButton.SetEnabled(true);
    }

    public override void StartGame(BaseAppData baseAppData = null)
    {
        footer.style.display = DisplayStyle.Flex;
        stageSelector.style.display = DisplayStyle.None;
        gameController.style.display = DisplayStyle.Flex;

        stageImageElement.schedule.Execute(() =>
        {
            angle += 1.0f;
            stageImageElement.style.rotate = new Rotate(new Angle(angle));
        }).Every(100); // 100ミリ秒ごとに実行
    }

    public override void Notification(NotificationData notificationData)
    {
        throw new System.NotImplementedException("MusicManager does not support notification.");
    }
}
