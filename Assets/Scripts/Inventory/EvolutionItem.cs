using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "Items/Create new Evolution Item")]
public class EvolutionItem : ItemBase
{
    public override bool Use(Pokemon pokemon)
    {
        return true;
    }

    public override bool CanUseInBattle => false;
}
