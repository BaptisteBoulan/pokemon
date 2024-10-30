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
            bs.SelectedAction = BattleAction.Move;
            MoveSelectionState.i.Moves = bs.PlayerUnit.Pokemon.Moves;
            bs.StateMachine.ChangeState(MoveSelectionState.i);
        } else if (selection == 1)
        {
            bs.SelectedAction = BattleAction.SwitchPokemon;
        }
        else if (selection == 2)
        {
            bs.SelectedAction = BattleAction.UseItem;
        }
        else if (selection == 3)
        {
            bs.SelectedAction = BattleAction.Run;
            bs.StateMachine.ChangeState(RunTurnsState.i);
        }
    }
}
