using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class BaseAppSceneManager : MonoBehaviour
{
    public VisualElement rootElement { get; protected set; }

    public async UniTask ShowScene(VisualElement rootElement, ChangeType changeType)
    {
        OnBeforeShow();
        await Show(rootElement, changeType);
        OnAfterShow();
    }

    protected virtual UniTask Show(VisualElement parentElement, ChangeType changeType)
    {
        parentElement.Add(rootElement);
        return UniTask.CompletedTask;
    }

    protected virtual void OnBeforeShow() { }
    protected virtual void OnAfterShow() { }

    public void HideScene(VisualElement rootElement)
    {
        OnBeforeHide();
        Hide(rootElement);
        OnAfterHide();
    }

    protected virtual void Hide(VisualElement parentElement)
    {
        parentElement.Remove(rootElement);
    }

    protected virtual void OnBeforeHide() { }
    protected virtual void OnAfterHide() { }
}

public enum ChangeType
{
    Back,
    Enter,
    Notification
}