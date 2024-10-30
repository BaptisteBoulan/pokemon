using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class NPCcontroller : MonoBehaviour, Interactable, ISavable
{
    [SerializeField] Dialog dialog;

    [Header("Movements")]
    [SerializeField] List<Vector2> movementPatern;
    [SerializeField] float timeBetweenPaterns;

    [Header("Quests")]
    [SerializeField] QuestBase questToStart;
    [SerializeField] QuestBase questToComplete;

    float idleTimer = 0f;
    NPCstate state;
    int currentMovementPatern = 0;

    Character character;
    ItemGiver itemGiver;
    PokemonGiver pokemonGiver;
    Quest activeQuest;
    Healer healer;
    Merchant merchant;

    private void Awake()
    {
        character = GetComponent<Character>();
        itemGiver = GetComponent<ItemGiver>();
        pokemonGiver = GetComponent<PokemonGiver>();
        healer = GetComponent<Healer>();
        merchant = GetComponent<Merchant>();
    }
    public IEnumerator Interact(Transform initiator)
    {
        if (state == NPCstate.Idle)
        {
            state = NPCstate.Dialog;
            character.LookToward(initiator.position);

            if (questToComplete != null)
            {
                var quest = new Quest(questToComplete);
                yield return quest.CompleteQuest();
                questToComplete = null;
            }

            if (itemGiver != null && itemGiver.CanBeGiven())
            {
                yield return itemGiver.GiveItem(initiator.GetComponent<PlayerController>());
            }
            else if (pokemonGiver != null && pokemonGiver.CanBeGiven())
            {
                yield return pokemonGiver.GivePokemon(initiator.GetComponent<PlayerController>());
            }
            else if (questToStart != null)
            {
                activeQuest = new Quest(questToStart);
                yield return activeQuest.StartQuest();
                questToStart = null;
            }
            else if (activeQuest != null)
            {
                if (activeQuest.CanBeCompleted())
                {
                    yield return activeQuest.CompleteQuest();
                    activeQuest = null;
                }
                else
                {
                    yield return DialogManager.Instance.ShowDialog(activeQuest.Base.ProgresDialog);
                }
            } 
            else if (healer != null)
            {
                yield return healer.Heal(dialog, initiator);
            }
            else if (merchant != null)
            {
                yield return merchant.Trade();
            }
            else
            {
                yield return DialogManager.Instance.ShowDialog(dialog);
            }

            state = NPCstate.Idle;
            idleTimer = 0f;
        }
    }

    private void Update()
    {
        if (state == NPCstate.Idle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= timeBetweenPaterns)
            {
                idleTimer = 0f;
                if (movementPatern.Count > 0)
                    StartCoroutine(Walk());
            }
        }
        character.HandleUpdtate();
    }
    IEnumerator Walk()
    {
        state = NPCstate.Walking;

        var oldPos = transform.position;

        yield return character.Move(movementPatern[currentMovementPatern]);

        if (oldPos != transform.position)
            currentMovementPatern = (currentMovementPatern + 1) % movementPatern.Count;

        state = NPCstate.Idle;
    }

    public object CaptureState()
    {
        var saveData = new NPCsaveData();

        if (activeQuest != null)
            saveData.activeQuest = activeQuest.GetSaveData();
        if (questToStart != null)
            saveData.questToStart = (new Quest(questToStart)).GetSaveData();

        if (questToComplete != null)
            saveData.questToComplete = (new Quest(questToComplete)).GetSaveData();

        return saveData;
        
    }

    public void RestoreState(object state)
    {
        var saveData = state as NPCsaveData;
        if (saveData != null)
        {
            activeQuest = (saveData.activeQuest != null) ? new Quest(saveData.activeQuest) : null ;
            questToStart = (saveData.questToStart != null) ? (new Quest(saveData.questToStart)).Base : null;
            questToComplete = (saveData.questToComplete != null) ? (new Quest(saveData.questToComplete)).Base : null;
        }
    }
}

[System.Serializable]
public class NPCsaveData
{
    public QuestSaveData activeQuest;
    public QuestSaveData questToStart;
    public QuestSaveData questToComplete;
}



public enum NPCstate
{
    Idle,
    Walking,
    Dialog

}
