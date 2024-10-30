using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public enum BuyState { Choosing, Busy}
public class ShopUI : MonoBehaviour
{
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;

    [SerializeField] Image itemIcon;
    [SerializeField] Text itemDescription;

    [SerializeField] Image upArrow;
    [SerializeField] Image downArrow;

    List<ItemBase> availablesItems;
    List<ItemSlotUI> slotUIList;

    int selectedItem = 0;
    const int itemInViewPort = 6;
    RectTransform itemListRect;

    BuyState state;

    bool showUpArrow;
    bool showDownArrow;

    public event Action OnClose;

    private void Awake()
    {
        itemListRect = itemList.GetComponent<RectTransform>();
    }

    public void Show(List<ItemBase> availablesItems)
    {
        gameObject.SetActive(true);

        this.availablesItems = availablesItems;

        UpdateItemList();

        state = BuyState.Choosing;
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    void UpdateItemList()
    {
        // Clear all the existing items
        slotUIList = new List<ItemSlotUI>();
        foreach (Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in availablesItems)
        {
            var slotUiObj = Instantiate(itemSlotUI, itemList.transform);
            slotUiObj.SetNameAndPrice(item);

            slotUIList.Add(slotUiObj);
        }

        UpdateItemSelection();
    }
    private void UpdateItemSelection()
    {
        selectedItem = Mathf.Clamp(selectedItem, 0, slotUIList.Count() - 1);

        for (int i = 0; i < slotUIList.Count(); i++)
        {
            if (i == selectedItem)
            {
                slotUIList[i].NameText.color = GlobalSettings.i.HighlightedColor;
                slotUIList[i].CountText.color = GlobalSettings.i.HighlightedColor;
            }
            else
            {
                slotUIList[i].NameText.color = Color.black;
                slotUIList[i].CountText.color = Color.black;
            }
        }

        if (availablesItems.Count > 0)
        {
            // Security to avoid the game from crashing when the last item is used
            selectedItem = Mathf.Clamp(selectedItem, 0, availablesItems.Count - 1);

            var item = availablesItems[selectedItem];
            itemIcon.sprite = item.Icon;
            itemDescription.text = item.Description;

            HandleScrolling();
        }


    }
    void HandleScrolling()
        {
            // var scrollPos = Mathf.Clamp(selectedItem - itemInViewPort/2, 0, selectedItem )* slotUIList[0].Height;
            var scrollPos = Mathf.Clamp(selectedItem - itemInViewPort / 2, 0, selectedItem) * 55;

            itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);

            showUpArrow = selectedItem > itemInViewPort / 2;
            showDownArrow = selectedItem + itemInViewPort / 2 < slotUIList.Count && slotUIList.Count > itemInViewPort;
        }

    public void HandleUpdate()
    {
        if (state == BuyState.Choosing)
        {
            var prevSelectedItem = selectedItem;

            if (Input.GetKeyDown(KeyCode.DownArrow))
                selectedItem++;
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                selectedItem--;

            if (prevSelectedItem != selectedItem)
                UpdateItemSelection();


            if (Input.GetKeyDown(KeyCode.X))
            {
                Close();
                OnClose?.Invoke();
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                state = BuyState.Busy;
                StartCoroutine(BuyItem(availablesItems[selectedItem]));
            }

            upArrow.gameObject.SetActive(showUpArrow);
            downArrow.gameObject.SetActive(showDownArrow);
        }
    }

    IEnumerator BuyItem(ItemBase item)
    {
        yield return ShopController.instance.BuyItem(item);
        state = BuyState.Choosing;
    }
}
