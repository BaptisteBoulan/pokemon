using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.StateMachine;

public class MoveToForgetState : State<GameController>
{
    [SerializeField] MoveToForgetSelection moveSelectionUI;

    public List<MoveBase> CurrentMoves { get; set; }
    public MoveBase NewMove { get; set; }
    public int Selection { get; set; }

    public static MoveToForgetState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    GameController gc;

    public override void Enter(GameController owner)
    {
        gc = owner;

        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMoveData(CurrentMoves, NewMove);

        moveSelectionUI.OnSelected += OnMoveSelected;
        moveSelectionUI.OnBack += OnQuitMoveToForget;
    }

    public override void Exit()
    {
        moveSelectionUI.gameObject.SetActive(false);
        moveSelectionUI.OnSelected -= OnMoveSelected;
        moveSelectionUI.OnBack -= OnQuitMoveToForget;
    }

    public override void Execute()
    {
        moveSelectionUI.HandleUpdate();
    }

    void OnMoveSelected(int selection)
    {
        Selection = selection;
        gc.StateMachine.Pop();
    }

    void OnQuitMoveToForget()
    {
        Selection = -1;
        gc.StateMachine.Pop();
    }
}
