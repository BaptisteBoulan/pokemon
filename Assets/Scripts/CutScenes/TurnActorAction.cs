using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnActorAction : CutSceneAction
{
    [SerializeField] CutSceneActor actor;
    [SerializeField] FacingDirection direction;

    public override IEnumerator Play()
    {
        actor.GetCharacter().Animator.SetFacingDirection(direction);
        yield break;
    }
}
