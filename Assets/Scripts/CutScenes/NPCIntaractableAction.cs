using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCIntaractableAction : CutSceneAction
{
    [SerializeField] NPCcontroller npc;

    public override IEnumerator Play()
    {
        yield return npc.Interact(PlayerController.i.transform);
    }
}
