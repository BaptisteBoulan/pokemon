using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour, Interactable, ISavable
{
    [SerializeField] ItemBase item;
    [SerializeField] int count = 1;

    public bool Used {get;set; }

    private void Awake()
    {
        GetComponent<SpriteRenderer>().sprite = item.Icon;
    }

    public IEnumerator Interact(Transform initiator)
    {
        if (!Used)
        {
            if (count == 1)
                yield return DialogManager.Instance.ShowDialogText($"You found a {item.Name} !");
            else
                yield return DialogManager.Instance.ShowDialogText($"You found {count} {item.Name}s !");

            initiator.GetComponent<Inventory>().AddItem(item, count);

            Used = true;

            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;

            var cutScene = GetComponent<CutScene>();
            if (cutScene != null )
            {
                cutScene.OnPlayerTriggered(initiator.GetComponent<PlayerController>());
            }
        }
    }

    public object CaptureState()
    {
        return Used;
    }

    public void RestoreState(object state)
    {
        Used = (bool)state;

        GetComponent<SpriteRenderer>().enabled = !Used;
        GetComponent<BoxCollider2D>().enabled = !Used;
}
}
