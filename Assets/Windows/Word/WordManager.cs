using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class WordManager : BaseWindowManager
{
    WordInfo[] wordInfoArray;
    WordInfo selectedWordInfo;
    LineManager minM;

    public override void Init()
    {
        rootElement = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("WordDeck");
        VisualElement wordElement = rootElement.Q<VisualElement>("WordWrapper");

        if (wordInfoArray == null) 
        {
            wordInfoArray = new WordInfo[3];
            for (int index = 0; index < 3; index++)
            {
                VisualElement childWordElement = wordElement.Q<VisualElement>("Word" + index);
                wordInfoArray[index] = new WordInfo(index, childWordElement);
            }
        }
        else for (int i = 0; i < wordInfoArray.Length; i++) if (wordInfoArray[i].exist) wordInfoArray[i].RemoveWord();

        selectedWordInfo = null;
        HideWindow();
    }

    public override void StartGame()
    {
        minM = GameManager.smaM.GetAppManager<LineManager>();
        ShowWindow();
    }

    public void SelectWord(int index)
    {
        if (!wordInfoArray[index].exist) return;

        if (selectedWordInfo == wordInfoArray[index]) 
        {
            selectedWordInfo.OnDeselected();
            selectedWordInfo = null;
        }
        else
        {
            if (selectedWordInfo != null) selectedWordInfo.OnDeselected();
            wordInfoArray[index].OnSelected();
            selectedWordInfo = wordInfoArray[index];
        }

        TalkManager talM = minM.GetCurrentTalkManager();
        if (talM != null) talM.setSendMessage();
    }

    public WordData GetSelectWordData()
    {
        if (selectedWordInfo == null) return null;
        return selectedWordInfo.wordData;
    }

    public void AddWord(WordData wordData)
    {
        foreach (var word in wordInfoArray)
        {
            if (!word.exist)
            {
                word.SetWord(wordData.deepCopy());
                GameManager.audM.PlayNormalSound(NormalSound.getDeck);
                return;
            }
        }
        if (!GameManager.solM.doSoliloquy) GameManager.solM.SetSoliloquy("頭がいっぱいでワードを覚えられないよ、、、").Forget();
    }

    public void RemoveWord(int wordId)
    {
        foreach (var wordInfo in wordInfoArray)
        {
            if (wordInfo.exist && wordInfo.wordData.id == wordId)
            {
                wordInfo.RemoveWord();
                GameManager.audM.PlayNormalSound(NormalSound.dropDeck);
                return;
            }
        }
        throw new System.ArgumentOutOfRangeException("存在しないwordId。wordId:" + wordId);
    }

    class WordInfo
    {
        WordManager wordManager;
        
        VisualElement rootElement;
        VisualElement iconElement;
        Label wordElement;

        public bool exist { get; private set; }
        public WordData wordData { get; private set; }

        public WordInfo(int index, VisualElement rootElement)
        {
            this.wordManager = GameManager.worM;
            this.rootElement = rootElement;
            this.wordElement = rootElement.Q<Label>("Name");
            this.iconElement = rootElement.Q<VisualElement>("Icon");
            exist = false;

            rootElement.RegisterCallback<ClickEvent>(e => { wordManager.SelectWord(index); });
        }

        public void SetWord(WordData wordData)
        {
            this.wordData = wordData;
            wordElement.text = wordData.word;
            iconElement.style.backgroundImage = wordData.icon;
            exist = true;
            rootElement.AddToClassList("word-button");
        }

        public void RemoveWord()
        {   
            wordData = null;
            wordElement.text = "";
            iconElement.style.backgroundImage = null;
            exist = false;
            rootElement.style.unityBackgroundImageTintColor = new StyleColor(new Color(1, 1, 1, 1));
            rootElement.RemoveFromClassList("word-button");
        }

        public void OnSelected()
        {
            rootElement.style.unityBackgroundImageTintColor = new StyleColor(new Color(1f, 1f, 0.5f, 1f));
        }

        public void OnDeselected()
        {
            rootElement.style.unityBackgroundImageTintColor = new StyleColor(new Color(1, 1, 1, 1));
        }
    }
}
