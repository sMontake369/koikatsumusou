using UnityEngine;

[CreateAssetMenu(fileName = "EnterTalk", menuName = "Requirements/EnterTalk")]
public class EnterTalk : BaseRequirementData
{
    public int talkId;
    LineManager linM;

    public override BaseRequirementData deepCopy()
    {
        EnterTalk copy = CreateInstance<EnterTalk>();
        copy.talkId = talkId;
        return copy;
    }

    public override void init()
    {
        linM = GameManager.smaM.getAppManager<LineManager>();
    }

    public override bool isRequirement()
    {
        TalkManager talkManager = linM.getCurrentTalkManager();
        return talkManager != null && talkManager.talkId == talkId;
    }
}
