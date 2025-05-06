using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonStorageBoxes : MonoBehaviour, ISavable
{
    int numBoxes = 16;
    int numSlots = 30;

    Pokemon[,] boxes = new Pokemon[16, 30];

    public void AddPokemon(Pokemon pokemon, int boxIndex, int slotIndex)
    {
        boxes[boxIndex, slotIndex] = pokemon;
    }

    public void AddPokemonToEmptySlot(Pokemon pokemon)
    {
        for (int i = 0; i < numBoxes; i++)
        {
            for (int j = 0; j < numSlots; j++)
            {
                if (boxes[i, j] == null)
                {
                    AddPokemon(pokemon, i, j);
                    return;
                }
            }
        }
    }

    public void RemovePokemon(int boxIndex, int slotIndex)
    {
        boxes[boxIndex, slotIndex] = null;
    }

    public Pokemon GetPokemon(int boxIndex, int slotIndex)
    {
        return boxes[boxIndex, slotIndex];
    }

    public static PokemonStorageBoxes GetPlayerStorageBoxes()
    {
        return FindObjectOfType<PlayerController>().GetComponent<PokemonStorageBoxes>();
    }

    public object CaptureState()
    {
        var saveData = new BoxSlotsData()
        {
            boxSlots = new List<BoxSlotData>()
        };
        
        for (int i = 0;i < numBoxes;i++)
        {
            for(int j = 0;j < numSlots;j++)
            {
                var pokemon = boxes[i, j];
                if (boxes[i, j] != null)
                {
                    var slotData = new BoxSlotData()
                    {
                        saveData = pokemon.GetSaveData(),
                        boxIndex = i,
                        slotIndex = j,
                    };
                    saveData.boxSlots.Add(slotData);
                }
            }
        }
        return saveData;
    }

    public void RestoreState(object state)
    {
        boxes = new Pokemon[16, 30];
        var data = state as BoxSlotsData;
        foreach (var pokemonData in data.boxSlots)
        {
            boxes[pokemonData.boxIndex, pokemonData.slotIndex] = new Pokemon(pokemonData.saveData);
        }
    }
}

[System.Serializable]
public class BoxSlotData
{
    public PokemonSaveData saveData;
    public int boxIndex;
    public int slotIndex;
}

[System.Serializable]
public class BoxSlotsData
{
    public List<BoxSlotData> boxSlots;
}
