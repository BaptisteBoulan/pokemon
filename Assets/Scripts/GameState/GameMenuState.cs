using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        menuController.UpdateMenuItems();
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
        if (selection == 0 && menuController.HasPokemon)
        {
            gc.StateMachine.Push(PartyState.i);
        } 
        else if ((selection == 1 && menuController.HasPokemon) ||(selection == 0 && !menuController.HasPokemon))
        {
            gc.StateMachine.Push(InventoryState.i);
        }
        else if ((selection == 2 && menuController.HasPokemon) || (selection == 1 && !menuController.HasPokemon))
        {
            StartCoroutine(SaveSelected());
        }
        else if ((selection == 3 && menuController.HasPokemon) || (selection == 2 && !menuController.HasPokemon))
        {
            StartCoroutine(loadSlected());
        }
        else if (selection == 4 && menuController.HasPokemon)
        {
            gc.StateMachine.Push(StorageState.i);
        }
        else
        {
            OnQuitMenu();
        }
    }

    IEnumerator SaveSelected()
    {
        yield return Fader.i.FadeIn(0.5f);
        SavingSystem.i.Save("SaveSlot1");
        yield return Fader.i.FadeOut(0.5f);
    }

    IEnumerator loadSlected()
    {
        yield return Fader.i.FadeIn(0.5f);
        SavingSystem.i.Load("saveSlot1");
        yield return Fader.i.FadeOut(0.5f);
    }

    void OnQuitMenu()
    {
        gc.StateMachine.Pop();
    }
}
