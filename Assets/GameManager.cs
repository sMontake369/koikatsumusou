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

        init();
    }

    public void init()
    {   
        initDate = DateTime.Now;
        step = 0;
        isGameEnd = false;

        audM.init();
        smaM.init();
        decM.init();
        worM.init();
        solM.init();
        resM.init();
        audM.SetBGM(EBGM.Title);

        titleWindow.enabled = true;
    }

    public void startGame(int stageId)
    {
        currentStageId = stageId;
        StageData stageData = stageDataList[currentStageId].deepCopy();

        smaM.startGame(stageData.applicationDataList);
        decM.startGame(stageData.deckDataList);
        worM.startGame();
        solM.startGame();
        audM.SetBGM(EBGM.Game);
        
        titleWindow.enabled = false;

        if (isTutorial) 
        {
            showTutorial();
            isTutorial = false;
        }
    }

    public void showTutorial()
    {
        tutorialSwiper.Show();
    }

    public void addStep()
    {
        step++;
        if (!isGameEnd) 
        {
            smaM.onStep();
            if (getStepRatio() == 1.0f) finishGame().Forget();
        }
    }

    public async UniTask finishGame()
    {
        isGameEnd = true;
        await UniTask.Delay(1000);
        audM.SetBGM(EBGM.Ending);
        decM.hideWindow();
        worM.hideWindow();
        solM.hideWindow();
        await UniTask.Delay(500);
        await smaM.showResult();

        await resM.showResult();
    }

    public void retryGame()
    {
        init();
        startGame(currentStageId);
    }

    public void nextGame()
    {
        init();
        currentStageId++;
        if (currentStageId < stageDataList.Count) { startGame(currentStageId); }
        else 
        { 
            currentStageId--;
            solM.setSoliloquy("全ステージをクリアした!").Forget(); 
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

    public int getStep()
    {
        return step;
    }

    public string getTime()
    {
        DateTime curDate = initDate + TimeSpan.FromMinutes(step);
        return new string(curDate.Hour + ":" + curDate.Minute.ToString("D2"));
    }

    public float getStepRatio()
    {
        return math.clamp((float)step / stageDataList[currentStageId].maxStep, 0, 1);
    }
}