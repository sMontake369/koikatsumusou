using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
partial class ListAutoFit : ListView
{
    [UxmlAttribute]
    [Tooltip("高さ当たりのリスト数")]
    public int itemNumPerHeight { get; set; } = 10;

    public ListAutoFit()
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
        UpdateItemHeight();
    }

    private void UpdateItemHeight()
    {
        float newHeightSize = 0;

        try
        {
            float previousHeightSize = resolvedStyle.height;
            newHeightSize = previousHeightSize / itemNumPerHeight;

            // 値を調整
            fixedItemHeight = newHeightSize;
            foreach (var item in this.Query<VisualElement>(className: "unity-list-view__item").ToList())
            {
                item.style.height = new StyleLength(new Length(newHeightSize, LengthUnit.Pixel));
            }
        }
        finally 
        {

        }
    }
}