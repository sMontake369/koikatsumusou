using System.Collections.Generic;
using UnityEngine;

public static class DeepCopy
{
    public static List<T> DeepCopyList<T>(List<T> original) where T : Object
    {
        if (original == null) return null;
        List<T> copy = new List<T>();
        foreach (var item in original)
        {
            copy.Add(item != null ? Object.Instantiate(item) : null);
        }
        return copy;
    }
}
