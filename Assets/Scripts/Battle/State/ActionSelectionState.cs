using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.GenericSelectionUI;
using Utils.StateMachine;

public class ActionSelectionState : State<BattleSystem>
{
    [SerializeField] ActionSelectionUI actionSelectionUI;

    public static ActionSelectionState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    BattleSystem bs;

    public override void Enter(BattleSystem owner)
    {
        bs = owner;
        bs.DialogBox.EnableActionSelector(true);
        bs.DialogBox.EnableDialog(true);
        actionSelectionUI.OnSelected += OnActionSelected;
        bs.DialogBox.SetDialog("What will you do ?");
    }

    public override void Exit()
    {
        bs.DialogBox.EnableActionSelector(false);
        bs.DialogBox.EnableDialog(false);
        actionSelectionUI.OnSelected -= OnActionSelected;
    }

    public override void Execute()
    {
        actionSelectionUI.HandleUpdate();
    }

    void OnActionSelected(int selection)
    {
        if (selection == 0)
        {
            // Fight
            bs.SelectedAction = BattleAction.Move;
            MoveSelectionState.i.Moves = bs.PlayerUnit.Pokemon.Moves;
            bs.StateMachine.ChangeState(MoveSelectionState.i);
        } else if (selection == 1)
        {
            // Bag
            StartCoroutine(GoToInventoryState());
        }
        else if (selection == 2)
        {
            // Pokemon
            StartCoroutine(GoToPartyState());
        }
        else if (selection == 3)
        {
            // Run
            bs.SelectedAction = BattleAction.Run;
            bs.StateMachine.ChangeState(RunTurnsState.i);
        }
    }

    IEnumerator GoToPartyState()
    {
        yield return GameController.Instance.StateMachine.PushAndWait(PartyState.i);
        var pokemon = PartyState.i.SelectedPokemon;
        if (pokemon != null)
        {
            bs.SelectedAction = BattleAction.SwitchPokemon;
            bs.SelectedPokemon = pokemon;
            bs.StateMachine.ChangeState(RunTurnsState.i);
        }
    }

    IEnumerator GoToInventoryState()
    {
        yield return GameController.Instance.StateMachine.PushAndWait(InventoryState.i);
        var item = InventoryState.i.SelectedItem;
        if (item != null)
        {
            bs.SelectedAction = BattleAction.UseItem;
            bs.SelectedItem = item;
            bs.StateMachine.ChangeState(RunTurnsState.i);
        }
    }
}
