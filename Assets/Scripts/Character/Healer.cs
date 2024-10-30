using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    public IEnumerator Heal(Dialog dialog, Transform player)
    {
        int selectedChoice = 0;

        yield return DialogManager.Instance.ShowDialog(dialog, new List<string>() { "yes", "no" }, (choiceIndex) => { selectedChoice = choiceIndex; });


        if (selectedChoice == 0)
        {
            var party = player.GetComponent<PokemonParty>();

            party.Heal();

            yield return Fader.i.FadeIn(0.5f);
            yield return Fader.i.FadeOut(0.5f);

            yield return DialogManager.Instance.ShowDialogText("All your pokemons have been healed !");
            yield return DialogManager.Instance.ShowDialogText("Be safe !");
        } 
        else
        {
            yield return DialogManager.Instance.ShowDialogText("Va bien te faire foutre petite merde.");
        }
    }
}
