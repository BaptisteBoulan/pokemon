using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Utils.StateMachine;

public class InventoryState : State<GameController>
{
    [SerializeField] InventoryUI inventoryUI;

    public ItemBase SelectedItem { get; set; }

    public static InventoryState i { get; private set; }

    Inventory inventory;

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
        SelectedItem=inventoryUI.SelectedItem;

        var prevState = gc.StateMachine.GetPreviousState();
        if (prevState == ShopMenuState.i)
        {
            StartCoroutine(SellAndWait());
        } 
        else 
            StartCoroutine(SelectPokemonAndUseItem());
    }

    void OnQuitInventory()
    {
        SelectedItem = null;
        if (gc.StateMachine.GetPreviousState() == ShopMenuState.i)
            StartCoroutine(ShopMenuState.i.StartMenuScreen());
        gc.StateMachine.Pop();

    }

    IEnumerator SelectPokemonAndUseItem()
    {
        var prevSate = gc.StateMachine.GetPreviousState();

        if (!SelectedItem.CanUseInBattle && prevSate == BattleState.i)
        {
            yield return DialogManager.Instance.ShowDialogText("This item cannot be used in a battle");
            yield break;
        }
        if (!SelectedItem.CanUseOutsideBattle && prevSate != BattleState.i)
        {
            yield return DialogManager.Instance.ShowDialogText("This item cannot be used outside battle");
            yield break;
        }


        if (SelectedItem is PokeballItem)
        {
            inventory.UseItem(SelectedItem, null);
            gc.StateMachine.Pop();
            yield break;
        }

        yield return gc.StateMachine.PushAndWait(PartyState.i);

        if (prevSate == BattleState.i)
        {
            if (UseItemState.i.ItemUsed)
                gc.StateMachine.Pop();
        }
    }

    IEnumerator SellAndWait()
    {
        SellingState.i.ItemToSell = SelectedItem;
        yield return gc.StateMachine.PushAndWait(SellingState.i);
    }
}
