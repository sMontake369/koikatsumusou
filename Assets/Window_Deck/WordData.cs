using UnityEngine;

[CreateAssetMenu(fileName = "wordData ", menuName = "LineData/WordData")]
public class WordData : ScriptableObject
{
    public int id;
    public string type;
    public Texture2D icon;
    public string word;

    public WordData deepCopy()
    {
        WordData copy = CreateInstance<WordData>();
        copy.id = id;
        copy.type = type;
        copy.icon = icon;
        copy.word = word;
        return copy;
    }
}