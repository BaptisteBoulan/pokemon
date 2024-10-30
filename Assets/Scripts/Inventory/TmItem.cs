using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "Items/Create new Tm")]
public class TmItem : ItemBase
{
    [SerializeField] MoveBase move;
    [SerializeField] bool isHm;

    public override string Name => base.Name + " : " + move.Name;
    public MoveBase Move => move;
    public bool IsHm => isHm;
    public override bool Use(Pokemon pokemon)
    {
        // Handled by InventoryUI, and if the move is learned, return true.
        return pokemon.HasMove(Move);
    }

    public override bool CanUseInBattle => false;
    public override bool IsReusable => isHm;

    public bool CanBeTaught(Pokemon pokemon)
    {
        return pokemon.Base.LearnableByItems.Contains(move);
    }
}
