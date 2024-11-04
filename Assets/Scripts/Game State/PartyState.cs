using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.StateMachine;

public class PartyState : State<GameController>
{
    [SerializeField] PartyScreen partyScreen;

    public Pokemon SelectedPokemon;

    public static PartyState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    GameController gc;
    public override void Enter(GameController owner)
    {
        gc = owner;
        partyScreen.gameObject.SetActive(true);

        partyScreen.OnSelected += OnPokemonSelected;
        partyScreen.OnBack += OnQuitPartyScreen;

        SelectedPokemon = null;
    }

    public override void Exit() 
    { 
        partyScreen.gameObject.SetActive(false);

        partyScreen.OnSelected -= OnPokemonSelected;
        partyScreen.OnBack -= OnQuitPartyScreen;
    }

    public override void Execute()
    {
        partyScreen.HandleUpdate();
    }

    void OnPokemonSelected(int selection)
    {
        var prevState = gc.StateMachine.GetPreviousState();

        if (prevState == InventoryState.i)
        {
            // use item
            StartCoroutine(GoToUseItemState());
        } 
        else if (prevState == GameMenuState.i)
        {
            // show summary
            Debug.Log("summary " + selection);
        } else if (prevState == BattleState.i)
        {
            var battleState = prevState as BattleState;
            SelectedPokemon = partyScreen.SelectedMember;
            if (SelectedPokemon.HP <= 0)
            {
                partyScreen.SetMessageText("You can't send out a fainted Pokemon to battle...");
                return;
            }
            if (SelectedPokemon == battleState.BattleSystem.PlayerUnit.Pokemon)
            {
                partyScreen.SetMessageText($"{SelectedPokemon.Base.Name} already in battle.");
                return;
            }

            gc.StateMachine.Pop();
        }
    }

    IEnumerator GoToUseItemState()
    {
        yield return gc.StateMachine.PushAndWait(UseItemState.i);
        gc.StateMachine.Pop();
    }

    void OnQuitPartyScreen()
    {
        var prevState = gc.StateMachine.GetPreviousState();

        SelectedPokemon = null;

        if (prevState == BattleState.i)
        {
            var battleState = prevState as BattleState;
            if (battleState.BattleSystem.PlayerUnit.Pokemon.HP <= 0)
            {
                partyScreen.SetMessageText("Do not try to run, you coward !");
                return;
            }
        }
        gc.StateMachine.Pop();
    }
}

