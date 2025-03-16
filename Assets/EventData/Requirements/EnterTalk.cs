using UnityEngine;

[CreateAssetMenu(fileName = "EnterTalk", menuName = "Requirements/EnterTalk")]
public class EnterTalk : BaseRequirementData
{
    public int talkId;
    LineManager linM;

    public override BaseRequirementData Copy()
    {
        EnterTalk copy = CreateInstance<EnterTalk>();
        copy.talkId = talkId;
        return copy;
    }

    public override void Init()
    {
        linM = GameManager.smaM.GetAppManager<LineManager>();
    }

    public override bool IsRequirement()
    {
        TalkManager talkManager = linM.GetCurrentTalkManager();
        return talkManager != null && talkManager.talkId == talkId;
    }
}
