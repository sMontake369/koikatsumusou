using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
partial class MessageElement : VisualElement
{
    [UxmlAttribute]
    public Label messageElement { get; set; }
    [UxmlAttribute]
    public Label timeElement { get; set; }
    [UxmlAttribute]
    public string message
    {
        get => messageElement.text;
        set => messageElement.text = value;
    }
    [UxmlAttribute]
    public string time
    {
        get => timeElement.text;
        set => timeElement.text = value;
    }

    [UxmlAttribute]
    [Tooltip("1行あたりのメッセージ数")]
    public int messageNumPerLine { get; set; } = 10;

    [UxmlAttribute]
    [Tooltip("時間サイズ")]
    public int timeNumPerLine { get; set; } = 3;

    [UxmlAttribute]
    [Tooltip("時間サイズ")]
    public Color messageBColor { get; set; }

    public MessageElement()
    {
        RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
        messageElement = new Label();
        messageElement.name = "Message";
        messageElement.style.whiteSpace = WhiteSpace.Normal;
        messageElement.style.unityTextAlign = TextAnchor.MiddleLeft;
        messageElement.style.marginBottom = 0;
        messageElement.style.marginTop = 0;
        messageElement.style.marginLeft = 0;
        messageElement.style.marginRight = 0;

        messageElement.style.paddingBottom = new Length(2, LengthUnit.Percent);
        messageElement.style.paddingTop = new Length(2, LengthUnit.Percent);
        messageElement.style.paddingLeft = new Length(2, LengthUnit.Percent);
        messageElement.style.paddingRight = new Length(2, LengthUnit.Percent);

        messageElement.style.borderTopLeftRadius = new Length(10, LengthUnit.Pixel);
        messageElement.style.borderTopRightRadius = new Length(10, LengthUnit.Pixel);
        messageElement.style.borderBottomLeftRadius = new Length(10, LengthUnit.Pixel);
        messageElement.style.borderBottomRightRadius = new Length(10, LengthUnit.Pixel);

        messageElement.style.backgroundColor = new Color(0.8705882f, 0.8705882f, 0.8705882f, 1f);

        this.Add(messageElement);

        timeElement = new Label();
        timeElement.name = "Time";
        timeElement.style.unityTextAlign = TextAnchor.LowerCenter;
        timeElement.style.marginBottom = 0;
        timeElement.style.marginTop = 0;
        timeElement.style.marginLeft = new Length(1, LengthUnit.Percent);
        timeElement.style.marginRight = new Length(1, LengthUnit.Percent);
        
        timeElement.style.paddingBottom = 0;
        timeElement.style.paddingTop = 0;
        timeElement.style.paddingLeft = 0;
        timeElement.style.paddingRight = 0;
        this.Add(timeElement);
    }

    protected void OnAttachToPanel(AttachToPanelEvent e)
    {
        UnregisterCallback<AttachToPanelEvent>(OnAttachToPanel);
        RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }

    protected void OnGeometryChanged(GeometryChangedEvent e)
    {
        UpdateContentAndFontSize();
    }

    public void UpdateContentAndFontSize()
    {
        try
        {
            int messageLength = messageElement.text.Length; // 文字数
            if (messageLength == 0) return;

            float width = resolvedStyle.width;
            float maxMessageWidth = width * 0.80f;

            int lineNum = math.max(1, Mathf.CeilToInt((float)messageLength / messageNumPerLine)); // 何行になるか
            int maxLineCharNum = math.min(messageLength, messageNumPerLine); // 1行あたりの文字数

            float widthRatio = (float)maxLineCharNum / messageNumPerLine; // 横サイズの割合
            float messageWidth = math.min(maxMessageWidth, resolvedStyle.width * widthRatio);
            
            float messageFontSize = (messageWidth - messageElement.resolvedStyle.paddingLeft - messageElement.resolvedStyle.paddingRight - (maxLineCharNum * 0.5f)) / maxLineCharNum;
            float messageHeight = messageFontSize * lineNum + messageElement.resolvedStyle.paddingTop + messageElement.resolvedStyle.paddingBottom;

            float timeWidth = width * 0.18f;

            style.height = new StyleLength(new Length(messageHeight + resolvedStyle.paddingTop + resolvedStyle.paddingBottom + (5 * lineNum), LengthUnit.Pixel));

            messageElement.style.width = new StyleLength(new Length(messageWidth, LengthUnit.Pixel));
            messageElement.style.height = new StyleLength(new Length(messageHeight + resolvedStyle.paddingTop + resolvedStyle.paddingBottom + (5 * lineNum), LengthUnit.Pixel));
            messageElement.style.fontSize = new StyleLength(new Length(messageFontSize, LengthUnit.Pixel));
            messageElement.style.backgroundColor = messageBColor;

            timeElement.style.width = new StyleLength(new Length(timeWidth + resolvedStyle.paddingLeft + resolvedStyle.paddingRight, LengthUnit.Pixel));
            timeElement.style.height = new StyleLength(new Length(messageHeight + resolvedStyle.paddingTop + resolvedStyle.paddingBottom + (5 * lineNum), LengthUnit.Pixel));
            timeElement.style.fontSize = new StyleLength(new Length(timeWidth / timeNumPerLine, LengthUnit.Pixel));
        }
        finally
        {

        }
    }
}
