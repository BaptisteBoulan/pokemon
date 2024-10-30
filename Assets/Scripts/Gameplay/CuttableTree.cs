using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CuttableTree : MonoBehaviour, Interactable
{
    public IEnumerator Interact(Transform initiator)
    {
        yield return DialogManager.Instance.ShowDialogText("This tree looks like it could be cut...");

        var pokemonWithCut = initiator.GetComponent<PokemonParty>().Pokemons.FirstOrDefault(p => p.Moves.Any(m => m.Base.Name == "Ember"));

        if (pokemonWithCut != null)
            {
                int selectedChoice = 0;
                yield return DialogManager.Instance.ShowDialogText("Would you like to cut it ?", 
                    choices: new List<string>() { "Yes","No"},
                    onSlected: (choiceIndex) => selectedChoice = choiceIndex,
                    waitForInput: false);
                if (selectedChoice == 0)
                    {
                        yield return DialogManager.Instance.ShowDialogText($"{pokemonWithCut.Base.Name} used cut !");
                        gameObject.SetActive(false);
                    }
            }
        }
}
