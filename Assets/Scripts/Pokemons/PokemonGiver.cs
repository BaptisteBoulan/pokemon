using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonGiver : MonoBehaviour, ISavable
{
    [SerializeField] Pokemon pokemon;
    [SerializeField] Dialog dialog;

    bool used = false;

    public IEnumerator GivePokemon(PlayerController player)
    {
        yield return DialogManager.Instance.ShowDialog(dialog);
        pokemon.Init();
        player.GetComponent<PokemonParty>().AddPokemon(pokemon);

        used = true;
        yield return DialogManager.Instance.ShowDialogText($"You recieved {pokemon.Base.Name} !");
    }

    public bool CanBeGiven()
    {
        return (!used && pokemon != null && pokemon.Base != null);
    }

    public object CaptureState()
    {
        return used;
    }

    public void RestoreState(object state)
    {
        used = (bool)state;
    }
}
