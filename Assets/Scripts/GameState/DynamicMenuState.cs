using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using Utils.StateMachine;

public class DynamicMenuState : State<GameController>
{
    [SerializeField] DynamicMenuUI menuUI;
    [SerializeField] TextSlot itemTextPrefab;
    public static DynamicMenuState i { get; private set; }

    public List<string> MenuItems {get;set;}

    public int? SelectedItem { get; private set; }

    private void Awake()
    {
        i = this;
    }

    GameController gc;

    public override void Enter(GameController owner)
    {
        gc = owner;

        foreach (Transform child in menuUI.transform)
        {
            Destroy(child.gameObject);
        }

        var itemTextSlots = new List<TextSlot>();
        foreach (var text in MenuItems)
        {
            var item = Instantiate(itemTextPrefab,menuUI.transform);
            item.SetText(text);
            itemTextSlots.Add(item);
        }

        menuUI.SetItems(itemTextSlots);

        menuUI.gameObject.SetActive(true);
        menuUI.OnSelected += OnItemSelected;
        menuUI.OnBack += OnBack;
    }

    public override void Execute()
    {
        menuUI.HandleUpdate();
    }

    public override void Exit()
    {
        menuUI.ClearItems();
        menuUI.gameObject.SetActive(false);
        menuUI.OnSelected -= OnItemSelected;
        menuUI.OnBack -= OnBack;
    }

    void OnItemSelected(int selection)
    {
        SelectedItem = selection;
        gc.StateMachine.Pop();
    }

    void OnBack()
    {
        SelectedItem = null;
        gc.StateMachine.Pop();
    }
}
