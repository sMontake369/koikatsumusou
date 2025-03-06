using System;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class BaseAppManager : MonoBehaviour
{
    public VisualTreeAsset appElement;
    protected SmartPhoneManager smaM;
    protected VisualElement rootAppElement;

    public void init()
    {
        rootAppElement = appElement.Instantiate().Q<VisualElement>("rootAppElement");
        smaM = GameManager.smaM;
        initM();
    }
    protected abstract void initM();
    public virtual void startGame(BaseAppData baseAppData) { }
    public virtual void onStep() { }

    public void openApp(VisualElement rootElement, ChangeType changeType)
    {
        onBeforeShow();
        showApp(rootElement, changeType);
        onAfterShow();
    }
    protected virtual void showApp(VisualElement rootElement, ChangeType changeType)
    {
        rootElement.Add(rootAppElement);
    }
    protected virtual void onBeforeShow() { }
    protected virtual void onAfterShow() { }

    public void closeApp(VisualElement rootElement)
    {
        onBeforeHide();
        hideApp(rootElement);
        onAfterHide();
    }
    protected virtual void hideApp(VisualElement rootElement)
    {
        rootElement.Remove(rootAppElement);
    }
    protected virtual void onBeforeHide() { }
    protected virtual void onAfterHide() { }

    public abstract void notification(NotificationData notificationData);
}
