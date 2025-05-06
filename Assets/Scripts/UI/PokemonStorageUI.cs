using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Utils.GenericSelectionUI;

public class PokemonStorageUI : SelectionUI<BoxSlot>
{
    [SerializeField] List<BoxSlot> boxSlots;
    [SerializeField] Image cursor;

    List<BoxPartySlotUI> partySlots = new List<BoxPartySlotUI>();
    List<BoxSlotUI> storageSlots = new List<BoxSlotUI>();
    List<Image> pokemonSprites;

    int numColoms = 7; // 6 + 1

    PokemonParty party;
    PokemonStorageBoxes boxes;

    public int SelectedBox {  get; private set; }

    private void Awake()
    {
        foreach (var slot in boxSlots)
        {
            var slotUI = slot.GetComponent<BoxSlotUI>();

            if (slotUI != null)
            {
                storageSlots.Add(slotUI);
            }
            else
            {
                partySlots.Add(slot.GetComponent<BoxPartySlotUI>());
            }
        }

        party = PokemonParty.GetPlayerParty();
        boxes = PokemonStorageBoxes.GetPlayerStorageBoxes();
        pokemonSprites = boxSlots.Select(b => b.GetComponentInChildren<Image>()).ToList();
        cursor.gameObject.SetActive(false);
    }

    private void Start()
    {
        SetItems(boxSlots);
        SetSelectionType(SelectionType.Grid, numColoms);
    }

    public void SetDataInPartySlots()
    {
        for (int i = 0; i < partySlots.Count; i++)
        {
            if (i < party.Pokemons.Count && party.Pokemons[i] != null && party.Pokemons[i].Base != null)
                partySlots[i].SetData(party.Pokemons[i]);
            else
                partySlots[i].ClearData();
        }
    }

    public void SetDataInStorageSlots()
    {
        for (int i = 0; i < storageSlots.Count; i++)
        {
            var pokemon = boxes.GetPokemon(SelectedBox, i);
            if (pokemon != null)
                storageSlots[i].SetData(pokemon);
            else
                storageSlots[i].ClearData();
        }
    }

    bool IsPartySlot(int slotIdex)
    {
        return (slotIdex % numColoms == 0);
    }

    public Pokemon TakePokemonFromSlot(int slotIdex)
    {
        Pokemon pokemon;
        if (IsPartySlot(slotIdex))
        {
            int partyIndex = slotIdex / numColoms;

            if (partyIndex >= party.Pokemons.Count)
                return null;

            pokemon = party.Pokemons[partyIndex];
            party.Pokemons[partyIndex] = null;
        }
        else
        {
            int boxSlotIndex = slotIdex - (slotIdex / numColoms + 1);

            pokemon = boxes.GetPokemon(SelectedBox, boxSlotIndex);
            boxes.RemovePokemon(SelectedBox, boxSlotIndex);
        }

        if (pokemon != null)
        {
            cursor.sprite = pokemon.Base.FrontSprite;
            cursor.gameObject.SetActive(true);
        }

        return pokemon;

    }

    public void PutPokemonIntoSlot(Pokemon pokemon, int slotIndex)
    {
        if (IsPartySlot(slotIndex))
        {
            int partyIndex = Mathf.Clamp(slotIndex/numColoms, 0, party.Pokemons.Count);
            party.Pokemons.Insert(partyIndex, pokemon);
        }
        else
        {
            int boxSlotIndex = slotIndex - (slotIndex / numColoms + 1);

            boxes.AddPokemon(pokemon ,SelectedBox, boxSlotIndex);
        }

        cursor.gameObject.SetActive(false);
    }

    public override void HandleUpdate()
    {
        base.HandleUpdate();
        cursor.transform.position = boxSlots[selectedItem].transform.position + Vector3.up * 50f;
    }

}
