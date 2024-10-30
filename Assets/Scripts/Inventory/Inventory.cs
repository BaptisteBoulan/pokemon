using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class Inventory : MonoBehaviour, ISavable
{
    [SerializeField] List<ItemSlot> recoverySlots;
    [SerializeField] List<ItemSlot> pokeballSlots;
    [SerializeField] List<ItemSlot> tmSlots;
    [SerializeField] List<ItemSlot> items;

    public static List<string> ItemCategories { get; set; } = new List<string>()
    {
        "RECOVERIES","POKEBALLS","TMs & HMs","ITEMS",
    };

    List<List<ItemSlot>> allSlots;

    public event Action OnUpdated;

    private void Awake()
    {
        allSlots = new List<List<ItemSlot>>()
        {
            recoverySlots,pokeballSlots,tmSlots,items
        };
    }
    public static Inventory GetInventory()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }

    public bool UseItem(ItemBase item, Pokemon pokemon)
    {
        bool itemUsed = item.Use(pokemon);

        if (itemUsed)
        {
            if (!item.IsReusable)
                RemoveItem(item);
            return true;
        }
        else return false;
    }

    public bool HasItem(ItemBase item, int count=1)
    {
        var category = (int) GetCategoryFromItem(item);
        var currentSlot = GetSlotsByCategory(category);

        return currentSlot.Exists(i => i.Item == item && i.Count >= count);
    }

    public int GetCountFromItem(ItemBase item)
    {
        var category = (int)GetCategoryFromItem(item);
        var currentSlot = GetSlotsByCategory(category);

        var slot = currentSlot.Where(slot => slot.Item == item).FirstOrDefault();

        if (slot != null) return slot.Count;
        return 0;
    }

    public void RemoveItem(ItemBase item, int count=1)
    {
        // Assume the player has the items, won't bug if he hasnt though
        var category = (int)GetCategoryFromItem(item);
        var currentSlots = GetSlotsByCategory(category);
        var itemSlot = currentSlots.First(slot => slot.Item == item);
        itemSlot.Count -= count;

        if(itemSlot.Count <= 0 )
        {
            currentSlots.Remove(itemSlot);
        }

        OnUpdated?.Invoke();
    }

    public void AddItem(ItemBase item, int count=1)
    {
        var selectedCategory = (int) GetCategoryFromItem(item);
        var currentSlots = GetSlotsByCategory(selectedCategory);
        var itemSlot = currentSlots.FirstOrDefault(slot => slot.Item == item);
        if (itemSlot != null)
            itemSlot.Count += count;
        else
            currentSlots.Add(new ItemSlot()
            {
                Count = count,
                Item = item,
            }) ;

        OnUpdated?.Invoke();
    }

    ItemCategory GetCategoryFromItem(ItemBase item)
    {
        if (item is RecoveryItem)
            return ItemCategory.Recoveries;
        else if (item is PokeballItem)
            return ItemCategory.Pokeball;
        else if (item is TmItem)
            return ItemCategory.TMs;
        else
            return ItemCategory.Items;
    }

    public List<ItemSlot> GetSlotsByCategory(int categoryIndex)
    {
        return allSlots[categoryIndex];
    }

    public ItemBase GetItem(int itemIndex, int categoryIndex)
    {
        var currentSlots = GetSlotsByCategory(categoryIndex);
        return currentSlots[itemIndex].Item;
    }

    public object CaptureState()
    {
        var saveSata = new InventorySaveData()
        {
            items = recoverySlots.Select(slot => slot.GetSaveData()).ToList(),
            pokeballs = pokeballSlots.Select(slot => slot.GetSaveData()).ToList(),
            tms = tmSlots.Select(slot => slot.GetSaveData()).ToList(),
        };
        return saveSata;
    }

    public void RestoreState(object state)
    {
        var saveData = state as InventorySaveData;

        recoverySlots = saveData.items.Select(i => new ItemSlot(i)).ToList();
        pokeballSlots = saveData.pokeballs.Select(i => new ItemSlot(i)).ToList();
        tmSlots = saveData.tms.Select(i => new ItemSlot(i)).ToList();

        allSlots = new List<List<ItemSlot>>()
        {
            recoverySlots,pokeballSlots,tmSlots
        };

        OnUpdated?.Invoke();
    }
}


[Serializable]
public class ItemSlot
{
    [SerializeField] ItemBase item;
    [SerializeField] int count;

    public ItemSlot()
    {
        // Must remain empty
    }

    public ItemSlot(ItemSaveData saveData)
    {
        item = ItemsDB.GetObjectByName(saveData.name);
        count = saveData.count;
    }

    public ItemSaveData GetSaveData()
    {
        var saveData = new ItemSaveData()
        {
            name = item.name,
            count = count,
        };
        return saveData;
    }

    public ItemBase Item 
    {
        get => item;
        set => item = value;
            }

    public int Count
    {
        get => count;
        set => count = value;
    }
}

public enum ItemCategory
{
    Recoveries,
    Pokeball,
    TMs,
    Items,
}

[Serializable]
public class ItemSaveData
{
    public string name;
    public int count;
}

[Serializable]
public class InventorySaveData
{
    public List<ItemSaveData> recoveries;
    public List<ItemSaveData> pokeballs;
    public List<ItemSaveData> tms;
    public List<ItemSaveData> items;
}
