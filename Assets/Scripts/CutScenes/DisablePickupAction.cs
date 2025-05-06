using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisablePickupAction : CutSceneAction
{
    [SerializeField] bool isPokemonPickup;
    [SerializeField] GameObject obj;

    public override IEnumerator Play()
    {
        if (isPokemonPickup)
        {
            var pickup = obj.GetComponent<PokemonPickup>();
            pickup.Used = true;
            pickup.GetComponent<SpriteRenderer>().enabled = false;
            pickup.GetComponent<BoxCollider2D>().enabled = false;
        }
        else
        {
            var pickup = obj.GetComponent<Pickup>();
            pickup.Used = true;
            pickup.GetComponent<SpriteRenderer>().enabled = false;
            pickup.GetComponent<BoxCollider2D>().enabled = false;
        }

        yield break;
    }
}
