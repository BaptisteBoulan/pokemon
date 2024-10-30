using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SurfableWater : MonoBehaviour, Interactable, IPlayerTriggerable
{
    bool isJumpingToWater = false;

    public bool TriggerRepeatly => true;

    public IEnumerator Interact(Transform initiator)
    {
        
        var animator = initiator.GetComponent<CharacterAnimator>();
        if (animator.IsSurfing || isJumpingToWater) yield break;
        yield return DialogManager.Instance.ShowDialogText("This water is deep blue");

        var pokemonWithSurf = initiator.GetComponent<PokemonParty>().Pokemons.FirstOrDefault(p => p.Moves.Any(m => m.Base.Name == "Aqua Jet"));

        if (pokemonWithSurf != null)
        {
            int selectedChoice = 0;
            yield return DialogManager.Instance.ShowDialogText("Would you like to surf on it ?",
                choices: new List<string>() { "Yes", "No" },
                onSlected: (choiceIndex) => selectedChoice = choiceIndex,
                waitForInput: false);
            if (selectedChoice == 0)
            {
                yield return DialogManager.Instance.ShowDialogText($"{pokemonWithSurf.Base.Name} used surf !");

                var dir = new Vector3(animator.MoveX, animator.MoveY);
                var targetPos = initiator.position + dir;

                isJumpingToWater = true;
                yield return initiator.DOJump(targetPos, 0.3f, 1, 0.5f).WaitForCompletion();
                animator.IsSurfing = true;
                isJumpingToWater = true;
            }
        }
    }

    public void OnPlayerTriggered(PlayerController player)
    {
        if (UnityEngine.Random.Range(0, 10) < 1)
        {
            player.Character.Animator.IsMoving = false;
            GameController.Instance.StartBattle(BattleTrigger.Water);
        }
    }
}
