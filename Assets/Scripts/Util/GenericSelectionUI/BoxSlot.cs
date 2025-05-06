using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxSlot : MonoBehaviour, ISelectableItem
{
    Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    Color originalColor;

    public void Init()
    {
        originalColor = image.color;
    }

    public void Clear()
    {
        image.color = originalColor;
    }

    public void OnSelectionChanged(bool selected)
    {
        image.color = selected ? GlobalSettings.i.BgHighlightedColor : originalColor;
    }
}
