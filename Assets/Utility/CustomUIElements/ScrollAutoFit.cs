using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
partial class ScrollAutoFit : ScrollView
{
    [UxmlAttribute]
    [Tooltip("1行あたりのアイテム数")]
    public int itemNumPerWidth { get; set; } = 3;
    
    public ScrollAutoFit()
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
        UpdateItemWidth();
        ForceUpdate();
    }

    public void UpdateItemWidth()
    {

        try
        {
            float width = resolvedStyle.width;
            int itemWidth = Mathf.FloorToInt(width / itemNumPerWidth) - 5;
            foreach (var item in this.Q<VisualElement>(className: "unity-scroll-view__content-container").Children())
            {
                item.style.width = new StyleLength(new Length(itemWidth, LengthUnit.Pixel));
            }
        }
        finally
        {

        }
    }

    public void ForceUpdate()
    {
        this.schedule.Execute(() =>
        {
            var fakeOldRect = Rect.zero;
            var fakeNewRect = this.layout;

            using var evt = GeometryChangedEvent.GetPooled(fakeOldRect, fakeNewRect);
            evt.target = this.contentContainer;
            this.contentContainer.SendEvent(evt);
        });
    }
}
