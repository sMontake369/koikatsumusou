using UnityEngine;
using UnityEngine.UIElements;

public abstract class BaseWindowManager : MonoBehaviour
{
    public VisualElement rootElement;
    public abstract void Init();
    public virtual void StartGame() { }
    public void ShowWindow()
    {
        rootElement.style.display = DisplayStyle.Flex;
    }
    public void HideWindow()
    {
        rootElement.style.display = DisplayStyle.None;
    }
}
