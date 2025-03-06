using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "SwiperData", menuName = "CustomUIElements/SwiperData")]
public class SwiperData : ScriptableObject
{
    [Tooltip("スライドさせたいVisualTreeAsset")]
    public List<VisualTreeAsset> slideList = new List<VisualTreeAsset>();
}
