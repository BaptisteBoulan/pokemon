using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class LongGrass : MonoBehaviour, IPlayerTriggerable
{
    public bool TriggerRepeatly => true;

    public void OnPlayerTriggered(PlayerController player)
    {
        if (UnityEngine.Random.Range(0, 10) < 1)
        {
            player.Character.Animator.IsMoving = false;
            GameController.Instance.StartBattle(BattleTrigger.Grass);
        }
    }
}
