using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] List<Pokemon> pokemons;

    PokemonStorageBoxes boxes;

    public event Action OnUpdated;

    public List<Pokemon> Pokemons
    {
        get 
        {
            return pokemons; 
        } 
        set
        {
            pokemons = value;
            OnUpdated?.Invoke();
        }
    }

    public static PokemonParty GetPlayerParty()
    {
        return FindObjectOfType<PlayerController>().GetComponent<PokemonParty>();
    }

    private void Awake()
    {
        foreach (var pokemon in pokemons)
        {
            pokemon.Init();
        }

        boxes = GetComponent<PokemonStorageBoxes>();
    }

    public Pokemon GetHealthyPokemon()
    {
        return pokemons.Where(w => w.HP > 0).FirstOrDefault();
    }

    public void AddPokemon(Pokemon pokemon)
    {
        if (pokemons.Count < 6)
        {
            pokemons.Add(pokemon);

            OnUpdated?.Invoke();
        }
        else
        { 
            boxes.AddPokemonToEmptySlot(pokemon);
            StartCoroutine(DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} was added to your PC"));
        }
    }

    public IEnumerator CheckForEvolutions()
    {
        foreach (var pkmn in pokemons)
        {
            var evolutionPokemon = pkmn.CheckForEvolution();
            if (evolutionPokemon != null)
            {
                yield return EvolutionState.i.Evolve(pkmn, evolutionPokemon);
                OnUpdated?.Invoke();
            }
        }
    }

    public void Heal()
    {
        foreach (var pokemon in pokemons)
        {
            pokemon.Heal();
        }
        OnUpdated?.Invoke();
    }

    public void PartyUpdated()
    {
        OnUpdated?.Invoke();
    }
}
