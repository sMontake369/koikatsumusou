using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using unityroom.Api;

public class ResultManager : BaseWindowManager
{
    GameManager gamM;

    Label blockNumLabel; // ブロック数の要素
    Label successNumLabel; // 成功数の要素
    Label progressNumLabel; // 進行中数の要素
    Label scoreLabel; // スコアの要素
    Label totalScoreLabel; // 合計スコアの要素

    VisualElement retryLabel; // リトライボタンの要素
    VisualElement titleLabel; // タイトルボタンの要素

    public override void Init()
    {
        this.gamM = GameManager.gamM;
        GetComponent<UIDocument>().enabled = true;
        this.rootElement = GetComponent<UIDocument>().rootVisualElement;
        VisualElement resultElement = rootElement.Q<VisualElement>("Result");
        VisualElement evaluationElement = resultElement.Q<VisualElement>("Evaluation");
        VisualElement InformationElement = evaluationElement.Q<VisualElement>("Information");
        blockNumLabel = InformationElement.Q<VisualElement>("BlockNum").Q<Label>("Text");
        blockNumLabel.text = "";
        progressNumLabel = InformationElement.Q<VisualElement>("ProgressNum").Q<Label>("Text");
        progressNumLabel.text = "";
        successNumLabel = InformationElement.Q<VisualElement>("SuccessNum").Q<Label>("Text");
        successNumLabel.text = "";

        scoreLabel = resultElement.Q<VisualElement>("Score").Q<Label>("ScoreText");
        scoreLabel.text = "";
        totalScoreLabel = resultElement.Q<VisualElement>("Score").Q<Label>("TotalScoreText");
        totalScoreLabel.text = "";

        // ボタンの処理
        VisualElement buttonElement = rootElement.Q<VisualElement>("Button");
        retryLabel = buttonElement.Q<Label>("Retry");
        titleLabel = buttonElement.Q<Label>("Title");

        titleLabel.RegisterCallback<ClickEvent>((e) => { OnClickTitle(); });
        retryLabel.RegisterCallback<ClickEvent>((e) => { OnClickRetry(); });
        titleLabel.style.display = DisplayStyle.None;
        retryLabel.style.display = DisplayStyle.None;

        HideWindow();
    }

    public async UniTask ShowResult()
    {
        LineAppData lineAppData = GameManager.smaM.GetAppManager<LineManager>().GetLineAppData();
        AudioManager audM = GameManager.audM;
        int blockNum = 0;
        int progressNum = 0;
        int successNum = 0;
        int intimacyScore = 0;
        
        foreach (var friendData in lineAppData.friendDataList)
        {
            switch (friendData.status)
            {
                case EFriendStatus.Success:
                    intimacyScore += friendData.intimacyScore * 100;
                    successNum++;
                    break;
                case EFriendStatus.Block:
                    intimacyScore += friendData.intimacyScore * 40;
                    blockNum++;
                    break;
                case EFriendStatus.Progress:
                    intimacyScore += friendData.intimacyScore * 70;
                    progressNum++;
                    break;
            }   
        }
        GameManager gamM = GameManager.gamM;
        int maxStep = gamM.GetCurrentStageData().maxStep;
        int timeScore = (maxStep - gamM.GetStep()) * (1000 / math.min(maxStep, 1000));
        ShowWindow();
        await UniTask.Delay(1000);
        blockNumLabel.text = blockNum.ToString() + "人";
        audM.PlayNormalSound(NormalSound.sendMessage);
        await UniTask.Delay(1000);
        progressNumLabel.text = progressNum.ToString() + "人";
        audM.PlayNormalSound(NormalSound.sendMessage);
        await UniTask.Delay(1000);
        successNumLabel.text = successNum.ToString() + "人";
        audM.PlayNormalSound(NormalSound.sendMessage);
        await UniTask.Delay(2000);

        int totalScore = intimacyScore + timeScore;
        scoreLabel.text = "信頼度スコア:" + intimacyScore.ToString("D5") + "\nタイムスコア:" + timeScore.ToString("D5");
        totalScoreLabel.text = "　合計スコア:" + totalScore.ToString("D5");
        audM.PlayNormalSound(NormalSound.sendMessage);
        UnityroomApiClient.Instance.SendScore(1, totalScore, ScoreboardWriteMode.HighScoreAsc);

        titleLabel.style.display = DisplayStyle.Flex;
        retryLabel.style.display = DisplayStyle.Flex;
    }

    void OnClickTitle()
    {
        gamM.Init();
    }

    void OnClickRetry()
    {
        gamM.Retry();
    }
}
