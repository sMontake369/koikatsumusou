using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class DeckManager : BaseWindowManager
{
    LineManager linM;
    AudioManager audM;
    public VisualTreeAsset deckAsset; // 選択肢のプレハブ
    ScrollAutoFit scrollView; // 選択肢リストのスクロールビュー
    VisualElement selectedElement; // 選択された選択肢

    List<ConversationDeckData> deckDataList; // 選択肢データ
    public ConversationDeckData selectedDeckData { get; private set; } // 選択された選択肢データ

    public override void init()
    {
        rootElement = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("ChoicesWindow");
        scrollView = rootElement.Q<ScrollAutoFit>();
        this.deckDataList = new List<ConversationDeckData>();
        scrollView.contentContainer.Clear();
        audM = GameManager.audM;

        hideWindow();
    }

    public void startGame(List<ConversationDeckData> newDeckDataList)
    {
        linM = GameManager.smaM.getAppManager<LineManager>();
        this.deckDataList.AddRange(newDeckDataList);

        foreach(ConversationDeckData deckData in deckDataList)
        {
            VisualElement deckElement = deckAsset.Instantiate();
            deckElement.dataSource = deckData;
            scrollView.contentContainer.Add(deckElement);
            deckElement.RegisterCallback<ClickEvent>(evt => { selectDeck(deckElement, deckData); });
        }

        scrollView.UpdateItemWidth();
        showWindow();
        scrollView.ForceUpdate();
    }

    public void selectDeck(VisualElement deckElement, ConversationDeckData conversationDeckData)
    {
        if (deckElement == selectedElement) 
        {
            resetDeck();
            audM.PlayNormalSound(NormalSound.unSelect);
            return;
        }
        if (selectedElement != null) selectedElement.style.backgroundColor = new StyleColor(new Color(0f, 0f, 0f, 0f));

        selectedElement = deckElement;
        selectedElement.style.backgroundColor = new StyleColor(new Color(1f, 0.92f, 0.345f, 1f));
        selectedElement.style.borderTopLeftRadius = 10;
        selectedElement.style.borderTopRightRadius = 10;
        deckElement.style.borderBottomLeftRadius = 10;
        deckElement.style.borderBottomRightRadius = 10;
        selectedDeckData = conversationDeckData;

        TalkManager talkManager = linM.getCurrentTalkManager();
        if (talkManager != null) talkManager.setSendMessage();
        audM.PlayNormalSound(NormalSound.select);
    }

    public void resetDeck()
    {
        selectedElement.style.backgroundColor = new StyleColor(new Color(0f, 0f, 0f, 0f));
        selectedElement = null;
        selectedDeckData = null;

        TalkManager talkManager = linM.getCurrentTalkManager();
        if (talkManager != null) talkManager.setSendMessage(null, false);
        return;
    }

    public void addDeck(ConversationDeckData deckData)
    {
        deckDataList.Add(deckData);

        VisualElement deckElement = deckAsset.Instantiate();
        deckElement.dataSource = deckData;
        scrollView.contentContainer.Add(deckElement);
        scrollView.UpdateItemWidth();
        deckElement.RegisterCallback<ClickEvent>(evt => { selectDeck(deckElement, deckData); });
        audM.PlayNormalSound(NormalSound.getDeck);

        scrollView.schedule.Execute (() => { // 100ms後に実行
            scrollView.ForceUpdate();
            scrollView.ScrollTo(deckElement);
        }).StartingIn(100);
    }

    public void removeDeck(int deckId)
    {
        ConversationDeckData removeDeckData = deckDataList.Find(deckData => deckData.id == deckId);
        if (removeDeckData == selectedDeckData) resetDeck();
        if (removeDeckData != null) 
        {
            deckDataList.Remove(removeDeckData);
            scrollView.schedule.Execute (() => { // 100ms後に実行
                scrollView.ForceUpdate();
            }).StartingIn(100);
        }
    }

    public ConversationDeckData getSelectDeckData()
    {
        return selectedDeckData;
    }
}
