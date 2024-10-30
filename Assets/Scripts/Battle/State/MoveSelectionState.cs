using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.StateMachine;

public class MoveSelectionState : State<BattleSystem>
{
    [SerializeField] MoveSelectionUI moveSelectionUI;
    [SerializeField] GameObject moveDetails;

    public List<Move> Moves { get; set; }

    public static MoveSelectionState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    BattleSystem bs;

    public override void Enter(BattleSystem owner)
    {
        moveSelectionUI.SetMoves(Moves);
        bs = owner;
        moveSelectionUI.OnSelected += OnMoveSelected;
        moveSelectionUI.OnBack += OnBack;

        bs.DialogBox.EnableMoveSelector(true);
    }

    public override void Exit()
    {
        moveSelectionUI.OnSelected -= OnMoveSelected;
        moveSelectionUI.OnBack -= OnBack;

        bs.DialogBox.EnableMoveSelector(false);
    }

    public override void Execute()
    {
        moveSelectionUI.HandleUpdate();
    }

    void OnMoveSelected(int selection)
    {
        bs.SelectedMove = selection;
        bs.StateMachine.ChangeState(RunTurnsState.i);
    }

    void OnBack()
    {
        bs.StateMachine.ChangeState(ActionSelectionState.i);
    }
}
