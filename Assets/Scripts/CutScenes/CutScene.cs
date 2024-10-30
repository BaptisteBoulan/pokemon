using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CutScene : MonoBehaviour, IPlayerTriggerable
{
    [SerializeReference]
    [SerializeField] List<CutSceneAction> actions = new List<CutSceneAction>();

    public bool TriggerRepeatly => false;

    public void AddAction(CutSceneAction action)
    {

#if UNITY_EDITOR
        Undo.RegisterCompleteObjectUndo(this, "Add action to cut scene");
#endif

        action.Name = action.GetType().ToString();
        actions.Add(action);
    }

    public void OnPlayerTriggered(PlayerController player)
    {
        player.Character.Animator.IsMoving = false;
        StartCoroutine(Play());
    }

    public IEnumerator Play()
    {
        GameController.Instance.StartCutScene();

        foreach (var action in actions)
        {
            if (action.WaitForCompletion)
                yield return action.Play();
            else
                StartCoroutine(action.Play());
        }

        GameController.Instance.StartFreeRoam();
    }
}
