using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils.StateMachine;

public class UseItemState : State<GameController>
{
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] InventoryUI inventoryUI;

    Inventory inventory;

    public static UseItemState i { get; private set; }

    private void Awake()
    {
        i = this;
        inventory = Inventory.GetInventory();
    }

    GameController gc;
    public override void Enter(GameController owner)
    {
        gc = owner;
        StartCoroutine(UseItem());
    }

    public override void Exit()
    {
    }

    public override void Execute()
    {
        partyScreen.HandleUpdate();
    }

    public IEnumerator UseItem()
    {
        var item = inventoryUI.SelectedItem;

        var pokemon = partyScreen.SelectedMember;

        var canBeUsed = inventory.UseItem(item, pokemon);

        if (item is TmItem)
        {
            yield return HandleTMsItem();
        }

        if (item is EvolutionItem)
        {
            var pokemonToEvolve = pokemon.CheckForEvolution((EvolutionItem)item);
            yield return EvolutionManager.i.Evolve(pokemon, pokemonToEvolve);
            partyScreen.SetPartyData();
        }
        else if (canBeUsed)
        {
            if (item is RecoveryItem)
                yield return DialogManager.Instance.ShowDialogText($"{item.Name} used on {pokemon.Base.Name}");
        }
        else
        {
            if (item is RecoveryItem)
                yield return DialogManager.Instance.ShowDialogText($"It won't have any effects on {partyScreen.SelectedMember.Base.Name}");
        }

        gc.StateMachine.Pop();
    }

    IEnumerator HandleTMsItem()
    {
        var tmItem = inventoryUI.SelectedItem as TmItem;

        var pokemon = partyScreen.SelectedMember;

        if (pokemon.HasMove(tmItem.Move))
        {
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} already knows {tmItem.Move.Name} !");
            yield break;
        }

        if (!tmItem.CanBeTaught(pokemon))
        {
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} can't learn {tmItem.Move.Name} !");
            yield break;
        }

        if (pokemon.Moves.Count < PokemonBase.MaxMoveNumber)
        {
            pokemon.LearnMove(tmItem.Move);
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} learned {tmItem.Move.Name} !");
        }
        else
        {
            var moveToLearn = tmItem.Move;
            // Forget a move
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} is trying to learn {moveToLearn.Name} !");
            yield return DialogManager.Instance.ShowDialogText($"But {pokemon.Base.Name} already knows four moves...");
            yield return DialogManager.Instance.ShowDialogText($"Choose a move to forget.", autoClose: false, waitForInput: false);

            MoveToForgetState.i.CurrentMoves = pokemon.Moves.Select(m => m.Base).ToList();
            MoveToForgetState.i.NewMove = moveToLearn;
            yield return gc.StateMachine.PushAndWait(MoveToForgetState.i);

            int moveIndex = MoveToForgetState.i.Selection;

            DialogManager.Instance.CloseDialog();
            if (moveIndex == PokemonBase.MaxMoveNumber || moveIndex == -1)
            {
                // dont learn new move
                yield return (DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} did not learn {moveToLearn.Name}"));
            }
            else
            {
                // forget selected move
                var forgottenMove = pokemon.Moves[moveIndex].Base.Name;
                pokemon.Moves[moveIndex] = new Move(moveToLearn);
                yield return (DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} forgot {forgottenMove} and learnt {moveToLearn.Name} !"));

            }
        }
    }
}
