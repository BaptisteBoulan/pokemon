using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MoveActorAction : CutSceneAction
{
    [SerializeField] CutSceneActor actor;
    [SerializeField] List<Vector2> movePatern;

    public override IEnumerator Play()
    {
        var character = actor.GetCharacter();
        foreach (var moveVec in movePatern)
        {
            yield return character.Move(moveVec, checkForCollision: false);
        }
    }
}

[System.Serializable]
public class CutSceneActor
{
    [SerializeField] bool isPlayer;
    [SerializeField] Character character;

    public Character GetCharacter() => isPlayer ? PlayerController.i.Character : character;
}