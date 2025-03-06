using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

[UxmlElement]
partial class ContentRatioFit : VisualElement
{
    [UxmlAttribute]
    [Tooltip("基準軸のサイズに対する変更する軸のサイズ割合")]
    public float ratio { get; set; } = 1.0f;

    [UxmlAttribute]
    [Tooltip("高さを元に横幅を調整する")]
    public bool fitHeight { get; set; } = false;

    public ContentRatioFit()
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
        UpdateContentSize();
    }

    private void UpdateContentSize()
    {
        try
        {
            if (fitHeight)
            {
                float height = resolvedStyle.height;
                float width = height * ratio;
                style.width = new StyleLength(new Length(width, LengthUnit.Pixel));
            }
            else
            {
                float width = resolvedStyle.width;
                float height = width * ratio;
                style.height = new StyleLength(new Length(height, LengthUnit.Pixel));
            }
        }
        finally
        {
            
        }
    }
}
