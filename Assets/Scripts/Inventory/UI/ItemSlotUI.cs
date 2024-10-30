using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text countText;

    RectTransform rectTransform;


    public Text NameText => nameText;
    public Text CountText => countText;

    public void SetNameAndCount(ItemSlot itemSlot)
    {
        rectTransform = GetComponent<RectTransform>();
        nameText.text = itemSlot.Item.Name;
        countText.text = $"x {itemSlot.Count}";
    }

    public void SetNameAndPrice(ItemBase item)
    {
        rectTransform = GetComponent<RectTransform>();
        nameText.text = item.Name;
        countText.text = $"$ {item.Price}";
    }

    public float Height => rectTransform.rect.height;
}
