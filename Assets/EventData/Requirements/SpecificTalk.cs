using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "SpecificTalkId ", menuName = "Requirements/SpecificTalkId")]
public class SpecificTalk : BaseRequirementData
{
    [Label("検索するトークID")]
    public int talkId;
    [Label("デッキID")]
    public int deckId;

    public override void init()
    {
        
    }

    public override bool isRequirement()
    {
        TalkManager talM = GameManager.smaM.getAppManager<LineManager>().GetTalkManager(talkId);
        foreach (MessageData message in talM.GetMessageDataList())
        {
            if (message.deckData != null && message.deckData.id == deckId) 
            {
                return true;
            }
        }
        return false;
    }

    public override BaseRequirementData deepCopy()
    {
        SpecificTalk copy = CreateInstance<SpecificTalk>();
        copy.talkId = talkId;
        copy.deckId = deckId;
        return copy;
    }
}
