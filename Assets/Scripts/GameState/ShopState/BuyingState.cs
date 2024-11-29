using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Utils.StateMachine;

public class BuyingState : State<GameController>
{
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] WalletUI walletUI;
    [SerializeField] CountSelectorUI countSelectorUI;
    [SerializeField] ShopUI shopUI;
    public static BuyingState i { get; private set; }
    public List<ItemBase> AvailiableItems { get; set; }
    Inventory inventory;

    bool browseItems = false;

    private void Awake()
    {
        i = this;
    }

    private void Start()
    {
        inventory = Inventory.GetInventory();
    }

    GameController gc;

    public override void Enter(GameController owner)
    {
        gc = owner;
        browseItems = false;
        StartCoroutine(StartBuying());
    }

    public override void Execute()
    {
        if (browseItems)
            shopUI.HandleUpdate();
    }

    IEnumerator StartBuying()
    {
        shopUI.Show(AvailiableItems,OnBuy,OnBackFromBuying);
        walletUI.Show();
        browseItems = true;
        yield break;
    }

    public IEnumerator BuyItem(ItemBase item)
    {
        browseItems = false;
        bool isBuying = false;
        int buyingPrice = item.Price;
        int buyingCount = 0;
        // Max amout is the max amout the payer can pay and make sure the player has no more tha 99 items
        int maxCount = Mathf.Min(Wallet.Instance.Money / buyingPrice, 99 - inventory.GetCountFromItem(item));
        yield return DialogManager.Instance.ShowDialogText($"It'll be {buyingPrice} pounds for one {item.Name}", waitForInput: false, autoClose: false);
        yield return countSelectorUI.Show(maxCount, buyingPrice, (count) => buyingCount = count);
        // if the player wants to sell 0 items or presses the X button
        if (buyingCount == 0)
        {
            browseItems = true;
            yield break;
        }
        yield return DialogManager.Instance.ShowDialogText($"It'll be {buyingPrice * buyingCount} pounds for these {buyingCount} {item.Name}(s)", waitForInput: false, choices: new List<string>() { "Yes", "No" }, onSlected: (choiceIndex) => isBuying = choiceIndex == 0);

        if (isBuying)
        {
            inventory.AddItem(item, buyingCount);
            Wallet.Instance.RemoveMoney(buyingPrice * buyingCount);
            yield return DialogManager.Instance.ShowDialogText($"You bought {buyingCount} {item.Name}(s) and paid {buyingPrice * buyingCount} pounds !");
        }

        browseItems = true;
    }

    void OnBackFromBuying()
    {
        shopUI.Close();
        walletUI.Close();
        gc.StateMachine.ChangeState(ShopMenuState.i);
    }

    void OnBuy(ItemBase item)
    {
        StartCoroutine(BuyItem(item));
    }
}
