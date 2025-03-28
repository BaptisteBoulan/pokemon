using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceText : MonoBehaviour
{
    Text text;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    public Text TextField => text;

    public void SetSelected(bool selected)
    {
        text.color = selected ? GlobalSettings.i.HighlightedColor : Color.black;
    }
}
