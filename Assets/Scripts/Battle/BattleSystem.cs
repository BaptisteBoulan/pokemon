using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Utils.StateMachine;
using static UnityEngine.EventSystems.EventTrigger;

public enum BattleState_
{
    Start,
    ActionSelection,
    MoveSelection,
    RunningTurn,
    PartyScreen, 
    Bag,
    BattleOver,
    AboutToUse,
    MoveToForget,
    Busy
}
public enum BattleAction
{
    Move,
    SwitchPokemon,
    UseItem,
    Run
}

public enum BattleTrigger { Grass, Water, Cave}

public class BattleSystem : MonoBehaviour
{
    // Events
    public event Action<bool> OnBattleOver;

    // Serialize Fields
    [SerializeField] UnityEngine.UI.Image background;
    [SerializeField] List<Sprite> backgroundImages;
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] UnityEngine.UI.Image playerImage;
    [SerializeField] UnityEngine.UI.Image trainerImage;
    [SerializeField] GameObject pokeballSprite;
    [SerializeField] MoveToForgetSelection moveSelectionUI;
    [SerializeField] InventoryUI inventoryUI;

    // Battle states
    BattleState_ state;
    public StateMachine<BattleSystem> StateMachine { get; private set; }

    // Menu choices
    public int CurrentAction { get; private set; }
    public int CurrentMove { get; private set; }
    bool AboutToUseChoice = true;

    MoveBase moveToLearn;

    public PokemonParty PlayerParty { get; private set; }
    public PokemonParty TrainerParty { get; private set; }
    public Pokemon WildPokemon { get; private set; }

    public bool IsTrainerBattle = false;
    PlayerController player;
    TrainerController trainer;

    public int NumberOfEscapeAttempt { get; set; }

    public BattleDialogBox DialogBox => dialogBox;

    public PlayerController Player => player;

    public BattleUnit PlayerUnit => playerUnit;
    public BattleUnit EnemyUnit => enemyUnit;
    public PartyScreen PartyScreen => partyScreen;



    public int SelectedMove { get; set; }
    public BattleAction SelectedAction { get; set; }
    public bool IsBattleOver { get; private set; }

    // Battle initialization
    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon, BattleTrigger trigger)
    {
        NumberOfEscapeAttempt = 0;
        this.PlayerParty = playerParty;
        this.WildPokemon = wildPokemon;

        IsTrainerBattle = false;

        player = playerParty.GetComponent<PlayerController>();

        background.sprite = backgroundImages[(int)trigger];

        StartCoroutine(SetupBattle());
    }
    public void StartTrainerBattle(PokemonParty playerParty, PokemonParty trainerParty)
    {
        this.PlayerParty = playerParty;
        this.TrainerParty = trainerParty;
        
        IsTrainerBattle = true;

        player = playerParty.GetComponent<PlayerController>();
        trainer = trainerParty.GetComponent<TrainerController>();

        // Will be changed
        background.sprite = backgroundImages[0];

        StartCoroutine(SetupBattle());
    }
    public IEnumerator SetupBattle()
    {
        Debug.Log("set up");
        StateMachine = new StateMachine<BattleSystem>(this);

        playerUnit.Clear();
        enemyUnit.Clear();

        if (!IsTrainerBattle)
        {
            playerUnit.Setup(PlayerParty.GetHealthyPokemon());
            enemyUnit.Setup(WildPokemon);
            yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared !");
            yield return new WaitForSeconds(0.6f);

            dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
        }
        else
        {
            playerUnit.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(false);

            playerImage.gameObject.SetActive(true);
            trainerImage.gameObject.SetActive(true);

            playerImage.sprite = player.Sprite;
            trainerImage.sprite = trainer.Sprite;

            yield return dialogBox.TypeDialog($"{trainer.Name} wants to fight you !");


            trainerImage.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(true);

            var enemyPokemon = TrainerParty.GetHealthyPokemon();
            enemyUnit.Setup(enemyPokemon);

            yield return dialogBox.TypeDialog($"{trainer.Name} sends out {enemyPokemon.Base.Name}");
            yield return new WaitForSeconds(0.6f);


            playerImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);

            var playerPokemon = PlayerParty.GetHealthyPokemon();
            playerUnit.Setup(playerPokemon);

            yield return dialogBox.TypeDialog($"GO {playerPokemon.Base.Name} !");

            dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);


        }

        partyScreen.Init();

        IsBattleOver = false;

        StateMachine.ChangeState(ActionSelectionState.i);
    }


    // Handle updates : depending on the BattleState "state", exectute the code of one of the folowing functions.

    // Handle the case of menus (action, moves, party, move to forget, do you want to switch)
    public void HandleUpdate()
    {

        StateMachine.Execute();

        if (state == BattleState_.PartyScreen)
        {
            HandlePartyScreenSelection();
        }
        else if (state == BattleState_.Bag)
        {
            Action onBack = () =>
            {
                inventoryUI.gameObject.SetActive(false);
                state = BattleState_.ActionSelection;
            };

            Action<ItemBase> onItemUsed = (ItemBase item) =>
            {
                StartCoroutine(OnItemUsed(item));
            };

            // inventoryUI.HandleUpdate(onBack, onItemUsed);
        }
        else if (state == BattleState_.AboutToUse)
        {
            HandleAboutToUse();
        }
        else if (state == BattleState_.MoveToForget)
        {
            // define a local function.
            Action<int> OnMoveSelected = (moveIndex) =>
            {
                moveSelectionUI.gameObject.SetActive(false);
                if (moveIndex == PokemonBase.MaxMoveNumber)
                {
                    // dont learn new move
                    StartCoroutine(dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} did not learn {moveToLearn.Name}"));
                }
                else
                {
                    // forget selected move
                    var forgottenMove = playerUnit.Pokemon.Moves[moveIndex].Base.Name;
                    playerUnit.Pokemon.Moves[moveIndex] = new Move(moveToLearn);
                    StartCoroutine(dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} forgot {forgottenMove} and learnt {moveToLearn.Name} !"));
                    
                }
                moveToLearn = null;
                state = BattleState_.RunningTurn;
            };

            // moveSelectionUI.HandleMoveSelection(OnMoveSelected);
        }
    }


    // Handle the case of Battlestate.PartyScreen
    public void HandlePartyScreenSelection()
    {
        Action onSelected = () =>
        {
            var selectedMember = partyScreen.SelectedMember;
            if (selectedMember.HP <= 0)
            {
                partyScreen.SetMessageText("You can't send out a fainted Pokemon to battle...");
                return;
            }
            if (selectedMember == playerUnit.Pokemon)
            {
                partyScreen.SetMessageText($"{selectedMember.Base.Name} already in battle.");
                return;
            }

            // Return makes sure that we don't deactivate partyScreen
            partyScreen.gameObject.SetActive(false);

            if (false) // partyScreen.CalledFrom == BattleState.ActionSelection) // The ennemy needs to play its turn
            {
                // StartCoroutine(RunTurns(BattleAction.SwitchPokemon));
            }
            else // Start a new turn
            {
                state = BattleState_.Busy;
                bool isTrainerAboutToUse = false; // partyScreen.CalledFrom == BattleState.AboutToUse;
                StartCoroutine(SwitchPokemon(selectedMember, isTrainerAboutToUse));
            }
        };

        Action onBack = () =>
        {
            if (playerUnit.Pokemon.HP <= 0)
            {
                partyScreen.SetMessageText("Do not try to run, you coward !");
                return;
            }

            partyScreen.gameObject.SetActive(false);
            if (false) // partyScreen.CalledFrom == BattleState.AboutToUse)
            {
                StartCoroutine(SendNextTrainerPokemon());
            }
            else ActionSelection();

            // partyScreen.CalledFrom = null;
        };


        partyScreen.HandleUpdate();
    }


    // Handle the case of Battlestate.AboutToUse
    public void HandleAboutToUse()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            AboutToUseChoice = !AboutToUseChoice;
        }
        dialogBox.UpdatechoiceBoxSelection(AboutToUseChoice);

        if(Input.GetKeyDown(KeyCode.Return))
        {
            dialogBox.EnableChoice(false);
            if (AboutToUseChoice)
            {
                OpenPartyScreen();
            } 
            else
            {
                StartCoroutine(SendNextTrainerPokemon());
            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableChoice(false);
            StartCoroutine(SendNextTrainerPokemon());
        }
    }






    // Called by HandleFaintedPokemon on a new level and learn new move and more than 4 moves
    IEnumerator ChooseMoveToForget(Pokemon pokemon, MoveBase newMove)
    {
        state = BattleState_.Busy;
        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMoveData(pokemon.Moves.Select(x => x.Base).ToList(), newMove);
        moveToLearn = newMove;
        state = BattleState_.MoveToForget;
        yield return dialogBox.TypeDialog($"Choose a move to forget.");
    }

    // if player and got other pokemons : AboutToUse : sets the BattleState.AboutToUse
    IEnumerator AboutToUse(Pokemon newPokemon)
    {
        state = BattleState_.Busy;
        yield return dialogBox.TypeDialog($"{trainer.Name} is about to use {newPokemon.Base.Name}. Do you want to change ?");
        state = BattleState_.AboutToUse;
        dialogBox.EnableChoice(true);
    }


    /* 
     * BattleOver : 
     * if the battle is over, 
     * take the victory as a bool argument, 
     * sets the BattleState.BattleOver, 
     * calls OnBattleOver for all pokemons (resets volatile status and stats boosts)
     */
    public void BattleOver(bool won)
    {
        IsBattleOver = true;
        PlayerParty.Pokemons.ForEach(pokemon => pokemon.OnBattleOver());
        playerUnit.Hud.ClearData();
        enemyUnit.Hud.ClearData();
        OnBattleOver(won);
    }





    /*
     * Called by HandleActionSelection
     * Or CheckForBattleOver
     * Or AboutToUse
     * playerParty.CalledFrom strores the value of the current Battlestate (ActionSelection, AboutToUse, RunningTurn)
     *      sets BattleState.PartyScreen
     *      initialize the partyScreen with setPartyData : takes the player party and initialize their huds
     */
    void OpenPartyScreen()
    {
        // partyScreen.CalledFrom = state;
        state = BattleState_.PartyScreen;
        partyScreen.gameObject.SetActive(true);
    }

    /*
     * Called by RunTurns : the player called switch pokemon as an action
     * Or PartyScreen : player sitch at the end of a turn (fainted pokemon or trainer about to use)
     *      1. Play animation
     *      2. Setup PlayerUnit
     *      3. partyScreen.CalledFrom (means called from about to use) => Send next trainer pokemon
     */
    IEnumerator SwitchPokemon(Pokemon newPokemon, bool isTrainerAboutToUse = false)
    {
        // If the current pokemon is still up, play come back animation
        if (playerUnit.Pokemon.HP >0)
        {
            dialogBox.EnableActionSelector(false);
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name}, come back !");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(1.5f);
        }

        // Setup
        playerUnit.Setup(newPokemon);
        dialogBox.SetMoveNames(newPokemon.Moves);
        yield return dialogBox.TypeDialog($"Go {newPokemon.Base.Name} ! I know you can do it !");

        if (false) // partyScreen.CalledFrom == BattleState.ActionSelection) 
        {
            state = BattleState_.RunningTurn; //Switch due to player action switch
        }
        else
        {
            if (isTrainerAboutToUse)
            {
                yield return SendNextTrainerPokemon(); // Switch due to trainer fainted pokemon
            }
            else
            {
                //ActionSelection(); // Switch due to player fainted pokemon

                state = BattleState_.RunningTurn;
            }
        }
    }

    // Called by SwitchPokemon, HandleAboutToUse or HandlePartyScreen : plays animation and setup pokemon
    IEnumerator SendNextTrainerPokemon()
    {
        state = BattleState_.Busy;
        var nextPokemon = TrainerParty.GetHealthyPokemon();

        enemyUnit.Setup(nextPokemon);
        yield return dialogBox.TypeDialog($"{trainer.Name} sends out {nextPokemon.Base.Name}");

        state = BattleState_.RunningTurn;
    }



    // Called by HandleActionSelection on player choose bag
    void OpenBag()
    {
        state = BattleState_.Bag;
        inventoryUI.gameObject.SetActive(true);
    }

    // Called by the UIInventory in its UseItem function
    IEnumerator OnItemUsed(ItemBase item)
    {
        state = BattleState_.Busy;
        inventoryUI.gameObject.SetActive(false);

        if (item is PokeballItem)
        {
            yield return ThrowPokeball((PokeballItem)item);
        }

        // StartCoroutine(RunTurns(BattleAction.UseItem));
    }

    // Called by RunTurns in playerAction Bag : plays animation and calls TryToCatchPokemon to compute probability to capture
    IEnumerator ThrowPokeball(PokeballItem pokeballItem)
    {
        state = BattleState_.Busy;

        if (IsTrainerBattle)
        {
            yield return dialogBox.TypeDialog("You can't capture a trainer's Pokemon !");
            state = BattleState_.RunningTurn;
            yield break;
        }

        yield return dialogBox.TypeDialog($"{player.Name} use a {pokeballItem.Name} !");

        var pokeballObject = Instantiate(pokeballSprite, playerUnit.transform.position - new Vector3(2f, 0f), Quaternion.identity);

        var pokeball = pokeballObject.GetComponent<SpriteRenderer>();

        pokeball.sprite = pokeballItem.Icon;

        //Animation

        yield return pokeball.transform.DOJump(enemyUnit.transform.position + new Vector3(0f, 2f), 2f, 1, 0.8f).WaitForCompletion();

        yield return enemyUnit.PlayCaptureAnimation();

        yield return pokeball.transform.DOMoveY(enemyUnit.transform.position.y - 1.3f, 0.4f).WaitForCompletion();

        int shakeCount = TryToCatchPokemon(enemyUnit.Pokemon, pokeballItem.CatchRateModifier);

        for (int i = 0; i < Mathf.Min(3,shakeCount); i++) 
        {
            yield return new WaitForSeconds(0.5f);
            yield return pokeball.transform.DOPunchRotation(new Vector3(0, 0, 10),0.5f).WaitForCompletion();
        }
        if (shakeCount == 4)
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} was caught !");
            yield return pokeball.DOFade(0, 1f).WaitForCompletion();

            Destroy(pokeball);
            PlayerParty.AddPokemon(enemyUnit.Pokemon);
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} was added to your party !");

            BattleOver(true);
        }
        else
        {
            yield return new WaitForSeconds(1f);
            yield return pokeball.DOFade(0, 0.2f).WaitForCompletion();
            yield return enemyUnit.PlayBreakOutAnimation();
            
            if (shakeCount < 3) yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} broke free !");
            else yield return dialogBox.TypeDialog($"Almost got it !");

            Destroy(pokeball);

            state = BattleState_.RunningTurn;
        }
    }

    // Compute the probability to capture a pokemon
    int TryToCatchPokemon(Pokemon pokemon, float catchRateModifier)
    {
        var statusBonus = ConditionsDB.GetStatusBonus(pokemon.Status);
        float a = (catchRateModifier * (3 * pokemon.MaxHP - 2 * pokemon.HP) * pokemon.Base.CatchRate * statusBonus / (3 * pokemon.MaxHP));

        if (a >= 255) return 4;

        float b = 1048560f / Mathf.Sqrt(Mathf.Sqrt(16711680f / a));
        Debug.Log(b / 653.53f);
        int shake = 0;
        while (shake < 4)
        {
            if (UnityEngine.Random.Range(0, 65535) > b)
                break;

            shake++;
        }
        return shake;
    }







    // End of RunTurns : ActionSelection : set the BattleState.ActionSelection
    void ActionSelection()
    {
        state = BattleState_.ActionSelection;
        dialogBox.SetDialog("What will you do ?");
        dialogBox.EnableActionSelector(true);
    }

    // Called by HandleActionSelection : sets BattleState.MoveSelection
    void MoveSelection()
    {
        state = BattleState_.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialog(false);
        dialogBox.EnableMoveSelector(true);
    }
}
