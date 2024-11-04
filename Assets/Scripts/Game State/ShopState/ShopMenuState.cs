using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.StateMachine;

public class ShopMenuState : State<GameController>
{
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] WalletUI walletUI;
    [SerializeField] CountSelectorUI countSelectorUI;
    [SerializeField] ShopUI shopUI;
    public static ShopMenuState i { get; private set; }

    public List<ItemBase> AvailiableItems { get; set; }
    private void Awake()
    {
        i = this;
    }

    GameController gc;
    public override void Enter(GameController owner)
    {
        gc = owner;
        StartCoroutine(StartMenuScreen());
    }

    public IEnumerator StartMenuScreen()
    {
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
            gc.StateMachine.ChangeState(BuyingState.i);
        }
        else if (selectedChoice == 1)
        {
            // Sell
            gc.StateMachine.Push(InventoryState.i);
        }
        else
        {
            // Quit
            gc.StateMachine.Pop();
        }

    }
}
