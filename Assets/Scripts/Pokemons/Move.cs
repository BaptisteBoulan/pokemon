using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move 
{
    public MoveBase Base { get; set; }
    public int PP { get; set; }

    public Move(MoveBase pBase)
    {
        Base = pBase;
        PP = Base.MaxPP;
    }

    public MoveSaveData GetMoveSaveData()
    {
        var saveData = new MoveSaveData
        {
            name = Base.name,
            pp = PP,
        };
        return saveData;
    }

    public Move(MoveSaveData saveData)
    {
        Base = MovesDB.GetObjectByName(saveData.name);
        PP = saveData.pp;
    }

    public void IncreasePp(int amount)
    {
        PP = Mathf.Clamp(PP + amount, 0, Base.MaxPP);
    }

}

[Serializable]
public class MoveSaveData
{
    public string name;
    public int pp;
}
