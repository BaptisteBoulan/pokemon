using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class PlayerController : MonoBehaviour, ISavable
{
    private Vector2 inputs;

    [SerializeField] Sprite sprite;
    [SerializeField] string name;
    Character character;

    public static PlayerController i;

    public Sprite Sprite { get => sprite; }
    public string Name { get { return name; } }
    public Character Character { get { return character; } }
    private void Awake()
    {
        character = GetComponent<Character>();
        i = this;
    }

    public void HandleUpdate()
    {
        if (!character.IsMoving) {
            inputs.x = Input.GetAxisRaw("Horizontal"); 
            inputs.y = Input.GetAxisRaw("Vertical");
            if (inputs.x != 0) inputs.y = 0;

            if (inputs != Vector2.zero)
                StartCoroutine(character.Move(inputs,OnMoveOver));

        }
        character.HandleUpdtate();
        if (Input.GetKeyDown(KeyCode.Return)) StartCoroutine(Interact());
    }

    IEnumerator Interact()
    {
        var faceDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var interactPos = transform.position + faceDir;

        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.i.InteractableLayer | GameLayers.i.WaterLayer);
        if (collider != null)
        {
            yield return collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }

    IPlayerTriggerable prevTriggerable = null;
    private void OnMoveOver()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, character.OffsetY, 0), 0.2f, GameLayers.i.TriggerableLayers);

        IPlayerTriggerable triggerable = null;
        foreach(var collider in colliders)
        {
            triggerable = collider.GetComponent<IPlayerTriggerable>();

            if (triggerable != prevTriggerable || triggerable.TriggerRepeatly)
                triggerable?.OnPlayerTriggered(this);
            prevTriggerable = triggerable;
            break;
        }

        if (colliders.Count() == 0 || prevTriggerable != triggerable) prevTriggerable = null;
    }

    public object CaptureState()
    {
        var saveData = new PlayerSaveData()
        {
            position = new float[] { transform.position.x, transform.position.y },
            pokemons = GetComponent<PokemonParty>().Pokemons.Select(p => p.GetSaveData()).ToList(),
        };

        return saveData;
    }

    public void RestoreState(object state)
    {
        // position
        var saveData = (PlayerSaveData)state;
        var position = saveData.position;
        transform.position = new Vector3(position[0], position[1]);

        // party
        GetComponent<PokemonParty>().Pokemons = saveData.pokemons.Select(s => new Pokemon(s)).ToList();
    }
}

[Serializable]
public class PlayerSaveData
{
    public float[] position;
    public List<PokemonSaveData> pokemons;
}
