using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
partial class LabelContentAutoFit : Label
{
    [UxmlAttribute]
    [Tooltip("1行あたりの文字数")]
    public int CharNumPerLine { get; set; } = 10;
    public LabelContentAutoFit()
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
        UpdateContentAndFontSize();
    }

    public virtual void UpdateContentAndFontSize()
    {
        try
        {
            int charLength = text.Length; // 文字数
            if (charLength == 0) return;

            int lineNum = math.max(1, Mathf.CeilToInt((float)charLength / CharNumPerLine));
            int maxLineCharNum = math.min(charLength, CharNumPerLine); // 1行あたりの最大文字数
            float widthRatio = (float)maxLineCharNum / CharNumPerLine; // 横サイズの割合
            float newWidth = resolvedStyle.maxWidth.value * widthRatio; 
            float fontSize = (newWidth - resolvedStyle.paddingLeft - resolvedStyle.paddingRight) / maxLineCharNum;
            float newHeight = fontSize * lineNum ;

            style.width = new StyleLength(new Length(newWidth + resolvedStyle.paddingLeft + resolvedStyle.paddingRight, LengthUnit.Pixel));
            style.height = new StyleLength(new Length(newHeight + resolvedStyle.paddingTop + resolvedStyle.paddingBottom + (5 * lineNum), LengthUnit.Pixel));
            style.fontSize = new StyleLength(new Length(fontSize, LengthUnit.Pixel));
        }
        finally
        {

        }
    }
}