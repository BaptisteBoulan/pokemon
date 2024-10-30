using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.StateMachine;

public class GamePartyState : State<GameController>
{
    [SerializeField] PartyScreen partyScreen;

    public static GamePartyState i { get; private set; }

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
        }
    }

    IEnumerator GoToUseItemState()
    {
        yield return gc.StateMachine.PushAndWait(UseItemState.i);
        gc.StateMachine.Pop();
    }

    void OnQuitPartyScreen()
    {
        gc.StateMachine.Pop();
    }
}

