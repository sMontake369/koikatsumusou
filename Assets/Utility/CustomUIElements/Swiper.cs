using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 複数の要素をスライドできるカスタムUIElement
/// </summary>
/// <author>montake369</author>
/// <date>2025/03/03</date>
[UxmlElement]
partial class Swiper : VisualElement
{
    [UxmlAttribute("SlideData"), 
    Tooltip("スライドさせたい要素を入れたSwiperData\nSwiperDataはプロジェクトを右クリック->新規->customUIElements->SwiperDataで作成できます")]
    SwiperData swiperData { get; set; }

    [UxmlAttribute("TransitionTime"), Tooltip("スライド時のアニメーション時間")]
    float transitionTime = 0.5f;

    [UxmlAttribute("RightArrowTexture"), Tooltip("右矢印のテクスチャ")]
    Texture2D rightArrowTexture;

    [UxmlAttribute("LeftArrowTexture"), Tooltip("左矢印のテクスチャ")]
    Texture2D leftArrowTexture;

    [UxmlAttribute("CloseTexture"), Tooltip("閉じるボタンのテクスチャ")]
    Texture2D closeTexture;

    public event Action OnShow;
    public event Action OnHide;
    public event Action OnSwipe;

    VisualElement rightArrow;
    VisualElement leftArrow;
    VisualElement close;
    VisualElement swiperElement;
    VisualElement swipeContainer;
    VisualElement scrollElement;
    List<TemplateContainer> swipeElementList; 
    EventCallback<PointerMoveEvent> pointerMoveCallback;
    EventCallback<PointerUpEvent> pointerUpCallback;
    EventCallback<PointerLeaveEvent> pointerLeaveCallback;
    CancellationTokenSource cts;
    float deltaX;
    string pointerType;
    int index;

    public Swiper()
    {
        swiperElement = new VisualElement{
            name = "SwiperElement",
            style = {
                flexDirection = FlexDirection.Row,
                width = new Length(100, LengthUnit.Percent),
                height = new Length(100, LengthUnit.Percent),
            }
        };
        this.Add(swiperElement);

        leftArrow = new VisualElement{
            name = "LeftArrow",
            style = {
                width = new Length(8, LengthUnit.Percent),
                marginTop = new Length(20, LengthUnit.Percent),
                marginLeft = new Length(1, LengthUnit.Percent),
                marginBottom = new Length(20, LengthUnit.Percent),
                marginRight = new Length(1, LengthUnit.Percent),
                backgroundPositionX = new BackgroundPosition(BackgroundPositionKeyword.Center),
                backgroundPositionY = new BackgroundPosition(BackgroundPositionKeyword.Center),
                backgroundRepeat = new BackgroundRepeat(Repeat.NoRepeat, Repeat.NoRepeat),
                backgroundSize = new BackgroundSize(BackgroundSizeType.Contain),
                unityBackgroundImageTintColor = Color.black,
                backgroundImage = leftArrowTexture,
            }
        };
        leftArrow.AddToClassList("swiper-button");
        leftArrow.RegisterCallback<MouseDownEvent>((e) => { Swipe(index - 1); });
        swiperElement.Add(leftArrow);

        swipeElementList = new List<TemplateContainer>();
        swipeContainer = new VisualElement{
            name = "SwipeContainer",
            style = {
                overflow = Overflow.Hidden,
                width = new Length(80, LengthUnit.Percent),
                height = new Length(100, LengthUnit.Percent),
            }
        };
        swipeContainer.AddToClassList("swiper-container");
        swiperElement.Add(swipeContainer);
        swipeContainer.RegisterCallback<PointerDownEvent>((e) => { OnPointerDownEvent(e); });
        pointerMoveCallback = (e) => { OnPointerMovedEvent(e); };
        pointerUpCallback = (e) => { OnPointerUpEvent(e); };
        pointerLeaveCallback = (e) => { OnPointerUpEvent(e); };

        scrollElement = new VisualElement{
            name = "ScrollElement",
            style = {
                width = new Length(100, LengthUnit.Percent),
                height = new Length(100, LengthUnit.Percent),
            }
        };
        scrollElement.AddToClassList("swiper-scroll");
        swipeContainer.Add(scrollElement);

        rightArrow = new VisualElement{
            name = "RightArrow",
            style = {
                width = new Length(8, LengthUnit.Percent),
                unityBackgroundImageTintColor = Color.black,
                backgroundImage = leftArrowTexture,
                marginTop = new Length(20, LengthUnit.Percent),
                marginLeft = new Length(1, LengthUnit.Percent),
                marginBottom = new Length(20, LengthUnit.Percent),
                marginRight = new Length(1, LengthUnit.Percent),
                backgroundPositionX = new BackgroundPosition(BackgroundPositionKeyword.Center),
                backgroundPositionY = new BackgroundPosition(BackgroundPositionKeyword.Center),
                backgroundRepeat = new BackgroundRepeat(Repeat.NoRepeat, Repeat.NoRepeat),
                backgroundSize = new BackgroundSize(BackgroundSizeType.Contain)
            }
        };
        rightArrow.AddToClassList("swiper-button");
        rightArrow.RegisterCallback<MouseDownEvent>((e) => { Swipe(index + 1); });
        swiperElement.Add(rightArrow);

        close = new VisualElement{
            name = "Close",
            style = {
                position = Position.Absolute,
                width = new Length(8, LengthUnit.Percent),
                height = new Length(20, LengthUnit.Percent),
                top = 0,
                right = 0,
                marginTop = new Length(1, LengthUnit.Percent),
                marginRight = new Length(1, LengthUnit.Percent),
                backgroundRepeat = new BackgroundRepeat(Repeat.NoRepeat, Repeat.NoRepeat),
                backgroundSize = new BackgroundSize(BackgroundSizeType.Contain),
                backgroundPositionY = new BackgroundPosition(BackgroundPositionKeyword.Top),
                unityBackgroundImageTintColor = Color.black,
                backgroundImage = closeTexture
            }
        };
        close.AddToClassList("swiper-button");
        close.RegisterCallback<MouseDownEvent>((e) => { Hide(); });
        swiperElement.Add(close);

        RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
    }

    private void OnAttachToPanel(AttachToPanelEvent e)
    {
        UnregisterCallback<AttachToPanelEvent>(OnAttachToPanel);

        rightArrow.style.backgroundImage = rightArrowTexture;
        leftArrow.style.backgroundImage = leftArrowTexture;
        close.style.backgroundImage = closeTexture;
        CreateSlide();
    }

    private void OnPointerDownEvent(PointerDownEvent e)
    {
        swipeContainer.RegisterCallback(pointerMoveCallback);
        this.RegisterCallback(pointerLeaveCallback);
        this.RegisterCallback(pointerUpCallback);

        pointerType = e.pointerType;
        deltaX = e.localPosition.x;
        cts.Cancel();
    }

    private void OnPointerMovedEvent(PointerMoveEvent e)
    {
        scrollElement.style.translate = new StyleTranslate
        (
            new Translate(new Length(scrollElement.resolvedStyle.translate.x + (e.localPosition.x - deltaX), LengthUnit.Pixel), 0, 0)
        );

        deltaX = e.localPosition.x;
    }

    private void OnPointerUpEvent<T>(T e) where T : PointerEventBase<T>, new()
    {
        if (pointerType != e.pointerType) return;

        swipeContainer.UnregisterCallback(pointerMoveCallback);
        this.UnregisterCallback(pointerLeaveCallback);
        this.UnregisterCallback(pointerUpCallback);

        int index = Mathf.RoundToInt(scrollElement.resolvedStyle.translate.x * -1 / scrollElement.resolvedStyle.width);
        Swipe(math.clamp(index, 0, swipeElementList.Count - 1));
    }

    private void CreateSlide()
    {
        if (swiperData == null) return;
        swipeElementList.Clear();
        scrollElement.Clear();
        index = 0;
        foreach (VisualTreeAsset slideAsset in swiperData.slideList)
        {
            if (slideAsset == null) continue;
            TemplateContainer slideElement = slideAsset.Instantiate();
            slideElement.name = slideAsset.name;
            slideElement.style.width = new Length(100, LengthUnit.Percent);
            slideElement.style.height = new Length(100, LengthUnit.Percent);
            slideElement.style.position = Position.Absolute;
            slideElement.style.translate = new StyleTranslate(new Translate(new Length(100 * index, LengthUnit.Percent), 0, 0));
            slideElement.AddToClassList("swiper-element");

            scrollElement.Add(slideElement);
            swipeElementList.Add(slideElement);
            index++;
        }
        index = 0;
        Swipe(0);
    }

    /// <summary>
    /// 要素を表示する
    /// </summary>
    public void Show()
    {
        this.style.display = DisplayStyle.Flex;
        OnShow?.Invoke();
    }

    /// <summary>
    /// 要素を非表示にし、スライドを初期位置に戻す
    /// </summary>
    public void Hide()
    {
        this.style.display = DisplayStyle.None;
        Swipe(0);
        OnHide?.Invoke();
    }

    /// <summary>
    /// 要素をスライドさせる
    /// </summary>
    /// <param name="newIndex"> スライド先番号</param>
    public void Swipe(int newIndex)
    {
        if (index < 0 || index >= swipeElementList.Count) return;

        if (this.style.display != DisplayStyle.None && index != newIndex) OnSwipe?.Invoke();

        index = newIndex;

        if (index == 0) leftArrow.visible = false;
        else leftArrow.visible = true;
        if (index == swipeElementList.Count - 1) rightArrow.visible = false;
        else rightArrow.visible = true;

        float pos = scrollElement.resolvedStyle.width * index * -1;

        cts = new CancellationTokenSource();
        Tween tween = DOTween.To(
            () => scrollElement.resolvedStyle.translate.x,
            (value) => scrollElement.style.translate = new StyleTranslate(
                new Translate(new Length(value, LengthUnit.Pixel), 0, 0)
            ),
            pos,
            transitionTime
        );

        cts.Token.Register(() => { if (tween.IsActive()) tween.Kill(); });
    }
}