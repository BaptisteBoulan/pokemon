using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Utils.StateMachine;

public class SellingState : State<GameController>
{
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] WalletUI walletUI;
    [SerializeField] CountSelectorUI countSelectorUI;
    [SerializeField] ShopUI shopUI;
    public static SellingState i { get; private set; }

    public ItemBase ItemToSell { get; set; }

    private void Awake()
    {
        i = this;
    }

    GameController gc;
    public override void Enter(GameController owner)
    {
        gc = owner;
        StartCoroutine(SellItem());
    }
    Inventory inventory;

    private void Start()
    {
        inventory = Inventory.GetInventory();
    }

    public IEnumerator SellItem()
    {
        if (ItemToSell.IsSellable)
        {
            bool isSelling = false;
            int sellingPrice = ItemToSell.Price / 2;
            int sellingCount = 0;
            int maxCount = inventory.GetCountFromItem(ItemToSell);
            yield return countSelectorUI.Show(maxCount, sellingPrice, (count) => sellingCount = count);
            // if the player wants to sell 0 items or presses the X button
            if (sellingCount == 0)
            {
                gc.StateMachine.Pop();
                yield break;
            }
            yield return DialogManager.Instance.ShowDialogText($"I can give you {sellingPrice * sellingCount} pounds for these {sellingCount} {ItemToSell.Name}(s)", waitForInput: false, choices: new List<string>() { "Yes", "No" }, onSlected: (choiceIndex) => isSelling = choiceIndex == 0);

            if (isSelling)
            {
                inventory.RemoveItem(ItemToSell, sellingCount);
                Wallet.Instance.AddMoney(sellingPrice * sellingCount);
                yield return DialogManager.Instance.ShowDialogText($"You sold {sellingCount} {ItemToSell.Name}(s) and recieved {sellingPrice * sellingCount} pounds !");
            }
        }
        else
        {
            yield return DialogManager.Instance.ShowDialogText($"You can't sell this {ItemToSell.Name}");
        }
        gc.StateMachine.Pop();
    }
}
