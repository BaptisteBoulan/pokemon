using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.StateMachine;

public class BattleState : State<GameController>
{
    [SerializeField] BattleSystem battleSystem;

    public BattleTrigger Trigger { get; set; }
    public TrainerController Trainer { get; set; }

    public static BattleState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    GameController gc;

    public override void Enter(GameController owner)
    {
        gc = owner;

        battleSystem.gameObject.SetActive(true);

        var playerParty = gc.Player.GetComponent<PokemonParty>();
        var wildPokemon = gc.CurrentScene.GetComponent<MapArea>().GetComponent<MapArea>().GetRandomWildPokemon(Trigger);
        var wildPokemonCopy = new Pokemon(wildPokemon.Base, wildPokemon.Level);

        gc.WorldCamera.gameObject.SetActive(false);

        if (Trainer == null)
            battleSystem.StartBattle(playerParty, wildPokemonCopy, Trigger);
        else
        {
            var trainerParty = Trainer.GetComponent<PokemonParty>();
            battleSystem.StartTrainerBattle(playerParty,trainerParty);
        }

        battleSystem.OnBattleOver += EndBattle;
    }

    public override void Exit()
    {

        battleSystem.gameObject.SetActive(false);
        gc.WorldCamera.gameObject.SetActive(true);
        battleSystem.OnBattleOver -= EndBattle;
    }

    public override void Execute()
    {
        battleSystem.HandleUpdate();
    }

    void EndBattle(bool won)
    {
        if (Trainer != null && won)
        {
            Trainer.BattleLost();
            Trainer = null;
        }

        gc.StateMachine.Pop();
    }

    public BattleSystem BattleSystem => battleSystem;
}
