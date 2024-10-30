using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class ItemBase : ScriptableObject
{
    [SerializeField] string name;
    [SerializeField] string description;
    [SerializeField] Sprite icon;

    [SerializeField] int price;
    [SerializeField] bool isSellable;

    public virtual string Name => name;
    public string Description => description;
    public Sprite Icon => icon;

    public int Price => price;
    public bool IsSellable => isSellable;

    public virtual bool Use(Pokemon pokemon)
    {
        return false;
    }


    public virtual bool CanUseInBattle => true;
    public virtual bool CanUseOutsideBattle => true;
    public virtual bool IsReusable => false;

}
