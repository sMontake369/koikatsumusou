using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 要素サイズに合わせて文字サイズを調整するカスタムUIElement
/// </summary>
/// <author>montake369</author>
/// <date>2025/03/03</date>
[UxmlElement]
partial class LabelAutoFit : Label
{
    [UxmlAttribute]
    [Tooltip("要素サイズからの割合で文字サイズを決定する")]
    bool useRatio { get; set; } = false;

    [UxmlAttribute]
    [Tooltip("1行あたりの最大文字数")]
    int charNum { get; set; } = 10;

    [UxmlAttribute]
    [Tooltip("割合")]
    float ratio { get; set; } = 1;

    public LabelAutoFit()
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
            float width = resolvedStyle.width - (resolvedStyle.paddingLeft + resolvedStyle.paddingRight);
            float height = resolvedStyle.height - (resolvedStyle.paddingTop + resolvedStyle.paddingBottom);
            
            float fontSize;
            if (useRatio) fontSize = math.max(0, math.min(width / text.Length * ratio, height));
            else fontSize = width / charNum;
            style.fontSize = new StyleLength(new Length(fontSize, LengthUnit.Pixel));
        }
        finally
        {
            
        }
    }
}