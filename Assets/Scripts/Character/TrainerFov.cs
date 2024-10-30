using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerFov : MonoBehaviour, IPlayerTriggerable
{
    public bool TriggerRepeatly => false;

    public void OnPlayerTriggered(PlayerController player)
    {
        player.Character.Animator.IsMoving = false;
        GameController.Instance.OnEnterTrainerView(GetComponentInParent<TrainerController>());
    }
}
