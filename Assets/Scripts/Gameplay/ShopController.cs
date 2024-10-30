using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using Unity.VisualScripting;
using UnityEngine;


public enum ShopState { Menu, Buying, Selling, Busy }
public class ShopController : MonoBehaviour
{
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] WalletUI walletUI;
    [SerializeField] CountSelectorUI countSelectorUI;
    [SerializeField] ShopUI shopUI;

    public event Action OnStart;
    public event Action OnClose;

    public static ShopController instance;

    ShopState state;
    Inventory inventory;

    List<ItemBase> availablesItems;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        inventory = Inventory.GetInventory();
        shopUI.OnClose += () => StartCoroutine(StartMenuScreen());
    }

    public IEnumerator StartTrading(Merchant merchant)
    {
        availablesItems = merchant.AvailablesItems;
        yield return StartMenuScreen();
    }

    public IEnumerator StartMenuScreen()
    {
        state = ShopState.Menu;
        OnStart?.Invoke();
        walletUI.Close();

        int selectedChoice = 0;

        yield return DialogManager.Instance.ShowDialogText(
                dialog: "What can I do for you ?",
                autoClose: true,
                waitForInput: false,
                choices: new List<string>() { "Buy", "Sell", "Quit" },
                onSlected: choiceIndex => selectedChoice = choiceIndex
            );

        if (selectedChoice == 0)
        {
            // Buy
            state = ShopState.Buying;
            shopUI.Show(availablesItems);
            walletUI.Show();
        }
        else if (selectedChoice == 1)
        {
            // Sell
            state = ShopState.Selling;
            inventoryUI.gameObject.SetActive(true);
            walletUI.Show();
        }
        else
        {
            // Quit
            OnClose?.Invoke();
            yield break;
        }
    }

    public void HandleUpdate()
    {
        if (state == ShopState.Selling)
        {
            // inventoryUI.HandleUpdate(OnBackFromSelling, (item) => { StartCoroutine(SellItem(item)); });
        } 
        else if (state == ShopState.Buying)
        {
            shopUI.HandleUpdate();
        }
    }

    void OnBackFromSelling()
    {
        StartCoroutine(StartMenuScreen());
        inventoryUI.gameObject.SetActive(false);
    }

    public IEnumerator SellItem(ItemBase item)
    {
        state = ShopState.Busy;

        if (item.IsSellable)
        {
            bool isSelling = false;
            int sellingPrice = item.Price / 2;
            int sellingCount = 0;
            int maxCount = inventory.GetCountFromItem(item);
            yield return countSelectorUI.Show(maxCount, sellingPrice,(count) => sellingCount = count);
            // if the player wants to sell 0 items or presses the X button
            if (sellingCount == 0)
            {
                state = ShopState.Selling;
                yield break;
            }
            yield return DialogManager.Instance.ShowDialogText($"I can give you {sellingPrice * sellingCount} pounds for these {sellingCount} {item.Name}(s)",waitForInput:false, choices: new List<string>() { "Yes", "No" }, onSlected: (choiceIndex) => isSelling = choiceIndex == 0);

            if (isSelling)
            {
                inventory.RemoveItem(item,sellingCount);
                Wallet.Instance.AddMoney(sellingPrice* sellingCount);
                yield return DialogManager.Instance.ShowDialogText($"You sold {sellingCount} {item.Name}(s) and recieved {sellingPrice * sellingCount} pounds !");
            }
        }
        else
        {
            yield return DialogManager.Instance.ShowDialogText($"You can't sell this {item.Name}");
        }
        state = ShopState.Selling;
    }

    public IEnumerator BuyItem(ItemBase item)
    {
        state = ShopState.Busy;

        bool isBuying = false;
        int buyingPrice = item.Price;
        int buyingCount = 0;
        // Max amout is the max amout the payer can pay and make sure the player has no more tha 99 items
        int maxCount = Mathf.Min(Wallet.Instance.Money / buyingPrice,99 - inventory.GetCountFromItem(item)); 
        yield return DialogManager.Instance.ShowDialogText($"It'll be {buyingPrice} pounds for one {item.Name}", waitForInput: false, autoClose: false);
        yield return countSelectorUI.Show(maxCount, buyingPrice, (count) => buyingCount = count);
        // if the player wants to sell 0 items or presses the X button
        if (buyingCount == 0)
        {
            state = ShopState.Buying;
            yield break;
        }
        yield return DialogManager.Instance.ShowDialogText($"It'll be {buyingPrice * buyingCount} pounds for these {buyingCount} {item.Name}(s)", waitForInput: false, choices: new List<string>() { "Yes", "No" }, onSlected: (choiceIndex) => isBuying = choiceIndex == 0);

        if (isBuying)
        {
            inventory.RemoveItem(item, buyingCount);
            Wallet.Instance.RemoveMoney(buyingPrice * buyingCount);
            yield return DialogManager.Instance.ShowDialogText($"You bought {buyingCount} {item.Name}(s) and paid {buyingPrice * buyingCount} pounds !");
        }
        state = ShopState.Buying;
    }
}

