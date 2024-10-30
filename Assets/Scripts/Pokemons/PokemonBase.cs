using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create new Pokemon")]
public class PokemonBase : ScriptableObject
{
    [Header("Name & description")]
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [Header("Sprite")]
    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;

    [Header("Types")]
    [SerializeField] PokemonType type1;
    [SerializeField] PokemonType type2;

    [Header("Stats")]
    [SerializeField] int maxHP;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int speAttack;
    [SerializeField] int speDefense;
    [SerializeField] int speed;

    [Header("XP")]
    [SerializeField] int xpYield;
    [SerializeField] GrowthRate growthRate;


    [Header("Moves")]
    [SerializeField] List<LearnableMove> learnableMoves;
    [SerializeField] List<MoveBase> learnableByItems;


    [Header("Evolutions")]
    [SerializeField] List<Evolution> evolutions;

    [Header("Catch rate")]
    [SerializeField] int catchRate = 255;
    public static int MaxMoveNumber { get; set; } = 4;

    public string Name
    {
        get { return name; }
    }

    public string Description
    { get { return description; } }

    public Sprite FrontSprite { get {return frontSprite; }}
    public Sprite BackSprite { get { return backSprite; } }

    public PokemonType Type1 { get { return type1; } }
    public PokemonType Type2 { get { return type2; } }

    public int MaxHP { get { return maxHP; } }
    public int Attack { get { return attack; } }
    public int Defense { get {  return defense; } }
    public int SpeAttack { get { return speAttack; } }
    public float SpeDefense { get { return speDefense; } }
    public float Speed { get {  return speed; } }
    public int XpYield { get { return xpYield; } }

    public List<LearnableMove> LearnableMoves { get { return learnableMoves; }  }
    public List<MoveBase> LearnableByItems { get { return learnableByItems; }  }
    public List<Evolution> Evolutions { get { return evolutions; }  }
    public int CatchRate => catchRate;

    public int GetExpForLevel(int level)
    {
        if (growthRate == GrowthRate.Fast)
        {
            return 4 * (level * level * level) / 5;
        } else if (growthRate == GrowthRate.MediumFast)
        {
            return level * level * level;
        } else return -1;
    }
}

[System.Serializable]
public class LearnableMove
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase Base { get { return moveBase; } }
    public int Level { get { return level; } }

}
public enum PokemonType
{
    None,
    Normal,
    Water,
    Fire,
    Grass,
    Psychic,
    Dark,
    Fighting,
    Ghost,
    Steel,
    Ground,
    Rock,
    Fairy,
    Dragon,
    Poison,
    Bug,
    Flying,
    Electric,
    Ice
}
public enum Stat
{
    Attack,
    Defense,
    SpeAttack,
    SpeDefense,
    Speed,

    Accuracy,
    Evasivness,
}

public enum GrowthRate
{
    Fast, MediumFast
}
public class TypeChart
{
    public static float[][] chart =
    {
        /*                     NOR  WAT  FIR  GRA  PSY  DAR  FIG  GHO  STE  GRO  ROC  FAI  DRA  POI  BUG  FLY  ELE  ICE */
        /*NORMAL*/ new float[] {1f, 1f,  1f,  1f,  1f,  1f,  1f,  0f,  0.5f,1f,  0.5f,1f,  1f,  1f,  1f,  1f,  1f,  1f},
        /*WATER */ new float[] {1f, 0.5f,2f,  0.5f,1f,  1f,  1f,  1f,  0.5f,2f,  2f,  1f,  1f,  1f,  1f,  1f,  1f,  0.5f},
        /*FIRE  */ new float[] {1f, 0.5f,0.5f,2f,  1f,  1f,  1f,  1f,  2f,  1f,  0.5f,1f,  0.5f,1f,  2f,  1f,  1f,  2f},
        /*GRASS */ new float[] {1f, 2f,  0.5f,0.5f,1f,  1f,  1f,  1f,  0.5f,2f,  2f,  1f,  0.5f,2f,  0.5f,0.5f,1f,  1f},
        /*PSYCH*/ new float[] {1f, 1f,  1f,  1f,  0.5f,0f,  2f,  1f,  0.5f,1f,  1f,  1f,  1f,  2f,  1f,  1f,  1f,  1f},
        /*DARK  */ new float[] {1f, 1f,  1f,  1f,  2f,  0.5f,0.5f,0.5f,1f,  1f,  1f,  0.5f,1f,  1f,  1f,  1f,  1f,  1f},
        /*FIGHT */ new float[] {2f, 1f,  1f,  1f,  0.5f,2f,  1f,  0f,  2f,  1f,  0.5f,0.5f,1f,  0.5f,0.5f,0.5f,1f,  2f},
        /*GHOST */ new float[] {0f, 1f,  1f,  1f,  1f,  2f,  1f,  2f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f},
        /*STEEL */ new float[] {1f, 1f,  0.5f,1f,  1f,  1f,  1f,  1f,  0.5f,2f,  2f,  2f,  1f,  1f,  1f,  1f,  1f,  0.5f},
        /*GROUND*/ new float[] {1f, 2f,  1f,  0.5f,1f,  1f,  1f,  1f,  2f,  1f,  0.5f,1f,  1f,  2f,  0.5f,0f,  2f,  2f},
        /*ROCK  */ new float[] {1f, 2f,  2f,  1f,  1f,  1f,  2f,  1f,  0.5f,2f,  1f,  1f,  1f,  1f,  2f,  0.5f,1f,  1f},
        /*FAIRY */ new float[] {1f, 1f,  1f,  1f,  1f,  2f,  2f,  1f,  0.5f,1f,  1f,  1f,  2f,  0.5f,1f,  1f,  1f,  1f},
        /*DRAGON*/ new float[] {1f, 1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  0f,  2f,  1f,  1f,  1f,  1f,  2f},
        /*POISON*/ new float[] {1f, 1f,  1f,  2f,  1f,  1f,  1f,  0.5f,0f,  0.5f,0.5f,2f,  1f,  0.5f,1f,  1f,  1f,  1f},
        /*BUG   */ new float[] {1f, 1f,  0.5f,2f,  1f,  1f,  0.5f,0.5f,0.5f,1f,  1f,  0.5f,1f,  0.5f,1f,  0.5f,1f,  1f},
        /*FLYING*/ new float[] {1f, 1f,  1f,  2f,  1f,  1f,  2f,  1f,  0.5f,1f,  2f,  1f,  1f,  1f,  2f,  1f,  0.5f,1f},
        /*ELEC  */ new float[] {1f, 2f,  1f,  0.5f,1f,  1f,  1f,  1f,  1f,  0f,  1f,  1f,  1f,  1f,  1f,  2f,  0.5f,1f},
        /*ICE   */ new float[] {1f, 1f,  0.5f,2f,  1f,  1f,  1f,  1f,  2f,  2f,  1f,  1f,  1f,  1f,  1f,  2f,  1f,  0.5f}
    };

    public static float GetEffectiveness(PokemonType attackType, PokemonType defenseType)
    {
        if (attackType == PokemonType.None || defenseType == PokemonType.None)
            return 1f;

        return chart[(int)attackType - 1][(int)defenseType - 1];
    }
}

[System.Serializable]
public class Evolution
{
    [SerializeField] PokemonBase pokemonToEvolve;
    [SerializeField] int level;
    [SerializeField] EvolutionItem requiredItem;

    public PokemonBase PokemonToEvolve => pokemonToEvolve;
    public int Level => level;
    public EvolutionItem RequiredItem => requiredItem;
}