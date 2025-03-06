using UnityEngine;
using UnityEngine.UIElements;

public abstract class BaseWindowManager : MonoBehaviour
{
    public VisualElement rootElement;
    public abstract void init();
    public virtual void startGame() { }
    public void showWindow()
    {
        rootElement.style.display = DisplayStyle.Flex;
    }
    public void hideWindow()
    {
        rootElement.style.display = DisplayStyle.None;
    }
}
