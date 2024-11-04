using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.StateMachine;

public class AboutToUseState : State<BattleSystem>
{
    public static AboutToUseState i { get; private set; }

    public Pokemon NewPokemon { get; set; }

    private void Awake()
    {
        i = this;
    }

    BattleSystem bs;
    bool aboutToUseChoice;

    public override void Enter(BattleSystem owner)
    {
        bs = owner;
        StartCoroutine(StartState());
    }

    IEnumerator StartState()
    {
        yield return bs.DialogBox.TypeDialog($"{bs.Trainer.Name} is about to use {NewPokemon.Base.Name}. Do you want to change ?");
        bs.DialogBox.EnableChoice(true);
    }

    public override void Execute()
    {
        if (!bs.DialogBox.IsChoiceBoxEnabled) return;

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            aboutToUseChoice = !aboutToUseChoice;
        }
        bs.DialogBox.UpdatechoiceBoxSelection(aboutToUseChoice);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            bs.DialogBox.EnableChoice(false);
            if (aboutToUseChoice)
            {
                StartCoroutine(SwitchAndContinueBattle());
            }
            else
            {
                StartCoroutine(ContinueBattle());
            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            bs.DialogBox.EnableChoice(false);
            StartCoroutine(ContinueBattle());
        }
    }

    IEnumerator ContinueBattle()
    {
        yield return bs.SendNextTrainerPokemon();
        bs.StateMachine.Pop();
    }

    IEnumerator SwitchAndContinueBattle()
    {
        yield return GameController.Instance.StateMachine.PushAndWait(PartyState.i);
        var selectedPokemon = PartyState.i.SelectedPokemon; 
        if (selectedPokemon != null)
        {
            yield return bs.SwitchPokemon(selectedPokemon);
            bs.StateMachine.Pop();
        }

        yield return ContinueBattle();
    }
}
