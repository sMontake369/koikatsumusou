using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

/// <summary>
/// 要素サイズに合わせて文字サイズを調整するカスタムUIElement
/// </summary>
/// <author>montake369</author>
/// <date>2025/03/03</date>
[UxmlElement]
partial class test : Label
{
    [UxmlAttribute]
    [Tooltip("割合")]
    float ratio { get; set; } = 1;

    public test()
    {
        RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
    }

    private void OnAttachToPanel(AttachToPanelEvent e)
    {
        UnregisterCallback<AttachToPanelEvent>(OnAttachToPanel);
        RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }

    private void OnGeometryChanged(GeometryChangedEvent e)
    {
        UpdateFontSize();
    }

    private void UpdateFontSize()
    {
        try
        {        
            Font font = resolvedStyle.unityFontDefinition.font;
            float width = resolvedStyle.width - (resolvedStyle.paddingLeft + resolvedStyle.paddingRight);
            float height = resolvedStyle.height - (resolvedStyle.paddingTop + resolvedStyle.paddingBottom) - font.ascent;
            
            float fontSize = math.min(width / text.Length * ratio, height);
            style.fontSize = new StyleLength(new Length(fontSize, LengthUnit.Pixel));
        }
        finally
        {
            
        }
    }
}