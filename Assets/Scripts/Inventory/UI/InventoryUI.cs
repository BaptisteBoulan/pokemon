using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Utils.GenericSelectionUI;


public enum InventoryUIstate { ItemSelection, PartySelection, Busy, MoveToForget}

public class InventoryUI : SelectionUI<TextSlot>
{

    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;

    [SerializeField] Image itemIcon;
    [SerializeField] Text itemDescription;
    [SerializeField] Text itemCategory;

    [SerializeField] PartyScreen partyScreen;
    [SerializeField] MoveToForgetSelection moveSelectionUI;

    [SerializeField] Image upArrow;
    [SerializeField] Image downArrow;

    [SerializeField] Image leftArrow;
    [SerializeField] Image rightArrow;

    Action<ItemBase> onItemUsed;

    bool showUpArrow;
    bool showDownArrow;

    InventoryUIstate state;

    
    const int itemInViewPort = 8;

    int selectedCategory;

    List<ItemSlotUI> slotUIList;

    Inventory inventory;
    RectTransform itemListRect;

    MoveBase moveToLearn;
    private void Awake()
    {
        inventory = Inventory.GetInventory();
        itemListRect = itemList.GetComponent<RectTransform>();
    }

    private void Start()
    {
        UpdateItemList();
        inventory.OnUpdated += UpdateItemList;
    }

    void UpdateItemList()
    {
        // Clear all the existing items
        slotUIList = new List<ItemSlotUI> ();
        foreach (Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var itemSlot in inventory.GetSlotsByCategory(selectedCategory))
        {
            var slotUiObj = Instantiate(itemSlotUI, itemList.transform);
            slotUiObj.SetNameAndCount(itemSlot);

            slotUIList.Add(slotUiObj);
        }

        SetItems(slotUIList.Select(s => s.GetComponent<TextSlot>()).ToList());

        UpdateSelectionUI();
    }

    public override void HandleUpdate()
    {
        var prevSelectedCategory = selectedCategory;
        if (Input.GetKeyDown(KeyCode.RightArrow))
            selectedCategory++;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            selectedCategory--;


        if (selectedCategory == Inventory.ItemCategories.Count())
            selectedCategory = 0;
        else if (selectedCategory == -1)
            selectedCategory = Inventory.ItemCategories.Count() - 1;

        if (prevSelectedCategory != selectedCategory)
        {
            ResetSelection();
            itemCategory.text = Inventory.ItemCategories[selectedCategory];
            UpdateItemList();
        }

        upArrow.gameObject.SetActive(showUpArrow);
        downArrow.gameObject.SetActive(showDownArrow);

        base.HandleUpdate();

    }

    public override void UpdateSelectionUI()
    {
        var slots = inventory.GetSlotsByCategory(selectedCategory);

        if (slots.Count > 0)
        {
            // Security to avoid the game from crashing when the last item is used
            selectedItem = Mathf.Clamp(selectedItem, 0, slots.Count - 1);

            var item = slots[selectedItem].Item;
            itemIcon.sprite = item.Icon;
            itemDescription.text = item.Description;

        }
        HandleScrolling();

        base.UpdateSelectionUI();
    }

    void HandleScrolling()
    {
        // var scrollPos = Mathf.Clamp(selectedItem - itemInViewPort/2, 0, selectedItem )* slotUIList[0].Height;
        var scrollPos = Mathf.Clamp(selectedItem - itemInViewPort / 2, 0, selectedItem) * 55;

        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);

        showUpArrow = selectedItem > itemInViewPort / 2;
        showDownArrow = selectedItem + itemInViewPort / 2 < slotUIList.Count && slotUIList.Count > itemInViewPort;
    }



    void ResetSelection()
    {
        selectedItem = 0;
        upArrow.gameObject.SetActive(false);
        downArrow.gameObject.SetActive(false);

        itemIcon.sprite = null;
        itemDescription.text = "";
    }

    public ItemBase SelectedItem
    {
        get { return inventory.GetItem(selectedItem, selectedCategory); }
    }
}
