using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Utils.GenericSelectionUI;

public class MenuController : SelectionUI<TextSlot>
{
    private void Start()
    {
        SetSelectionType(SelectionType.List, 1); // not mandatory
        SetItems(GetComponentsInChildren<TextSlot>().ToList());
    }
}
