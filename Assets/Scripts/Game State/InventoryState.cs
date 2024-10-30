using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.StateMachine;

public class InventoryState : State<GameController>
{
    [SerializeField] InventoryUI inventoryUI;

    public static InventoryState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    GameController gc;
    public override void Enter(GameController owner)
    {
        gc = owner;
        inventoryUI.gameObject.SetActive(true);

        inventoryUI.OnSelected += OnItemSelected;
        inventoryUI.OnBack += OnQuitInventory;
    }

    public override void Exit()
    {
        inventoryUI.gameObject.SetActive(false);

        inventoryUI.OnSelected -= OnItemSelected;
        inventoryUI.OnBack -= OnQuitInventory;
    }

    public override void Execute()
    {
        inventoryUI.HandleUpdate();
    }

    void OnItemSelected(int selection)
    {
        gc.StateMachine.Push(GamePartyState.i);
    }

    void OnQuitInventory()
    {
        gc.StateMachine.Pop();
    }
}
