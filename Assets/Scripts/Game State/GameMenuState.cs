using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.StateMachine;

public class GameMenuState : State<GameController>
{
    [SerializeField] MenuController menuController;

    public static GameMenuState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    GameController gc;
    public override void Enter(GameController owner)
    {
        gc = owner;
        menuController.gameObject.SetActive(true);
        menuController.OnSelected += OnMenuItemSeleted;
        menuController.OnBack += OnQuitMenu;
    }

    public override void Exit()
    {
        menuController.gameObject.SetActive(false); 
        menuController.OnSelected -= OnMenuItemSeleted;
        menuController.OnBack -= OnQuitMenu;
    }

    public override void Execute()
    {
        menuController.HandleUpdate();
    }

    void OnMenuItemSeleted(int selection)
    {
        if (selection == 0)
        {
            gc.StateMachine.Push(GamePartyState.i);
        } 
        else if (selection == 1)
        {
            gc.StateMachine.Push(InventoryState.i);
        }
        else if (selection == 2)
        {
            Debug.Log("save");
        }
        else if (selection == 3)
        {
            Debug.Log("load");
        }
        else
        {
            OnQuitMenu();
        }
    }

    void OnQuitMenu()
    {
        gc.StateMachine.Pop();
    }
}
