using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB 
{

    public static void Init()
    {
        foreach (var kvp in Conditions)
        {
            var conditionId = kvp.Key;
            var condition = kvp.Value;

            condition.Id = conditionId;
        }
    }

    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.psn, 
            new Condition()
            {
            Name = "Poison",
            StartMessage = "has been poisoned",
            OnAfterTurn =(Pokemon pokemon) =>
                {
                    pokemon.DecreaseHP(pokemon.MaxHP/8);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} hurt itself due to poison");
                }
            }
        },

         {
            ConditionID.brn,
            new Condition()
            {
            Name = "Burn",
            StartMessage = "has been burnt",
            OnAfterTurn =(Pokemon pokemon) =>
                {
                    pokemon.DecreaseHP(pokemon.MaxHP/8);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} hurt itself due to its burn");
                }
            }
        },
         {
            ConditionID.par,
            new Condition()
            {
            Name = "paralyzed",
            StartMessage = "has been paralysed",
            OnBeforeMove =(Pokemon pokemon) =>
                {
                    if (Random.Range(0,4) == 0)
                    {
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is paralyzed and unable to move");
                         return false;
                    }
                    return true;
                }
            }
        },
        {
            ConditionID.frz,
            new Condition()
            {
            Name = "freeze",
            StartMessage = "has been frozen",
            OnBeforeMove =(Pokemon pokemon) =>
                {
                    if (Random.Range(0,4) == 0)
                    {
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is not frozen anymore");
                        pokemon.CureStatus();
                        return true;
                    }
                    return false;
                }
            }
        },
        {
            ConditionID.slp,
            new Condition()
            {
            Name = "sleep",
            StartMessage = "has fallen asleep",
            OnStart = (Pokemon Pokemon) =>
                {
                    Pokemon.StatusTime = Random.Range(1,4);
                    Debug.Log($"asleep for {Pokemon.StatusTime} turns");
                },
            OnBeforeMove =(Pokemon pokemon) =>
                {
                    if (pokemon.StatusTime <= 0)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} woke up !");
                        return true;
                    }

                    pokemon.StatusTime--;
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is sleeping");
                    return false;
                }
            }
        },
        {
            ConditionID.confusion,
            new Condition()
            {
            Name = "confusion",
            StartMessage = "has been confused",
            OnStart = (Pokemon Pokemon) =>
                {
                    Pokemon.VolatileStatusTime = Random.Range(1,4);
                    Debug.Log($"confused for {Pokemon.VolatileStatusTime} turns");
                },
            OnBeforeMove =(Pokemon pokemon) =>
                {
                    if (pokemon.VolatileStatusTime < 0)
                    {
                        pokemon.CureVolatileStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is not confused anymore !");
                        return true;
                    }

                    pokemon.VolatileStatusTime--;

                    if (Random.Range(0,2) == 0) return true;

                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}is confused !");
                    pokemon.DecreaseHP(pokemon.MaxHP/8);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} hurt itself due to confusion !");
                    return false;

                }
            }
        }
    };

    public static float GetStatusBonus(Condition status)
    {
        if (status == null) return 1f;
        if (status.Id == ConditionID.slp || status.Id == ConditionID.frz) return 2f;
        else return 1.5f;
    }

}

public enum ConditionID
{
    none, psn, brn, slp, par, frz,
    confusion,
}
