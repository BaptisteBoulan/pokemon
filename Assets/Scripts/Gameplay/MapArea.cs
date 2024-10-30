using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<PokemonEncounterRecord> wildPokemons;
    [SerializeField] List<PokemonEncounterRecord> wildPokemonsOnWater;

    [HideInInspector]
    [SerializeField] int totalChance = 0;
    [SerializeField] int totalChanceWater = 0;
    private void OnValidate()
    {
        CalculateChancePercentage();
    }

    private void Start()
    {
        CalculateChancePercentage();
    }

    void CalculateChancePercentage()
    {
        totalChance = 0;

        foreach (var pokemonRecord in wildPokemons)
        {
            pokemonRecord.ChanceLower = totalChance;
            pokemonRecord.ChanceUpper = totalChance + pokemonRecord.changePercentage;

            totalChance = pokemonRecord.ChanceUpper;
        }

        totalChanceWater = 0;

        foreach (var pokemonRecord in wildPokemonsOnWater)
        {
            pokemonRecord.ChanceLower = totalChanceWater;
            pokemonRecord.ChanceUpper = totalChanceWater + pokemonRecord.changePercentage;

            totalChanceWater = pokemonRecord.ChanceUpper;
        }
    }

    public Pokemon GetRandomWildPokemon(BattleTrigger trigger)
    {
        int randVal = Random.Range(1, 101);
        PokemonEncounterRecord pokemonRecord = null;
        if (trigger == BattleTrigger.Grass)
            pokemonRecord = wildPokemons.Where(p => p.ChanceLower < randVal && p.ChanceUpper >= randVal).First();
        else if (trigger == BattleTrigger.Water)
            pokemonRecord = wildPokemonsOnWater.Where(p => p.ChanceLower < randVal && p.ChanceUpper >= randVal).First();

        var levelRange = pokemonRecord.levelRange;
        var level = levelRange.y <= levelRange.x ? levelRange.x : Random.Range(levelRange.x, levelRange.y + 1);
        var wildPokemon = new Pokemon(pokemonRecord.pokemon, level);
        return wildPokemon;
    }
}

[System.Serializable]
public class PokemonEncounterRecord
{
    public PokemonBase pokemon;
    public Vector2Int levelRange;
    public int changePercentage;

    public int ChanceLower { get; set; }
    public int ChanceUpper { get; set; }
}
