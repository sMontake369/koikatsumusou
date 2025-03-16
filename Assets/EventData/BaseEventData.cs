using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[System.Serializable]
public abstract class BaseEventData : ScriptableObject
{
    public abstract void Init();
    protected abstract UniTask DoEvent(CancellationToken token);

    public async UniTask Execute(CancellationToken token)
    {
        await DoEvent(token);
    }

    public virtual void DoNext()
    {
        
    }

    public abstract BaseEventData Copy();
}
