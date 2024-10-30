using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryItem : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] Dialog dialog;

    public bool TriggerRepeatly => false;

    public void OnPlayerTriggered(PlayerController player)
    {
        player.Character.Animator.IsMoving = false;
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog));
    }
}
