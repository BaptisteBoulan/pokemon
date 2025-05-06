using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBattleAction : CutSceneAction
{
    [SerializeField] TrainerController trainer;

    public override IEnumerator Play()
    {
        yield return trainer.Interact(PlayerController.i.transform);
    }
}
