using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.StateMachine;

public class RunTurnsState : State<BattleSystem>
{
    public static RunTurnsState i { get; private set; }

    BattleUnit playerUnit;
    BattleUnit enemyUnit;
    PartyScreen partyScreen;
    BattleDialogBox dialogBox;
    PokemonParty playerParty;
    PokemonParty trainerParty;

    bool isTrainerBattle;


    private void Awake()
    {
        i = this;
    }

    BattleSystem bs;

    public override void Enter(BattleSystem owner)
    {
        bs = owner;

        playerUnit = bs.PlayerUnit;
        enemyUnit = bs.EnemyUnit;
        dialogBox = bs.DialogBox;
        partyScreen = bs.PartyScreen;
        isTrainerBattle = bs.IsTrainerBattle;
        playerParty = bs.PlayerParty;
        trainerParty = bs.TrainerParty;

        bs.DialogBox.EnableDialog(true);


        StartCoroutine(RunTurns(bs.SelectedAction));
    }

    public override void Exit()
    {
        
    }

    public override void Execute()
    {
        
    }

    /*
 *  Manage BattleTurns
 * 
 *  First sets BattleState to RunningTurns
 * 
 *  then depending of the parameter "player action" (BattleAction) :
 *  Move => decide who goes first, then execute RunTurn in the right order
 *  SwitchPokemon => RunTurns was executed after the choice of the pokemon
 *                => execute switchPokemon function that :
 *                  - plays the animation, 
 *                  - eventualy sends the next trainers pokemons
 *                  - sets the battleUnit and goes back to running turns
 *  UseItem => TrowPokemon
 *  Run => TryToEscape
 *  => For the last three : runTurn and runAfterTurn for the ennemy
 *  Finally if the BattleState is not Over : Go to ActionSlection (sets BattleState.ActionSlection)
 */
    IEnumerator RunTurns(BattleAction playerAction)
    {
        if (playerAction == BattleAction.Move)
        {
            playerUnit.Pokemon.CurrentMove = playerUnit.Pokemon.Moves[bs.SelectedMove];
            enemyUnit.Pokemon.CurrentMove = enemyUnit.Pokemon.GetRandomMove();

            var playerPriority = playerUnit.Pokemon.CurrentMove.Base.Priority;
            var enemyPriority = enemyUnit.Pokemon.CurrentMove.Base.Priority;

            bool playerGoesFirst = true;
            if (playerPriority <= enemyPriority)
            {
                if (playerPriority == enemyPriority)
                    playerGoesFirst = playerUnit.Pokemon.Speed >= enemyUnit.Pokemon.Speed;
                else playerGoesFirst = false;
            }

            var firstUnit = (playerGoesFirst) ? playerUnit : enemyUnit;
            var secondUnit = (playerGoesFirst) ? enemyUnit : playerUnit;

            var secondPokemon = secondUnit.Pokemon;

            //First Turn
            yield return RunMove(firstUnit, secondUnit, firstUnit.Pokemon.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if (bs.IsBattleOver) yield break;

            //Second Turn
            if (secondPokemon.HP > 0)
            {
                yield return RunMove(secondUnit, firstUnit, secondUnit.Pokemon.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (bs.IsBattleOver) yield break;
            }
        }
        else
        {
            if (playerAction == BattleAction.SwitchPokemon)
            {
                var selectedPokemon = partyScreen.SelectedMember;
                // yield return SwitchPokemon(selectedPokemon);
            }
            else if (playerAction == BattleAction.UseItem)
            {
                // Handle by inventoryUI
                dialogBox.EnableActionSelector(false);
            }
            else if (playerAction == BattleAction.Run)
            {
                yield return TryToEscape();
            }
            // Enemy Turn

            Move enemyMove = enemyUnit.Pokemon.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            yield return RunAfterTurn(enemyUnit);
            if (bs.IsBattleOver) yield break;
        }

        if (!bs.IsBattleOver)
        {
            bs.StateMachine.ChangeState(ActionSelectionState.i);
        }
    }


    // Move
    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        bool canRunMove = sourceUnit.Pokemon.OnBeforeMove();
        yield return sourceUnit.Hud.WaitForHpUpdate();

        yield return ShowStatusChange(sourceUnit.Pokemon);
        if (!canRunMove) yield break;

        move.PP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} used {move.Base.Name}");

        if (CheckIfMoveHits(move, sourceUnit.Pokemon, targetUnit.Pokemon))
        {
            sourceUnit.PlayAttackAnimation();
            yield return new WaitForSeconds(0.8f);
            targetUnit.PlayHitAnimation();

            if (move.Base.Category == MoveCategory.Status)
            {
                yield return RunMoveEffects(move.Base.MoveEffects, sourceUnit.Pokemon, targetUnit.Pokemon, move.Base.MoveTarget);

            }
            else
            {
                var damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);
                yield return targetUnit.Hud.WaitForHpUpdate();
                yield return ShowDamageDetails(damageDetails);


            }

            if (move.Base.Secondaries != null && move.Base.Secondaries.Count > 0 && targetUnit.Pokemon.HP > 0)
            {
                foreach (var secondary in move.Base.Secondaries)
                {
                    if (UnityEngine.Random.Range(0, 100) <= secondary.Chance)
                    {
                        yield return RunMoveEffects(secondary, sourceUnit.Pokemon, targetUnit.Pokemon, secondary.Target);
                    }
                }
            }

            if (targetUnit.Pokemon.HP <= 0)
            {
                yield return HandleFaintedPokemon(targetUnit);

            }
        }
        else
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name}'attack missed...");
        }
    }


    // At the very beggining : calls the status chage and displays it
    IEnumerator ShowStatusChange(Pokemon pokemon)
    {
        while (pokemon.StatusChanges.Count > 0)
        {
            var message = pokemon.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }

    // 1. Calls "OnBeforeMove" to check if the pokemon is able to attack due to status effects : retruns a bool

    // 2. Check if move hits : depending on the accuracy, retruns a boolean
    bool CheckIfMoveHits(Move move, Pokemon source, Pokemon target)
    {
        if (move.Base.AlwaysHits) return true;

        float moveAccuracy = move.Base.Accuracy;

        int accuracy = source.StatBoosts[Stat.Accuracy];
        int evasion = target.StatBoosts[Stat.Evasivness];

        var boostValues = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };

        if (accuracy >= 0)
        {
            moveAccuracy *= boostValues[accuracy];
        }
        else
        {
            moveAccuracy /= boostValues[-accuracy];
        }

        if (evasion >= 0)
        {
            moveAccuracy /= boostValues[evasion];
        }
        else
        {
            moveAccuracy *= boostValues[-evasion];
        }

        return UnityEngine.Random.Range(0, 100) < moveAccuracy;
    }

    /*
     * 3. Apply the damages : Pokemon.TakeDamage (physical and special) and RunMoveEffects for the status
     *      Calls ShowDamageDetails to display the text (no real action)
     *      RunMoveEffects firsts applies the stats boosts, then the conditions and finally the confusion
     */
    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1.5f)
        {
            yield return dialogBox.TypeDialog("A critical Hit !");
        }
        if (damageDetails.Type > 1.5f)
        {
            yield return dialogBox.TypeDialog("It's super effective !");
        }
        else if (damageDetails.Type < 0.8f)
        {
            yield return dialogBox.TypeDialog("It's not very effective...");
        }
    }
    IEnumerator RunMoveEffects(MoveEffects moveEffects, Pokemon source, Pokemon target, MoveTarget moveTarget)
    {

        var effects = moveEffects;
        if (effects.Boosts != null)
        {
            if (moveTarget == MoveTarget.Foe)
            {
                target.ApplyBoosts(effects.Boosts);
            }
            else
            {
                source.ApplyBoosts(effects.Boosts);
            }

        }

        if (effects.Status != ConditionID.none)
        {
            target.SetStatus(effects.Status);
        }

        if (effects.VolatileStatus != ConditionID.none)
        {
            target.SetVolatileStatus(effects.VolatileStatus);
        }

        yield return ShowStatusChange(source);
        yield return ShowStatusChange(target);
    }


    // 4. Apply secondary effects

    /* 
     * 5. Check if the ennemy fainted and if true, call HandleFaintedPokemon
     *      1. Play faint animation
     *      2. If it is the ennemy's : 
     *          XP gain, level up, 
     *          learn new move, 
     *          eventually calls MoveToForget that sets BattleState.MoveToForget
     *      3. If it is the player's :
     *          Calls CheckForBattleOver function :
     *              if player and other pokemons    : OpenPartyScreen
     *              if player and no other pokemons : BattleOver(false) (means defeat)
     *              if wild                         : BattleOver(true)  (means victory)
     *              if Trainer and other pokemons   : AboutToUse
     *              if trainer and no other pokemons: BattleOver(true)  (means victory)
     */
    IEnumerator HandleFaintedPokemon(BattleUnit faintedUnit)
    {
        faintedUnit.PlayFaintAnimation();
        yield return dialogBox.TypeDialog($"{faintedUnit.Pokemon.Base.Name} Fainted !");
        yield return new WaitForSeconds(1.5f);

        if (!faintedUnit.IsPlayerUnit)
        {
            //Exp Gain
            var xpYield = faintedUnit.Pokemon.Base.XpYield;
            int enemyLevel = faintedUnit.Pokemon.Level;
            float trainerBonus = (isTrainerBattle) ? 1.5f : 1f;
            int xpGain = Mathf.FloorToInt(xpYield * enemyLevel * trainerBonus / 7);

            playerUnit.Pokemon.XP += xpGain;
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} gained {xpGain} XPs !");
            yield return playerUnit.Hud.SetXPsmooth();

            //Level Up

            while (playerUnit.Pokemon.CheckForLevelUp())
            {
                playerUnit.Hud.SetLevel();
                yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} grew to level {playerUnit.Pokemon.Level} !");

                // Try to learn a new move
                var newMove = playerUnit.Pokemon.GetLearnableMoveAtCurrentLevel();

                if (newMove != null)
                {
                    if (playerUnit.Pokemon.Moves.Count < PokemonBase.MaxMoveNumber)
                    {
                        playerUnit.Pokemon.LearnMove(newMove.Base);
                        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} learned {newMove.Base.Name} !");
                        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
                    }
                    else
                    {
                        /*// Forget a move
                        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} is trying to learn {newMove.Base.Name} !");
                        yield return dialogBox.TypeDialog($"But {playerUnit.Pokemon.Base.Name} already knows four moves...");
                        yield return ChooseMoveToForget(playerUnit.Pokemon, newMove.Base);
                        yield return new WaitUntil(() => state != BattleState_.MoveToForget);
                        yield return new WaitForSeconds(1.5f);*/
                    }
                }

                yield return playerUnit.Hud.SetXPsmooth(true);
            }
            yield return new WaitForSeconds(1f);
        }

        CheckForBattleOver(faintedUnit);
    }

    /*
     * After RunTurn, RunTurns calls Run AfterTurn
     * Only if battle is not over
     * calls OnAfterTurn for the source pokemon (invoke the effect of status and volatile status)
     * eventualy calls HandleFaintedPokemon
     */
    IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if (bs.IsBattleOver) yield break;

        sourceUnit.Pokemon.OnAfterTurn();
        yield return ShowStatusChange(sourceUnit.Pokemon);
        yield return sourceUnit.Hud.WaitForHpUpdate();

        if (sourceUnit.Pokemon.HP <= 0)
        {
            yield return HandleFaintedPokemon(sourceUnit);
        }
    }   
    
    // Called by RunTurns on PlayerAction Run : compute the chances of escape
    public IEnumerator TryToEscape()
    {
        if (isTrainerBattle)
        {
            yield return dialogBox.TypeDialog("You can't run from a trainer battle !");
            yield break;
        }
        var a = playerUnit.Pokemon.Speed;
        var b = enemyUnit.Pokemon.Speed;
        bs.NumberOfEscapeAttempt++;
        if (playerUnit.Pokemon.Speed > enemyUnit.Pokemon.Speed)
        {
            yield return dialogBox.TypeDialog("You got away safely");
            bs.BattleOver(true);
        }
        else if (UnityEngine.Random.Range(0, 256) <= bs.NumberOfEscapeAttempt * (30f + a * 128f / b))
        {
            yield return dialogBox.TypeDialog("You got away safely");
            bs.BattleOver(true);
        }
        else
        {
            yield return dialogBox.TypeDialog("You failed to run...");
        }
    }

    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            var nextPokemon = bs.PlayerParty.GetHealthyPokemon();
            if (nextPokemon != null)
            {
                // bs.OpenPartyScreen();
            }
            else
            {
                bs.BattleOver(false);
            }
        }
        else
        {
            if (!bs.IsTrainerBattle)
                bs.BattleOver(true);
            else
            {
                var nextPokemon = bs.TrainerParty.GetHealthyPokemon();
                if (nextPokemon != null)
                {
                    // StartCoroutine(AboutToUse(nextPokemon));
                }
                else bs.BattleOver(true);
            }
        }
    }


}
