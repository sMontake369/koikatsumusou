using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "AddWordData ", menuName = "EventData/AddWordData")]
public class AddWordData : BaseEventData
{
    public WordData wordData;
    public override void Init()
    {
        
    }

    protected override UniTask DoEvent(CancellationToken token)
    {
        GameManager.worM.AddWord(wordData);
        GameManager.solM.SetSoliloquy("ワード「" + wordData.word + "」を思いついた!").Forget();
        return UniTask.CompletedTask;
    }

    public override BaseEventData Copy()
    {
        AddWordData copy = CreateInstance<AddWordData>();
        copy.wordData = wordData.deepCopy();
        return copy;
    }
}
