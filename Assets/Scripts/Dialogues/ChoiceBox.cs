using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceBox : MonoBehaviour
{
    [SerializeField] ChoiceText choiceTextPrefab;
    
    List<ChoiceText> choiceTexts = new List<ChoiceText>();

    bool hasSelectedChoice;
    int currentChoice = 0;

    public IEnumerator ShowChoices(List<string> choices, Action<int> onSelected)
    {
        hasSelectedChoice = false;
        gameObject.SetActive(true);

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        choiceTexts.Clear();

        foreach (string choice in choices)
        {
            var choiceText = Instantiate(choiceTextPrefab, transform);
            choiceText.TextField.text = choice;
            choiceTexts.Add(choiceText);
        }

        yield return new WaitUntil(() =>  hasSelectedChoice);

        gameObject.SetActive(false);

        onSelected?.Invoke(currentChoice);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow)) 
        {
            currentChoice++;
        } 
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentChoice--;
        }

        currentChoice = Mathf.Clamp(currentChoice, 0, choiceTexts.Count - 1);

        for (int i = 0; i < choiceTexts.Count; i++)
        {
            choiceTexts[i].SetSelected(i == currentChoice);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            hasSelectedChoice = true;
        }
    }
}
