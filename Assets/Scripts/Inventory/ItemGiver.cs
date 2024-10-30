using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGiver : MonoBehaviour, ISavable
{
    [SerializeField] ItemBase item;
    [SerializeField] int count = 1;
    [SerializeField] Dialog dialog;

    bool used = false;

    public IEnumerator GiveItem(PlayerController player)
    {
        yield return DialogManager.Instance.ShowDialog(dialog);
        player.GetComponent<Inventory>().AddItem(item,count);

        used = true;
        if (count == 1)
            yield return DialogManager.Instance.ShowDialogText($"You recieved {item.Name} !");
        else
            yield return DialogManager.Instance.ShowDialogText($"You recieved {count} {item.Name}s !");
    }

    public bool CanBeGiven()
    {
        return (!used && count > 0 && item != null);
    }

    public object CaptureState()
    {
        return used;
    }

    public void RestoreState(object state)
    {
        used = (bool)state;
    }
}
