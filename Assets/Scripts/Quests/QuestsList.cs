using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class QuestsList : MonoBehaviour, ISavable
{
    List<Quest> quests = new List<Quest> ();

    public event Action OnUpdated;

    public void AddQuest(Quest quest)
    {
        if (quest!=null && !quests.Contains(quest))
        {
            quests.Add(quest);
        }

        OnUpdated?.Invoke();
    }

    public static QuestsList GetQuests()
    {
        return FindObjectOfType<PlayerController>().GetComponent<QuestsList>();
    }

    public bool IsStarted(string name)
    {
        var questSatus = quests.FirstOrDefault(q => q.Base.Name == name)?.Status;
        return questSatus == QuestStatus.Started || questSatus == QuestStatus.Completed;
    }

    public bool IsCompleted(string name)
    {
        
        var questSatus = quests.FirstOrDefault(q => q.Base.Name == name)?.Status;
        return questSatus == QuestStatus.Completed;
    }

    public object CaptureState()
    {
        Debug.Log("QuestList");
        return quests.Select(q => q.GetSaveData()).ToList();
    }

    public void RestoreState(object state)
    {
        var saveData = state as List<QuestSaveData>;
        if (saveData != null)
        {
            Debug.Log("QuestList");
            Debug.Log(quests.Count());
            quests = saveData.Select(q => new Quest(q)).ToList();
            OnUpdated?.Invoke();
        }
    }
}
