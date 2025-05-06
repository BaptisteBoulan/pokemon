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
    public StateMachine<BattleSystem> StateMachine { get; private set; }

    // Menu choices
    public int CurrentAction { get; private set; }
    public int CurrentMove { get; private set; }

    MoveBase moveToLearn;

    public PokemonParty PlayerParty { get; private set; }
    public PokemonParty TrainerParty { get; private set; }
    public Pokemon WildPokemon { get; private set; }

    public bool IsTrainerBattle = false;
    PlayerController player;
    public TrainerController Trainer { get; private set; }

    public int NumberOfEscapeAttempt { get; set; }

    public BattleDialogBox DialogBox => dialogBox;

    public PlayerController Player => player;

    public BattleUnit PlayerUnit => playerUnit;
    public BattleUnit EnemyUnit => enemyUnit;
    public PartyScreen PartyScreen => partyScreen;
    public int SelectedMove { get; set; }
    public BattleAction SelectedAction { get; set; }
    public Pokemon SelectedPokemon { get; set; }
    public ItemBase SelectedItem { get; set; }
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
        Trainer = trainerParty.GetComponent<TrainerController>();

        // Will be changed
        background.sprite = backgroundImages[0];

        StartCoroutine(SetupBattle());
    }
    public IEnumerator SetupBattle()
    {
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
            trainerImage.sprite = Trainer.Sprite;

            yield return dialogBox.TypeDialog($"{Trainer.Name} wants to fight you !");


            trainerImage.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(true);

            var enemyPokemon = TrainerParty.GetHealthyPokemon();
            enemyUnit.Setup(enemyPokemon);

            yield return dialogBox.TypeDialog($"{Trainer.Name} sends out {enemyPokemon.Base.Name}");
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



    // Handle the case of menus (action, moves, party, move to forget, do you want to switch)
    public void HandleUpdate()
    {
        StateMachine.Execute();
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
     * Called by RunTurns : the player called switch pokemon as an action
     * Or PartyScreen : player sitch at the end of a turn (fainted pokemon or trainer about to use)
     *      1. Play animation
     *      2. Setup PlayerUnit
     *      3. partyScreen.CalledFrom (means called from about to use) => Send next trainer pokemon
     */
    public IEnumerator SwitchPokemon(Pokemon newPokemon)
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
    }

    // Called by SwitchPokemon, HandleAboutToUse or HandlePartyScreen : plays animation and setup pokemon
    public IEnumerator SendNextTrainerPokemon()
    {
        var nextPokemon = TrainerParty.GetHealthyPokemon();

        enemyUnit.Setup(nextPokemon);
        yield return dialogBox.TypeDialog($"{Trainer.Name} sends out {nextPokemon.Base.Name}");
    }


    // Called by the UIInventory in its UseItem function

    // Called by RunTurns in playerAction Bag : plays animation and calls TryToCatchPokemon to compute probability to capture
    public IEnumerator ThrowPokeball(PokeballItem pokeballItem)
    {
        if (IsTrainerBattle)
        {
            yield return dialogBox.TypeDialog("You can't capture a trainer's Pokemon !");
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
}
