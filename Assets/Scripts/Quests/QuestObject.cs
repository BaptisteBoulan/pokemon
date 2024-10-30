using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestObject : MonoBehaviour
{
    [SerializeField] QuestBase questToCheck;
    [SerializeField] ObjectActions onStart;
    [SerializeField] ObjectActions onCompleted;

    QuestsList quests;
    

    private void Start()
    {
        quests = QuestsList.GetQuests();
        quests.OnUpdated += UpdateObjectStatus;

        UpdateObjectStatus();
    }

    private void OnDestroy()
    {
        quests.OnUpdated -= UpdateObjectStatus;
    }

    public void UpdateObjectStatus()
    {
        if (onStart != ObjectActions.DoNothing && quests.IsStarted(questToCheck.Name))
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(onStart == ObjectActions.Enable);

                var savableEntity = child.GetComponent<SavableEntity>();
                if (savableEntity != null)
                    SavingSystem.i.RestoreEntityState(savableEntity);
            }
        }

        if (onCompleted != ObjectActions.DoNothing && quests.IsCompleted(questToCheck.Name))
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(onCompleted == ObjectActions.Enable);
            }
        }
    }
}

public enum ObjectActions {  DoNothing, Enable, Disable}
