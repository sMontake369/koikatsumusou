using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class baseAppSceneManager : MonoBehaviour
{
    public VisualElement rootElement { get; protected set; }

    public async Task openScene(VisualElement rootElement, ChangeType changeType)
    {
        onBeforeShow();
        await showScene(rootElement, changeType);
        onAfterShow();
    }
    protected virtual UniTask showScene(VisualElement parentElement, ChangeType changeType)
    {
        parentElement.Add(rootElement);
        return UniTask.CompletedTask;
    }
    protected virtual void onBeforeShow() { }
    protected virtual void onAfterShow() { }

    public void closeScene(VisualElement rootElement)
    {
        onBeforeHide();
        hideScene(rootElement);
        onAfterHide();
    }

    protected virtual void hideScene(VisualElement parentElement)
    {
        parentElement.Remove(rootElement);
    }

    protected virtual void onBeforeHide() { }
    protected virtual void onAfterHide() { }
}

public enum ChangeType
{
    Back,
    Enter,
    Notification
}