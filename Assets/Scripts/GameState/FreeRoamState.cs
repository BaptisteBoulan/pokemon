using Utils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeRoamState : State<GameController>
{
    public static FreeRoamState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    GameController gc;
    public override void Enter(GameController owner) 
    {
        gc = owner;
    }

    public override void Exit() { }

    public override void Execute()
    {
        PlayerController.i.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            gc.StateMachine.Push(GameMenuState.i);
        }
    }
}
