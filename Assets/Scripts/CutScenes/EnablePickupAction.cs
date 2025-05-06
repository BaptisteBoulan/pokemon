using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class EnablePickupAction : CutSceneAction
{
    [SerializeField] bool isPokemonPickup;
    [SerializeField] GameObject obj;

    public override IEnumerator Play()
    {
        if (isPokemonPickup)
        {
            var pickup = obj.GetComponent<PokemonPickup>();
            pickup.Used = false;
            pickup.GetComponent<SpriteRenderer>().enabled = true;
            pickup.GetComponent<BoxCollider2D>().enabled = true;
        }
        else
        {
            var pickup = obj.GetComponent<Pickup>();
            pickup.Used = false;
            pickup.GetComponent<SpriteRenderer>().enabled = true;
            pickup.GetComponent<BoxCollider2D>().enabled = true;
        }

        yield break;
    }
}
