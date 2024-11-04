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

    Inventory inventory;

    List<ItemBase> availablesItems;

    private void Awake()
    {
        instance = this;
    }


    public IEnumerator StartTrading(Merchant merchant)
    {
        BuyingState.i.AvailiableItems = merchant.AvailablesItems;
        ShopMenuState.i.AvailiableItems = merchant.AvailablesItems;
        GameController.Instance.StateMachine.Push(ShopMenuState.i);
        // yield return StartMenuScreen();
        yield break;
    }
}

