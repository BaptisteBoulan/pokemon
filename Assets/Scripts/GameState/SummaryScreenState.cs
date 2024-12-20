using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using Utils.StateMachine;

public class SummaryScreenState : State<GameController>
{
    public static SummaryScreenState i { get; private set; }

    [SerializeField] SummaryScreenUI summaryScreenUI;
    [SerializeField] GameObject skillsUI;
    [SerializeField] GameObject movesUI;

    List<Pokemon> pokemons;

    SummaryPage page = SummaryPage.Skills;

    public int SelectedPokemonIndex { get; set; } = 0;

    private void Awake()
    {
        i = this;
    }

    private void Start()
    {
        pokemons = PlayerController.i.GetComponent<PokemonParty>().Pokemons;
    }

    GameController gc;

    public override void Enter(GameController owner)
    {
        gc = owner;

        summaryScreenUI.gameObject.SetActive(true);
        summaryScreenUI.SetBasicDetails(pokemons[SelectedPokemonIndex]);
        summaryScreenUI.SetSkills();
        summaryScreenUI.SetMoves();
    }

    public override void Execute()
    {
        if (Input.GetKeyDown(KeyCode.X))
            gc.StateMachine.Pop();

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SelectedPokemonIndex = (SelectedPokemonIndex + 1) % pokemons.Count;
            summaryScreenUI.SetBasicDetails(pokemons[SelectedPokemonIndex]);
            summaryScreenUI.SetSkills();
            summaryScreenUI.SetMoves();
        }


        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SelectedPokemonIndex = (SelectedPokemonIndex + pokemons.Count - 1) % pokemons.Count;
            summaryScreenUI.SetBasicDetails(pokemons[SelectedPokemonIndex]);
            summaryScreenUI.SetSkills();
            summaryScreenUI.SetMoves();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (page == SummaryPage.Skills) 
            {
                page = SummaryPage.Moves;
                skillsUI.SetActive(false);
                movesUI.SetActive(true);
            }
            else
            {
                page = SummaryPage.Skills;
                skillsUI.SetActive(true);
                movesUI.SetActive(false);
            }
        }
    }

    public override void Exit()
    {
        summaryScreenUI.gameObject.SetActive(false);
    }
}

public enum SummaryPage { Skills, Moves }