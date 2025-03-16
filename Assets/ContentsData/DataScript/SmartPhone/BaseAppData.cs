using NaughtyAttributes;
using UnityEngine;

public abstract class BaseAppData : ScriptableObject
{
    [Label("アプリ名")]
    public string appName;

    public abstract BaseAppData Copy();
}
