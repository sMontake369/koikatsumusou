using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[CreateAssetMenu(fileName = "AddQuestion", menuName = "EventData/AddQuestion")]
public class AddQuestion : BaseEventData
{
    public int talkId;
    public ReplyData questionData;
    public override void Init()
    {
        questionData.Init();
    }

    protected override UniTask DoEvent(CancellationToken token)
    {
        GameManager.smaM.GetAppManager<LineManager>().GetTalkManager(talkId).addQuestion(questionData);
        return UniTask.CompletedTask;
    }

    public override BaseEventData Copy()
    {
        AddQuestion addQuestion = CreateInstance<AddQuestion>();
        addQuestion.talkId = talkId;
        addQuestion.questionData = questionData.Copy();
        return addQuestion;
    }
}
