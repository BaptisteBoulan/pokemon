using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonPickup : MonoBehaviour, Interactable, ISavable
{
    [SerializeField] Pokemon pokemon;

    public bool Used { get; set; }

    private void Awake()
    {
        GetComponent<SpriteRenderer>().sprite = pokemon.Base.FrontSprite;
    }



    public IEnumerator Interact(Transform initiator)
    {
        if (!Used)
        {
            int selectedChoice = 0;
            yield return DialogManager.Instance.ShowDialogText($"Do you want to take this {pokemon.Base.Name}?",
                choices: new List<string>() { "Yes", "No" },
                onSlected: (choiceIndex) => selectedChoice = choiceIndex,
                waitForInput: false);

            if (selectedChoice == 0)
            {
                pokemon.Init();
                initiator.GetComponent<PokemonParty>().AddPokemon(pokemon);

                yield return DialogManager.Instance.ShowDialogText($"You found a {pokemon.Base.Name} !");


                Used = true;

                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<BoxCollider2D>().enabled = false;

                var cutScene = GetComponent<CutScene>();
                if (cutScene != null)
                {
                    cutScene.OnPlayerTriggered(initiator.GetComponent<PlayerController>());
                }
            }
        }
    }

    public object CaptureState()
    {
        return Used;
    }

    public void RestoreState(object state)
    {
        Debug.Log($"{pokemon.Base.Name} is used: {Used}");
        Used = (bool)state;

        GetComponent<SpriteRenderer>().enabled = !Used;
        GetComponent<BoxCollider2D>().enabled = !Used;
    }
}
