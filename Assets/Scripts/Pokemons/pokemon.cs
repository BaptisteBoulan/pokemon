using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Pokemon
{
    [SerializeField] PokemonBase _base;
    [SerializeField] int level;

    public Pokemon(PokemonBase pbase, int plevel)
    {
        _base = pbase;
        level = plevel;
        Init();
    }
    public PokemonBase Base { get { return _base; } }
    public int Level { get { return level; } }
    private List<Move> moves;  // Backing field for Moves
    public List<Move> Moves => moves;  // Read-only property
    public int HP { get; set; }

    public Dictionary<Stat, int> Stats { get; set; }
    public Dictionary<Stat, int> StatBoosts { get; set; }
    public Queue<string> StatusChanges { get; private set; }
    public Condition Status { get; set; }
    public int StatusTime { get; set; }


    public event Action OnStatusChanged;
    public event Action OnHpChange;
    public Condition VolatileStatus { get; set; }
    public int VolatileStatusTime { get; set; }
    public Move CurrentMove { get; set; }

    public int XP { get ; set; }


    public void Init()
    {
        // Initialize Moves
        moves = new List<Move>();

        // Generate Moves based on level
        foreach (var move in Base.LearnableMoves)
        {
            if (move.Level <= Level)
            {
                moves.Add(new Move(move.Base));
            }
            if (moves.Count >= PokemonBase.MaxMoveNumber) break;
        }

        XP = Base.GetExpForLevel(level);

        CalculateState();
        ResetStatBoosts();
        HP = MaxHP;

        Status = null;
        VolatileStatus = null;

        StatusChanges = new Queue<string>();

    }

    public PokemonSaveData GetSaveData()
    {
        var saveData = new PokemonSaveData()
        {
            name = Base.name,
            hp = HP,
            level = level,
            status = Status?.Id,
            xp = XP,
            moves = Moves.Select(m => m.GetMoveSaveData()).ToList(),
        };
        return saveData;
    }

    public Pokemon(PokemonSaveData saveData)
    {
        _base = PokemonsDB.GetObjectByName(saveData.name);
        HP = saveData.hp;
        level = saveData.level;
        XP = saveData.xp;
        if (saveData.status != null) Status = ConditionsDB.Conditions[saveData.status.Value];
        else Status = null;

        moves = saveData.moves.Select(m => new Move(m)).ToList();

        CalculateState();
        StatusChanges = new Queue<string>();
        ResetStatBoosts();
        VolatileStatus = null;
    }

    void ResetStatBoosts()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            { Stat.Attack, 0},
            { Stat.Defense, 0},
            { Stat.SpeAttack, 0},
            { Stat.SpeDefense, 0},
            { Stat.Speed, 0},

            {Stat.Accuracy, 0 },
            {Stat.Evasivness, 0},
        };
    }

    
        void CalculateState()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.Attack * Level) / 50) + 5);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((Base.Defense * Level) / 50f) + 5);
        Stats.Add(Stat.SpeAttack, Mathf.FloorToInt((Base.SpeAttack * Level) / 50f) + 5);
        Stats.Add(Stat.SpeDefense, Mathf.FloorToInt((Base.SpeDefense * Level) / 50f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.Speed * Level) / 50f) + 5);

        MaxHP = Mathf.FloorToInt((Base.MaxHP * Level) / 50) + 10 + level;
    }

    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];

        int boost = StatBoosts[stat];

        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (boost >= 0)
        {
            statVal = Mathf.FloorToInt(statVal *  boostValues[boost]);
        }
        else
        {
            statVal = Mathf.FloorToInt(statVal / boostValues[-boost]);
        }

        return statVal;
    }
public int MaxHP { get; private set; }
    public int Attack
    {
        get { return GetStat(Stat.Attack); }
    }
    public int Defense
    {
        get { return GetStat(Stat.Defense); }
    }
    public int SpeAttack
    {
        get { return GetStat(Stat.SpeAttack); }
    }
    public int SpeDefense
    {
        get { return GetStat(Stat.SpeDefense); }
    }
    public int Speed
    {
        get { return GetStat(Stat.Speed); }
    }

    public DamageDetails TakeDamage(Move move, Pokemon attacker)
    {
        

        float type = TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type1) * TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type1);


        float stab = (move.Base.Type == this.Base.Type1 || move.Base.Type == this.Base.Type2) ? 1.5f : 1f;

        float crit = 1f;
            
        if (UnityEngine.Random.value <= 0.0625f)
        {
            crit = 2f;
        }

        var details = new DamageDetails()
        {
            Type = type,
            Critical = crit,
            Fainted = false
        };

        float modifiers = UnityEngine.Random.Range(0.85f, 1f) * type * stab * crit;
        float a = (2 * attacker.Level + 10) / 250f;

        float d = (move.Base.Category == MoveCategory.Special) ? d = a * move.Base.Power * ((float)attacker.SpeAttack / SpeDefense) + 2 : d = a * move.Base.Power * ((float)attacker.Attack / Defense) + 2 ;

        int damage = Mathf.FloorToInt(d * modifiers);

        DecreaseHP(Mathf.FloorToInt(Mathf.Max(1,damage)));
        return details;
    }

    public void DecreaseHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage,0,MaxHP);
        OnHpChange?.Invoke();
    }

    public void IncreaseHP(int amount)
    {
        HP = Mathf.Clamp(HP + amount, 0, MaxHP);
        OnHpChange?.Invoke();
    }

    public Move GetRandomMove()
    {
        var movesWithPP = Moves.Where(move => move.PP > 0).ToList();
        int r = UnityEngine.Random.Range(0, movesWithPP.Count);
        return movesWithPP[r];
    }

    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach (var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost,-6,6) ;

            if (boost > 0)
            {
                StatusChanges.Enqueue($"{Base.Name}'s {stat} rose !");
            }
            else
            {
                StatusChanges.Enqueue($"{Base.Name}'s {stat} fell !");
            }

            
        }
    }
    
    public void OnBattleOver()
    {
        VolatileStatus = null;
        ResetStatBoosts();
    }

    public void SetStatus(ConditionID conditionID)
    {
        if (Status != null) { StatusChanges.Enqueue($"{Base.Name} already has the effect {Status.Name}"); return; }

        Status = ConditionsDB.Conditions[conditionID];
        Status?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {Status.StartMessage}");

        OnStatusChanged?.Invoke();
    }


    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this);
        VolatileStatus?.OnAfterTurn?.Invoke(this);
    }

    public bool OnBeforeMove()
    {
        bool canPerformMove = true;

        if (Status?.OnBeforeMove != null)
        {
            if (!Status.OnBeforeMove(this))
            {
                canPerformMove = false;
            }
        }
        if (VolatileStatus?.OnBeforeMove != null)
        {
            if (!VolatileStatus.OnBeforeMove(this))
            {
                canPerformMove = false;
            }
        }
        return canPerformMove;
    }

    public void CureStatus()
    {
        Status = null;
        OnStatusChanged?.Invoke();
    }



    public void SetVolatileStatus(ConditionID conditionID)
    {
        if (VolatileStatus != null) return;

        VolatileStatus = ConditionsDB.Conditions[conditionID];
        VolatileStatus?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {VolatileStatus.StartMessage}");
    }

    public void CureVolatileStatus()
    {
        Status = null;
    }

    public bool CheckForLevelUp()
    {
        if (XP > Base.GetExpForLevel(level+1))
        {
            level++;
            return true;
        }
        return false;
    }
    public float GetNormalizedXP()
    {
        int currentLevelXP = Base.GetExpForLevel(Level);
        int nextLevelXp = Base.GetExpForLevel(Level + 1);

        float normalizedXP = (float)(XP - currentLevelXP) / (nextLevelXp - currentLevelXP);
        return Mathf.Clamp(normalizedXP, 0f, 1f);
    }


    public LearnableMove GetLearnableMoveAtCurrentLevel()
    {
        return Base.LearnableMoves.Where(x => x.Level == level).FirstOrDefault();
    }

    public void LearnMove(MoveBase move)
    {
        if (Moves.Count >= PokemonBase.MaxMoveNumber) return; 

        Moves.Add(new Move(move));
    }

    public bool HasMove(MoveBase moveToCheck)
    {
        return Moves.Count(m => m.Base == moveToCheck) > 0;
    }

    public PokemonBase CheckForEvolution()
    {
        var evolution = Base.Evolutions.Where(e => e.Level == level).FirstOrDefault();
        return evolution?.PokemonToEvolve;
    }

    public PokemonBase CheckForEvolution(EvolutionItem item)
    {
        var evolution = Base.Evolutions.Where(e => e.RequiredItem == item).FirstOrDefault();
        return evolution?.PokemonToEvolve;
    }

    public void Evolve(PokemonBase pokemonToEvolveInto)
    {
        _base = pokemonToEvolveInto;
    }

    public void Heal()
    {
        HP = MaxHP;
        OnHpChange?.Invoke();

        foreach (var move in moves)
        {
            move.PP = move.Base.MaxPP;
        }

        CureStatus();
        CureVolatileStatus();
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float Type { get; set; }
}

[Serializable]
public class PokemonSaveData
{
    public string name;

    public int hp;
    public int level;
    public int xp;
    public ConditionID? status;

    public List<MoveSaveData> moves;
}

