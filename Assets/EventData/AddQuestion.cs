using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[CreateAssetMenu(fileName = "AddQuestion", menuName = "EventData/AddQuestion")]
public class AddQuestion : BaseEventData
{
    public int talkId;
    public ReplyData questionData;
    public override void init()
    {
        questionData.init();
    }

    protected override UniTask doEvent(CancellationToken token)
    {
        GameManager.smaM.getAppManager<LineManager>().GetTalkManager(talkId).addQuestion(questionData);
        return UniTask.CompletedTask;
    }

    public override BaseEventData deepCopy()
    {
        AddQuestion addQuestion = CreateInstance<AddQuestion>();
        addQuestion.talkId = talkId;
        addQuestion.questionData = questionData.deepCopy();
        return addQuestion;
    }
}
