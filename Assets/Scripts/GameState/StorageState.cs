using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Utils.StateMachine;

public class StorageState : State<GameController>
{
    [SerializeField] PokemonStorageUI storageUI;

    public static StorageState i;

    GameController gc;

    bool isMovingPokemon = false;
    int selectedSlotToMove = 0;
    Pokemon selectedPokemonToMove;

    PokemonParty party;

    private void Awake()
    {
        i = this;
        party = PokemonParty.GetPlayerParty();
    }

    public override void Enter(GameController owner)
    {
        gc = owner;

        storageUI.gameObject.SetActive(true);

        storageUI.SetDataInPartySlots();
        storageUI.SetDataInStorageSlots();

        storageUI.OnBack += OnBack;
        storageUI.OnSelected += OnSlotSelected;
    }

    public override void Exit()
    {
        storageUI.gameObject.SetActive(false);

        storageUI.OnBack -= OnBack;
        storageUI.OnSelected -= OnSlotSelected;
    }

    public override void Execute()
    {
        storageUI.HandleUpdate();
    }

    void OnSlotSelected(int slotIndex)
    {
        if (!isMovingPokemon)
        {
            var pokemon = storageUI.TakePokemonFromSlot(slotIndex);

            if (pokemon != null)
            {
                isMovingPokemon = true;
                selectedSlotToMove = slotIndex;
                selectedPokemonToMove = pokemon;
            }
            storageUI.SetDataInPartySlots();
            storageUI.SetDataInStorageSlots();
        }
        else
        {
            isMovingPokemon = false;

            var secondPokemon = storageUI.TakePokemonFromSlot(slotIndex);


            storageUI.PutPokemonIntoSlot(selectedPokemonToMove, slotIndex);

            if (secondPokemon != null)
            {
                storageUI.PutPokemonIntoSlot(secondPokemon, selectedSlotToMove);
            }

            party.Pokemons.RemoveAll(p => p == null || p.Base == null);

            party.PartyUpdated();

            storageUI.SetDataInPartySlots();
            storageUI.SetDataInStorageSlots();

        }
    }

    void OnBack()
    {
        if (isMovingPokemon) 
        { 
            isMovingPokemon = false; 
            storageUI.PutPokemonIntoSlot(selectedPokemonToMove, selectedSlotToMove);
            party.Pokemons.RemoveAll(p => p == null || p.Base == null);
            storageUI.SetDataInPartySlots();
            storageUI.SetDataInStorageSlots();
        }
        else
        {
            gc.StateMachine.Pop();
        }
    }
}
