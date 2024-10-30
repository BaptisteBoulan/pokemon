using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

[System.Serializable]
public class Quest 
{
    public QuestBase Base { get; private set; }
    public QuestStatus Status { get; private set; }


    public Quest(QuestBase _base)
    {
        Base = _base;
        Status = QuestStatus.None;
    }

    public IEnumerator StartQuest()
    {
        Status = QuestStatus.Started;

        yield return DialogManager.Instance.ShowDialog(Base.StartDialog);

        var questsList = QuestsList.GetQuests();
        questsList.AddQuest(this);
    }
    
    public IEnumerator CompleteQuest()
    {
        Status = QuestStatus.Completed;

        yield return DialogManager.Instance.ShowDialog(Base.CompletedDialog);

        var inventory = Inventory.GetInventory();

        if (Base.RequiredItem != null)
        {
            inventory.RemoveItem(Base.RequiredItem);
        }

        if (Base.RewardItem != null)
        {
            inventory.AddItem(Base.RewardItem);

            yield return DialogManager.Instance.ShowDialogText($"You recieved a {Base.RewardItem.Name}");
        }

        var questsList = QuestsList.GetQuests();
        questsList.AddQuest(this);
    }

    public bool CanBeCompleted()
    {
        var inventory = Inventory.GetInventory();

        if (Base.RequiredItem != null)
        {
            return inventory.HasItem(Base.RequiredItem);
        }

        return true;
    }

    public QuestSaveData GetSaveData()
    {
        var saveData = new QuestSaveData()
        {
            name = Base.name,
            status = this.Status,
        };
        return saveData;
    }

    public Quest(QuestSaveData saveData)
    {
        Base = QuestsDB.GetObjectByName(saveData.name);
        Status = saveData.status;
    }
}

[System.Serializable]
public class QuestSaveData
{
    public string name;
    public QuestStatus status;
}

public enum QuestStatus
{
    None,Started,Completed
}
