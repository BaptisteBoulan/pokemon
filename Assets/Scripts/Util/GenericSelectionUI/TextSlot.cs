using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextSlot : MonoBehaviour, ISelectableItem
{
    [SerializeField] Text text;

    public void SetText(string text)
    {
        this.text.text = text;
    }


    Color originalColor;

    public void Init()
    {
        originalColor = text.color != GlobalSettings.i.HighlightedColor ? text.color : Color.black;
    }

    public void OnSelectionChanged(bool selected)
    {
        text.color = selected ? GlobalSettings.i.HighlightedColor : originalColor;
    }
}
