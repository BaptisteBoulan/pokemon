using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.StateMachine;


public enum GameState { FreeRoam, Battle, Dialog, Cutscene, Paused, Menu, PartyScreen, Bag, Evolution, Shop }
public class GameController : MonoBehaviour
{
    [SerializeField] GameState state;
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] Camera worldCamera;


    public SceneDetails CurrentScene { get; private set; }
    public SceneDetails PrevScene { get; private set; }

    GameState prevState;
    public GameState State
    {
        get => state;
        set => state = value;
    }
    public PlayerController Player => playerController;
    public Camera WorldCamera => worldCamera;

    public StateMachine<GameController> StateMachine { get; private set; }

    private TrainerController trainer;

    public static GameController Instance { get; private set; }

    private void Awake()
    {
        ConditionsDB.Init();
        PokemonsDB.Init();
        MovesDB.Init();
        ItemsDB.Init();
        QuestsDB.Init();
        Instance = this;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Start()
    {
        StateMachine = new StateMachine<GameController>(this);
        StateMachine.ChangeState(FreeRoamState.i);

        DialogManager.Instance.OnShowDialog += () =>
        {
            prevState = state;
            state = GameState.Dialog;
        };

        DialogManager.Instance.OnCloseDialog += () =>
        {
            if (state == GameState.Dialog)
                state = prevState;
        };

        ShopController.instance.OnStart += () => state = GameState.Shop;
        ShopController.instance.OnClose += () => state = GameState.FreeRoam;

        battleSystem.OnBattleOver += EndBattle;

        partyScreen.Init();
    }

    public void StartBattle(BattleTrigger trigger)
    {
        BattleState.i.Trigger = trigger;
        StateMachine.Push(BattleState.i);
    }

    public void OnEnterTrainerView(TrainerController trainer)
    {
        state = GameState.Cutscene;
        StartCoroutine(trainer.TriggerTrainerBattle(playerController));
    }

    public void StartTrainerBattle(TrainerController trainer)
    {
        BattleState.i.Trainer = trainer;
        StateMachine.Push(BattleState.i);
    }


    void EndBattle(bool won)
    {
        StartCoroutine(playerController.GetComponent<PokemonParty>().CheckForEvolutions());
    }
    private void Update()
    {
        StateMachine.Execute();

        if (state == GameState.Cutscene)
        {
            playerController.Character.HandleUpdtate();
        }
        else if (state == GameState.Dialog)
        {
            DialogManager.Instance.handleUpdate();
        }
        else if (state == GameState.Shop)
        {
            ShopController.instance.HandleUpdate();
        }

    }
    public void PauseGame(bool pause)
    {
        if (pause)
        {
            prevState = state;
            state = GameState.Paused;
        }
        else
        {
            state = prevState;
        }
    }

    public void SetCurrentScene(SceneDetails scene)
    {
        PrevScene = CurrentScene;
        CurrentScene = scene;
    }

    void OnMenuSelected(int selected)
    {
        if (selected == 0)
        {
            // pokmeon
            partyScreen.gameObject.SetActive(true); 
            state = GameState.PartyScreen;
        } 
        else if (selected == 1)
        {
            // bag
            inventoryUI.gameObject.SetActive(true);
            state = GameState.Bag;
        }
        else if (selected == 2)
        {
            // Save
            SavingSystem.i.Save("SaveSlot1");
            state = GameState.FreeRoam;
        }
        else if (selected == 3)
        {
            // Load
            SavingSystem.i.Load("SaveSlot1");
            state = GameState.FreeRoam;
        }
        else if (selected == 4)
        {
            // Exit
            Application.Quit();
        }
    }

    public void StartCutScene()
    {
        state = GameState.Cutscene;
    }

    public void StartFreeRoam()
    {
        state = GameState.FreeRoam;
    }

    public void OnGUI()
    {
        var style = new GUIStyle();
        style.fontSize = 24;
        style.normal.textColor = Color.white;
        style.fontStyle = FontStyle.Bold;

        GUILayout.Label("STATE STACK",style);

        foreach (var state in StateMachine.StateStack)
        {
            GUILayout.Label(state.GetType().ToString(),style);
        }
    }

}
