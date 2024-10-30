using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils.GenericSelectionUI;


public class ActionSelectionUI : SelectionUI<TextSlot>
{
    private void Start()
    {
        SetSelectionType(SelectionType.Grid, 2);
        SetItems(GetComponentsInChildren<TextSlot>().ToList());
    }
}
