using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.StateMachine;

public class CutSceneState : State<GameController>
{
    public static CutSceneState i { get; private set; }
    private void Awake()
    {
        i = this;
    }
    public override void Execute()
    {
        PlayerController.i.Character.HandleUpdtate();
    }
}
