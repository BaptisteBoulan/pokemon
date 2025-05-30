using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new pokeball")]
public class PokeballItem : ItemBase
{
    [SerializeField] float catchRateModifier;

    public float CatchRateModifier => catchRateModifier;
    public override bool Use(Pokemon pokemon)
    {
        return true;    
    }

    public override bool CanUseOutsideBattle => false;
}
