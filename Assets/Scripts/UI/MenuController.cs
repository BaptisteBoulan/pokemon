using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Utils.GenericSelectionUI;

public class MenuController : SelectionUI<TextSlot>
{
    [SerializeField] TextSlot pokemonsSlot;
    [SerializeField] TextSlot boxesSlot;

    public bool HasPokemon = false;
    private void Awake()
    {
        pokemonsSlot.gameObject.SetActive(false);
        boxesSlot.gameObject.SetActive(false);
        SetSelectionType(SelectionType.List, 1); // not mandatory
        SetItems(GetComponentsInChildren<TextSlot>().ToList());
    }

    public void UpdateMenuItems()
    {
        if (!HasPokemon && PokemonParty.GetPlayerParty().Pokemons.Count > 0)
        {
            HasPokemon = true;

            pokemonsSlot.gameObject.SetActive(true);
            pokemonsSlot.Init();

            boxesSlot.gameObject.SetActive(true);
            boxesSlot.Init();

            Items = GetComponentsInChildren<TextSlot>().ToList();
            UpdateSelectionUI();
        }
    }
}
