using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "AddWordData ", menuName = "EventData/AddWordData")]
public class AddWordData : BaseEventData
{
    public WordData wordData;
    public override void init()
    {
        
    }

    protected override UniTask doEvent(CancellationToken token)
    {
        GameManager.worM.addWord(wordData);
        GameManager.solM.setSoliloquy("ワード「" + wordData.word + "」を思いついた!").Forget();
        return UniTask.CompletedTask;
    }

    public override BaseEventData deepCopy()
    {
        AddWordData copy = CreateInstance<AddWordData>();
        copy.wordData = wordData.deepCopy();
        return copy;
    }
}
