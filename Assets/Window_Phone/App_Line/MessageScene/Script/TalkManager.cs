using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine.U2D.IK;
using System;
using System.Threading;

// 友達1人orグループのメッセージリストを管理するクラス
public class TalkManager : baseAppSceneManager
{
    SmartPhoneManager smaM;
    DeckManager decM;
    WordManager worM;
    LineManager linM;
    AudioManager audM;
    SoliloquyManager solM;
    HistoryManager hisM;

    ListView listElement; // メッセージリストの要素
    ScrollView scrollView; // メッセージリストのスクロールビュー
    float currentMaxScrollY = 0.0f; // スクロールビューのスクロール幅

    public int talkId { get; private set; } // このメッセージのグループId

    TalkInformationData tInfoData; // トークのデータ
    List<int> sentDeckIdList; // 送信したデッキidのリスト
    Label sendMessageElement; // メッセージ入力欄の要素
    VisualElement sendButtonElement; // 送信ボタンの要素
    VisualElement backButtonElement; // 戻るボタンの要素
    bool isExecuteEvent = false; // イベントを実行中かどうか

    EventCallback<ClickEvent> backButtonCallback;
    CancellationTokenSource cts;
    BaseEventData executingEvent = null;

    public void startGame(TalkInformationData newTInfoData)
    {
        smaM = GameManager.smaM;
        decM = GameManager.decM;
        this.talkId = newTInfoData.id;
        this.tInfoData = newTInfoData;
        sentDeckIdList = new List<int>();
        linM = smaM.getAppManager<LineManager>();
        audM = GameManager.audM;
        worM = GameManager.worM;
        solM = GameManager.solM;
        hisM = linM.hisM;

        // メッセージリストの初期化
        rootElement = linM.messageTree.Instantiate();
        rootElement.style.height = Length.Percent(100);
        listElement = rootElement.Q<ListView>("MessageList");
        scrollView = listElement.Q<ScrollView>();

        listElement.makeItem = () => linM.messageListTree.CloneTree();

        listElement.bindItem += (element, index) =>
        {
            MessageData messageData = tInfoData.messageDataList[index];
            FriendData friendData = linM.GetFriendData(messageData.friendId);
            Button rootElement = element.Q<Button>("MessageElement");
            VisualElement iconElement = rootElement.Q<VisualElement>("UserIcon");
            iconElement.style.backgroundImage = friendData.icon;

            VisualElement thumbnail = rootElement.Q<VisualElement>("Thumbnail");
            Label userName = thumbnail.Q<Label>("UserName");
            userName.text = friendData.userName;

            MessageElement messageInfo = thumbnail.Q<MessageElement>("MessageInfo");
            messageInfo.message = messageData.message;
            messageInfo.time = messageData.sendTime;
            
            messageInfo.UpdateContentAndFontSize();

            if (messageData.friendId == linM.ownData.id) 
            {
                userName.style.display = DisplayStyle.None;
                iconElement.style.display = DisplayStyle.None;
                userName.style.unityTextAlign = TextAnchor.MiddleRight;
                messageInfo.style.flexDirection = FlexDirection.RowReverse;
                rootElement.style.flexDirection = FlexDirection.RowReverse;
                messageInfo.messageBColor = new Color(0.7f, 1f, 0.5f, 1f);
            }
            else 
            {
                userName.style.display = DisplayStyle.Flex;
                iconElement.style.display = DisplayStyle.Flex;
                userName.style.unityTextAlign = TextAnchor.MiddleLeft;
                messageInfo.style.flexDirection = FlexDirection.Row;
                rootElement.style.flexDirection = FlexDirection.Row;
                messageInfo.messageBColor = new Color(1, 1, 1, 1);
            }

            rootElement.RegisterCallback<ClickEvent>((e) => { 
                if (!GameManager.gamM.isGameEnd && !isExecuteEvent && !isTalkEnd()) 
                {
                    if(messageData.canExecuteEvent())
                    {
                        messageData.isUsed = true;
                        audM.PlayNormalSound(NormalSound.clickMessage);
                        executeEvent(messageData.eventDataList).Forget();
                        GameManager.gamM.addStep();
                    }
                }
            });
        };

        listElement.itemsSource = tInfoData.messageDataList;

        // サムネイルを書き換える
        rootElement.Q("Header").Q<Label>("ThumbnailName").text = tInfoData.talkName;

        // 戻るボタンの処理
        backButtonElement = rootElement.Q<VisualElement>("Header").Q<VisualElement>("Return");
        setButtonEnabled(true);

        //送信ボタンの処理
        VisualElement footer = rootElement.Q<VisualElement>("Footer");
        sendMessageElement = footer.Q<Label>("SendMessage");
        sendButtonElement = footer.Q<VisualElement>("SendButton");

        sendMessageElement.RegisterCallback<ClickEvent>(e => { 
            if (isExecuteEvent) eventSkip();
        });

        sendButtonElement.RegisterCallback<ClickEvent>(e => { 
            sendMessage(); 
        });

        if (cts != null) cts.Cancel();
        
        setSendMessage(null); //送信メッセージを初期化
    }

    public async void setSendMessage(string message = null, bool doSound = true)
    {
        if (GameManager.gamM.isGameEnd)
        {
            sendButtonElement.style.display = DisplayStyle.None;
            message = "　";
        }
        else if (isTalkEnd())
        {
            sendButtonElement.style.display = DisplayStyle.None;
            message = "ここのトークは終了しています";
        }
        else if (isExecuteEvent)
        {
            sendButtonElement.style.unityBackgroundImageTintColor = new Color(0, 0, 0, 0.5f);
            message = "ここをクリックで時間を1分進める";
        }
        else sendButtonElement.style.unityBackgroundImageTintColor = new Color(0, 0, 0, 1f);

        if (message == null)
        {
            ConversationDeckData selectedDeckData = decM.getSelectDeckData();
            if (selectedDeckData != null) 
            {
                if (selectedDeckData is DeckWithWordData deckWithWordData)
                {
                    // デッキがワード付きの時
                    WordData wordData = worM.getSelectWordData();
                    deckWithWordData.wordData = wordData ?? null;
                    message = deckWithWordData.getMessageText();
                }
                // デッキが普通で選択されている時
                else message = selectedDeckData.getMessageText();
            }
            // ワードが選択されていない時
            else message = "会話デッキからメッセージを選択…";
        }
        sendMessageElement.text = message;

        if (linM.getCurrentTalkManager() == this && doSound)
        {
            audM.PlayNormalSound(NormalSound.typing);
            await UniTask.Delay(200);
            audM.PlayNormalSound(NormalSound.typing);
            await UniTask.Delay(200);
            audM.PlayNormalSound(NormalSound.typing);
            await UniTask.Delay(200);
            audM.PlayNormalSound(NormalSound.typing);
            await UniTask.Delay(400);
            audM.PlayNormalSound(NormalSound.typing);
        }
    }

    void sendMessage()
    {
        // イベント実行中は送信できない
        if (isExecuteEvent) return;

        if (isTalkEnd()) 
        {
            GameManager.solM.setSoliloquy("ここのトークはもうできないみたい").Forget();
            return;
        }

        ConversationDeckData selectedDeckData = decM.getSelectDeckData();

        // デッキが選択されていない場合
        if (selectedDeckData == null) 
        {
            if (!solM.doSoliloquy) solM.setSoliloquy("送信するメッセージを考えないと、、、").Forget();
            return;
        }

        // すでに送信したデッキの場合
        if(sentDeckIdList.Contains(selectedDeckData.id))
        {
            if (!solM.doSoliloquy) GameManager.solM.setSoliloquy("そのメッセージはもう送ったよ").Forget();
            return;
        }
        
        MessageData messageData = ScriptableObject.CreateInstance<MessageData>();
        if (selectedDeckData is DeckWithWordData deckWithWordData) 
        {
            // デッキデータに単語データが必要な場合
            WordManager wordManager = GameManager.worM;
            WordData wordData = wordManager.getSelectWordData();
            if (wordData == null) 
            {
                if (!solM.doSoliloquy) GameManager.solM.setSoliloquy("メッセージに入れる単語を考えないと、、、").Forget();
                return;
            }

            deckWithWordData.wordData = wordData;
            selectedDeckData = deckWithWordData.deepCopy();
            messageData.deckData = deckWithWordData;
            messageData.message = deckWithWordData.getMessageText();

            wordManager.removeWord(wordData.id);
        }
        else 
        {
            // デッキデータに単語データが不必要な場合
            messageData.deckData = selectedDeckData;
            messageData.message = selectedDeckData.getMessageText();
        }

        if (selectedDeckData.sendableTalkIdList.Any(id => id != talkId) && !solM.doSoliloquy)
        {
            GameManager.solM.setSoliloquy("この文章はここのトークと関係なさそう、、、").Forget();
            return;
        }

        messageData.friendId = linM.ownData.id;
        messageData.sendTime = GameManager.gamM.getTime();

        addMessage(messageData);
        sentDeckIdList.Add(selectedDeckData.id);

        // reset
        setSendMessage("会話デッキからメッセージを選択…", false);
        GameManager.decM.resetDeck();

        listElement.schedule.Execute (() => { // 100ms後に実行
            listElement.ScrollToItem(tInfoData.messageDataList.Count - 1);
        }).StartingIn(100);

        onAdditionMessage();
        GameManager.gamM.addStep();

        sendReplyMessage(selectedDeckData);
    }

    public void addMessage(MessageData messageData)
    {
        if (!canReceiveMessage(messageData)) return;

        MessageData sendMessageData = messageData.deepCopy();
        sendMessageData.init();
        if (sendMessageData.sendTime == null) sendMessageData.sendTime = GameManager.gamM.getTime();
        else if (sendMessageData.sendTime.Length == 0) sendMessageData.sendTime = GameManager.gamM.getTime();
        sendMessageData.talkId = talkId;
        tInfoData.messageDataList.Add(sendMessageData);

        // このトークが表示されていない場合は通知を表示
        if (linM.getCurrentTalkManager() != this)
        {
            NotificationData notificationData = new NotificationData(sendMessageData, linM);
            notificationData.userData = this;
            smaM.showNotification(notificationData);
        }
        else audM.PlayNormalSound(NormalSound.receiveMessage);
        onAdditionMessage();
    }

    public void removeMessage(MessageData messageData)
    {
        MessageData removeData = tInfoData.messageDataList.Find(data => data.message == messageData.message);
        tInfoData.messageDataList.Remove(removeData);
        listElement.Rebuild();
    }

    public bool canReceiveMessage(MessageData messageData)
    {
        // 例外的にゲームが終了している場合は受け取る
        if (GameManager.gamM.isGameEnd) return true;

        // トークが終了している場合は受け取れない
        if (isTalkEnd()) return false;

        // 自分のメッセージはok
        if (messageData.friendId == linM.ownData.id) return true;

        // ブロックされている友達からのメッセージは受け取れない
        if (linM.getFriendStatus(messageData.friendId) == EFriendStatus.Block) return false;

        // 参加していない友達からのメッセージは受け取れない
        if (tInfoData.joinFriendIdList.Any(friendId => friendId != messageData.friendId))
        {
            Debug.Log("この友達はグループに属していない" + messageData.friendId + ":" + messageData.message);
            Debug.Log("参加している友達リスト" + string.Join(",", tInfoData.joinFriendIdList));
            return false;
        }

        return true;
    }

    void onAdditionMessage()
    {
        hisM.updateLatestMessage();
        
        listElement.Rebuild();
        listElement.schedule.Execute (() => { // 100ms後に実行
            float prevMaxScrollY = currentMaxScrollY;
            currentMaxScrollY = scrollView.contentContainer.resolvedStyle.height - scrollView.resolvedStyle.height;
            float curScrollY = scrollView.scrollOffset.y;
            if (Mathf.Clamp01(curScrollY / prevMaxScrollY) > 0.7f)
            {
                listElement.ScrollToItem(tInfoData.messageDataList.Count - 1);
            }
        }).StartingIn(200);
    }

    public async void onStep() //ステップが進むたびに呼ばれる処理
    {
        await checkEvent();
    }

    async UniTask checkEvent() //イベントをチェックする
    {
        if (isExecuteEvent) return;
        
        foreach (EventData eventData in tInfoData.eventDataList) 
        {
            if (eventData.isRequirements())
            {
                eventData.isUsed = true;
                await executeEvent(eventData.eventDataList);
            } 
        }
    }

    // 送信されたメッセージを元に返信する
    public void sendReplyMessage(ConversationDeckData deckData)
    {
        if (tInfoData.questionData != null) //質問データが存在する場合、そちらを優先
        {
            if (tInfoData.questionData.requirementDeckId == deckData.id)
            {
                if (deckData is DeckWithWordData deckWithWordData && tInfoData.questionData is ReplyWithWordData questionWithWordData)
                {
                    WordData requireWordData = questionWithWordData.wordData;
                    if (deckWithWordData.wordData.id == requireWordData.id) executeEvent(questionWithWordData.eventDataList).Forget(); //単語が一緒の場合
                    else if (deckWithWordData.wordData.type == requireWordData.type) executeEvent(questionWithWordData.diffWordEventDataList).Forget(); //タイプが異なっている場合
                    else executeEvent(questionWithWordData.diffAllEventDataList).Forget(); //単語とタイプが異なっている場合
                    tInfoData.questionData = null; //質問データを削除
                }
                else 
                {
                    executeEvent(tInfoData.questionData.eventDataList).Forget(); //ワードなし返答データが存在する場合
                    tInfoData.questionData = null; //質問データを削除
                }
            }
            else 
            {
                executeEvent(tInfoData.defaultQuestionReply).Forget(); //返答データが存在しない場合
                sentDeckIdList.Remove(deckData.id);
            }

            return;
        }

        ReplyData replyData = tInfoData.replyDataList.FirstOrDefault(replyData => replyData.requirementDeckId == deckData.id);
        if (replyData != null)
        {
            if (deckData is DeckWithWordData deckWithWordData && replyData is ReplyWithWordData replyWithWordData) // 単語データが必要な場合
            {
                WordData needWordData = replyWithWordData.wordData; // 正しい単語データ
                if (deckWithWordData.GetWordData().id == needWordData.id) executeEvent(replyWithWordData.eventDataList).Forget(); // 単語が一緒の場合
                else if (deckWithWordData.GetWordData().type == needWordData.type) executeEvent(replyWithWordData.diffWordEventDataList).Forget(); // タイプが一緒で単語が異なっている場合
                else executeEvent(replyWithWordData.diffAllEventDataList).Forget(); // 単語とタイプが異なっている場合
            }
            else executeEvent(replyData.eventDataList).Forget(); // ワードなし返答データが存在する場合
        }
        else executeEvent(tInfoData.defaultReply).Forget(); // 返答データが存在しない場合、デフォルト返答を実行
    }

    public async UniTask executeEvent(List<BaseEventData> eventList)
    {
        if (eventList == null) return;
        if (isExecuteEvent) return;
        isExecuteEvent = true;
        cts = new CancellationTokenSource();
        setSendMessage(null, false);

        try {
            foreach (BaseEventData eventData in eventList) 
            {
                executingEvent = eventData;
                await executingEvent.execute(cts.Token);
                cts.Token.ThrowIfCancellationRequested();
            }
        }
        catch (Exception) {}
        
        isExecuteEvent = false;
        setSendMessage(null, false);
        checkEvent().Forget();
    }

    void eventSkip()
    {
        executingEvent.next();
    }

    public List<MessageData> GetMessageDataList()
    {
        return DeepCopy.DeepCopyList(tInfoData.messageDataList);
    }

    public void addEvent(EventData eventData)
    {
        eventData.init();
        tInfoData.eventDataList.Add(eventData);
    }

    public bool removeEvent(int id)
    {
        for (int i = 0; i < tInfoData.eventDataList.Count; i++)
        {
            if (tInfoData.eventDataList[i].id == id)
            {
                tInfoData.eventDataList.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    public void addQuestion(ReplyData questionData)
    {
        tInfoData.questionData = questionData;
    }

    public void removeQuestion()
    {
        tInfoData.questionData = null;
    }

    protected override void onBeforeShow()
    {
        setSendMessage(null, false);
        listElement.ScrollToItem(tInfoData.messageDataList.Count - 1);
    }

    public void setButtonEnabled(bool isEnabled)
    {
        if (isEnabled)
        {
            backButtonElement.style.unityBackgroundImageTintColor = new Color(0, 0, 0, 1);
            backButtonCallback = async (e) => { await linM.changePreviousScene(); };
            backButtonElement.RegisterCallback(backButtonCallback);
        }
        else
        {
            backButtonElement.style.unityBackgroundImageTintColor = new Color(0, 0, 0, 0.5f);
            backButtonElement.UnregisterCallback(backButtonCallback);
            backButtonCallback = null;
        }
    }

    // このグループのトークが終了しているか
    public bool isTalkEnd()
    {
        foreach (int friendId in tInfoData.joinFriendIdList)
        {   
            if (friendId == linM.ownData.id) continue;
            if (linM.getFriendStatus(friendId) == EFriendStatus.Progress) return false; 
        }
        return true;
    }

    protected override async UniTask showScene(VisualElement parentElement, ChangeType changeType)
    {
        // シーンの切り替えアニメーション
        this.rootElement.style.left = 500;
        parentElement.Add(rootElement);
        await DOTween.To(() => 500, (value) => rootElement.style.left = value, 0, 0.5f).SetEase(Ease.OutQuart).AsyncWaitForCompletion();
    }

    public TalkInformationData getTalkInfoData()
    {
        return tInfoData.deepCopy();
    }
}
