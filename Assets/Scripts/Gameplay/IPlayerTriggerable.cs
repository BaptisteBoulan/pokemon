using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerTriggerable
{
    bool TriggerRepeatly { get; }
    void OnPlayerTriggered(PlayerController player);

}
