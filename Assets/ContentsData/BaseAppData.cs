using NaughtyAttributes;
using UnityEngine;

public abstract class BaseAppData : ScriptableObject
{
    [Label("アプリ名")]
    public string appName;

    // [Label("アプリアイコン")]
    // public Texture2D appIcon;
    public abstract BaseAppData deepCopy();
}
