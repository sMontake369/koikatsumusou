using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    [Label("ステージデータリスト")]
    public List<StageData> stageDataList;

    public static GameManager gamM { get; private set; }
    public static DeckManager decM { get; private set; }
    public static SmartPhoneManager smaM { get; private set; }
    public static WordManager worM { get; private set; }
    public static SoliloquyManager solM { get; private set; }
    public static AudioManager audM { get; private set; }
    public static ResultManager resM { get; private set; }
    public GameObject tutorialObj;
    bool isTutorial = true;
    
    Swiper tutorialSwiper;

    public GameObject titleObj;
    UIDocument titleWindow;

    int step = 0;// ゲームの進行ステップ
    DateTime initDate;

    int currentStageId = 0; // 現在のステージID
    public bool isGameEnd { get; private set; } = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gamM = this;
        decM = FindAnyObjectByType<DeckManager>();
        smaM = FindAnyObjectByType<SmartPhoneManager>();
        worM = FindAnyObjectByType<WordManager>();
        solM = FindAnyObjectByType<SoliloquyManager>();
        audM = FindAnyObjectByType<AudioManager>();

        resM = FindAnyObjectByType<ResultManager>();

        UIDocument uIDocument = tutorialObj.GetComponent<UIDocument>();
        titleWindow = titleObj.GetComponent<UIDocument>();

        uIDocument.enabled = true;
        tutorialSwiper = uIDocument.rootVisualElement.Q<Swiper>("Swiper");
        tutorialSwiper.OnSwipe += () => audM.PlayNormalSound(NormalSound.clicked);
        tutorialSwiper.Hide();

        Init();
    }

    public void Init()
    {   
        initDate = DateTime.Now;
        step = 0;
        isGameEnd = true;

        audM.Init();
        smaM.Init();
        decM.Init();
        worM.Init();
        solM.Init();
        resM.Init();
        audM.SetBGM(EBGM.Title);

        titleWindow.enabled = true;
    }

    public void StartGame(int stageId)
    {
        currentStageId = stageId;
        StageData stageData = stageDataList[currentStageId].Copy();

        smaM.StartGame(stageData.applicationDataList);
        decM.StartGame(stageData.deckDataList);
        worM.StartGame();
        solM.StartGame();
        audM.SetBGM(EBGM.Game);
        
        isGameEnd = false;
        titleWindow.enabled = false;

        if (isTutorial) 
        {
            ShowTutorial();
            isTutorial = false;
        }
    }

    public void ShowTutorial()
    {
        tutorialSwiper.Show();
    }

    public void AddStep()
    {
        if (!isGameEnd) 
        {
            step++;
            smaM.OnStep();
            if (GetStepRatio() == 1.0f) Finish().Forget();
        }
    }

    public async UniTask Finish()
    {
        isGameEnd = true;
        await UniTask.Delay(1000);
        audM.SetBGM(EBGM.Ending);
        decM.HideWindow();
        worM.HideWindow();
        solM.HideWindow();
        await UniTask.Delay(500);
        await smaM.ShowResult();

        await resM.ShowResult();
    }

    public void Retry()
    {
        Init();
        StartGame(currentStageId);
    }

    public void Next()
    {
        Init();
        currentStageId++;
        if (currentStageId < stageDataList.Count) { StartGame(currentStageId); }
        else 
        { 
            currentStageId--;
            solM.SetSoliloquy("全ステージをクリアした!").Forget(); 
        }
    }

    public StageData GetStageData(int stageId)
    {
        if (stageId < 0 || stageId >= stageDataList.Count) { throw new System.ArgumentOutOfRangeException("存在しないステージid"); }
        return stageDataList[stageId];
    }

    public StageData GetCurrentStageData()
    {
        return GetStageData(currentStageId);
    }

    public int GetStep()
    {
        return step;
    }

    public string GetTime()
    {
        DateTime curDate = initDate + TimeSpan.FromMinutes(step);
        return new string(curDate.Hour + ":" + curDate.Minute.ToString("D2"));
    }

    public float GetStepRatio()
    {
        return math.clamp((float)step / stageDataList[currentStageId].maxStep, 0, 1);
    }
}