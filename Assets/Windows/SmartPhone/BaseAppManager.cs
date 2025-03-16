using UnityEngine;
using UnityEngine.UIElements;

public abstract class BaseAppManager : MonoBehaviour
{
    public VisualTreeAsset appElement;
    protected SmartPhoneManager smaM;
    protected VisualElement rootAppElement;

    public void Init()
    {
        rootAppElement = appElement.Instantiate().Q<VisualElement>("rootAppElement");
        smaM = GameManager.smaM;
        InitM();
    }

    protected abstract void InitM();
    public virtual void StartGame(BaseAppData baseAppData) { }
    public virtual void OnStep() { }

    public void ShowApp(VisualElement rootElement, ChangeType changeType)
    {
        OnBeforeShow();
        Show(rootElement, changeType);
        OnAfterShow();
    }
    protected virtual void Show(VisualElement rootElement, ChangeType changeType)
    {
        rootElement.Add(rootAppElement);
    }
    protected virtual void OnBeforeShow() { }
    protected virtual void OnAfterShow() { }

    public void HideApp(VisualElement rootElement)
    {
        OnBeforeHide();
        Hide(rootElement);
        OnAfterHide();
    }
    protected virtual void Hide(VisualElement rootElement)
    {
        rootElement.Remove(rootAppElement);
    }
    protected virtual void OnBeforeHide() { }
    protected virtual void OnAfterHide() { }

    public abstract void Notification(NotificationData notificationData);
}
