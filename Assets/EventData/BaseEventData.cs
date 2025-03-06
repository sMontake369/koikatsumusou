using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[System.Serializable]
public abstract class BaseEventData : ScriptableObject
{
    public abstract void init();
    protected abstract UniTask doEvent(CancellationToken token);

    public async UniTask execute(CancellationToken token)
    {
        await doEvent(token);
    }

    public virtual void next()
    {
        
    }

    public abstract BaseEventData deepCopy();
}
