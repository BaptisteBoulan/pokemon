using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountSelectorUI : MonoBehaviour
{
    [SerializeField] Text countText;
    [SerializeField] Text priceText;

    bool hasSelected;
    int selectedCount;

    int pricePerUnit;
    int maxCount;

    public IEnumerator Show(int maxCount, int pricePerUnit, Action<int> OnSelected)
    {
        hasSelected = false;
        selectedCount = 1;
        this.maxCount = maxCount;
        this.pricePerUnit = pricePerUnit;
        SetValues();

        gameObject.SetActive(true);

        yield return new WaitUntil(() => hasSelected);

        gameObject.SetActive(false);

        OnSelected?.Invoke(selectedCount);
    }

    private void Update()
    {
        var prevCount = selectedCount;
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedCount++;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedCount--;
        }

        selectedCount = (selectedCount % (maxCount + 1) + maxCount + 1) % (maxCount + 1);

        if (prevCount != selectedCount) SetValues();

        if (Input.GetKeyDown(KeyCode.Return))
        {
            hasSelected = true;
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            selectedCount = 0;
            hasSelected = true;
        }
    }

    void SetValues()
    {
        countText.text = "x" + selectedCount;
        priceText.text = "$ " + pricePerUnit*selectedCount;
    }
}
