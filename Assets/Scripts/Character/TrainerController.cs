using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour, Interactable, ISavable
{
    [SerializeField] GameObject exclamtion;
    [SerializeField] Dialog dialog;
    [SerializeField] Dialog dialogAfterBattle;
    [SerializeField] GameObject fov;
    [SerializeField] Sprite sprite;
    [SerializeField] string name;

    Character character;
    bool battleLost = false;


    public Sprite Sprite { get => sprite; }
    public string Name {  get { return name;}}
    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        SetFovRotation(character.Animator.FacingDirection);
    }

    private void Update()
    {
        character.HandleUpdtate();
    }

    public IEnumerator TriggerTrainerBattle(PlayerController player)
    {
        GameController.Instance.StateMachine.Push(CutSceneState.i);
        //Show excalametion
        exclamtion.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamtion.SetActive(false);

        //walk to to player
        var diff = player.transform.position - transform.position;
        var moveVec = diff - diff.normalized;
        moveVec = new Vector2(Mathf.Round(moveVec.x), Mathf.Round(moveVec.y));
        yield return character.Move(moveVec);

        //show dialog
        yield return DialogManager.Instance.ShowDialog(dialog);
        
        GameController.Instance.StateMachine.Pop();
        GameController.Instance.StartTrainerBattle(this);
    }
    
    public void SetFovRotation(FacingDirection dir)
    {
        float angle = 0f;
        if (dir == FacingDirection.Left) angle = 270f;
        else if (dir == FacingDirection.Up) angle = 180f;
        else if (dir == FacingDirection.Right) angle = 90f;

        fov.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }

    public IEnumerator Interact(Transform initiator)
    {
        yield return TriggerTrainerBattleOnInteraction(initiator);
    }

    public IEnumerator TriggerTrainerBattleOnInteraction(Transform initiator)
    {
        //Look at the player
        character.LookToward(initiator.position);

        if (!battleLost)
        {
            //Show excalametion
            exclamtion.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            exclamtion.SetActive(false);

            //show dialog
            yield return DialogManager.Instance.ShowDialog(dialog);
            
                GameController.Instance.StartTrainerBattle(this);
        }
        else
        {
            yield return DialogManager.Instance.ShowDialog(dialogAfterBattle);
        }
    }

    public void BattleLost()
    {
        fov.SetActive(false);
        battleLost = true;
    }

    public object CaptureState()
    {
        return battleLost;
    }

    public void RestoreState(object state)
    {
        battleLost = (bool)state;

        if (battleLost)
            fov.SetActive(false);
    }
}
