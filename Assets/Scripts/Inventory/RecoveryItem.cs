using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new recovery item")]
public class RecoveryItem : ItemBase
{
    [Header("HP")]
    [SerializeField] int hpAmount;
    [SerializeField] bool restoreMaxHp;

    [Header("PP")]
    [SerializeField] int ppAmount;
    [SerializeField] bool restoreMaxPp;

    [Header("Status")]
    [SerializeField] ConditionID status;
    [SerializeField] bool recoverAllStatus;

    [Header("Revive")]
    [SerializeField] bool revive;
    [SerializeField] bool maxRevive;

    public override bool Use(Pokemon pokemon)
    {

        // Revive, Max Revive
        if (revive || maxRevive)
        {
            if (pokemon.HP > 0)
                return false;

            if (revive)
            {
                pokemon.IncreaseHP(pokemon.MaxHP / 2);
            }
            else
            {
                pokemon.IncreaseHP(pokemon.MaxHP);
            }

            pokemon.CureStatus();
            return true;
        }

        if (pokemon.HP == 0) return false;

        // Potion and others
        if (hpAmount > 0 || restoreMaxHp)
        {
            if (pokemon.HP == pokemon.MaxHP)
                return false;

            if (restoreMaxHp)
                pokemon.IncreaseHP(pokemon.MaxHP);
            else
                pokemon.IncreaseHP(hpAmount);

            return true;
        }

        // Antidote and others...
        if (recoverAllStatus || status != ConditionID.none)
        {
            if (pokemon.Status == null && pokemon.VolatileStatus == null)
                return false;

            if (recoverAllStatus)
            {
                pokemon.CureStatus();
                pokemon.CureVolatileStatus();
            }
            else
            {
                if (pokemon.Status.Id == status)
                {
                    pokemon.CureStatus();
                    return true;
                }
                else if (pokemon.VolatileStatus.Id == status)
                {
                    pokemon.CureVolatileStatus();
                    return true;
                }
                return false;
            }
        }

        // Ether and others...

        if (restoreMaxPp)
        {
            pokemon.Moves.ForEach(m => m.IncreasePp(m.Base.MaxPP));
            return true;
        }

        if (ppAmount > 0)
        {
            pokemon.Moves.ForEach(m => m.IncreasePp(ppAmount));
            return true;
        }
        return false;
    }


}
