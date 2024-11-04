using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Xml;
using Utils.StateMachine;

public class EvolutionState : State<GameController>
{   [SerializeField] Image background;
    [SerializeField] Image pokemonSprite;
    [SerializeField] List<Sprite> spriteList;

    CanvasAnimator animator;

    Pokemon babyPokemon;
    PokemonBase daddyPokemon;

   
    public static EvolutionState i;

    private void Awake()
    {
        i = this;
        animator = new CanvasAnimator(background,spriteList);
    }

    GameController gc;

    private void Start()
    {
        gc = GameController.Instance;
    }

    public void SetupData(Pokemon baby, PokemonBase daddy)
    {
        babyPokemon = baby; 
        daddyPokemon = daddy;
        pokemonSprite.sprite = babyPokemon.Base.FrontSprite;
    }

    private void Update()
    {
        if (background != null && background.gameObject != null && background.gameObject.activeSelf)
        {
            animator.HandleUpdate();
        }

    }


    public IEnumerator Evolve(Pokemon baby, PokemonBase daddy)
    {
        gc.StateMachine.Push(this);

        SetupData(baby, daddy);

        background.gameObject.SetActive(true);

        animator.Start();

        yield return DialogManager.Instance.ShowDialogText($"{babyPokemon.Base.Name} is evolving...");

        yield return PlayEvolveAnimation();

        string babyName = babyPokemon.Base.Name;
        babyPokemon.Evolve(daddyPokemon);

        yield return DialogManager.Instance.ShowDialogText($"{babyName} evolved into {daddyPokemon.Name} !");

        background.gameObject.SetActive(false);

        gc.PartyScreen.SetPartyData();

        gc.StateMachine.Pop();
    }

    private IEnumerator PlayEvolveAnimation()
    {
        yield return new WaitForSeconds(0.5f);

        var firstSequence = DOTween.Sequence();
        firstSequence.Append(pokemonSprite.DOColor(Color.black, 0.5f));
        firstSequence.Join(pokemonSprite.transform.DOScale(0.3f,0.5f));

        yield return firstSequence.WaitForCompletion();

        pokemonSprite.sprite = daddyPokemon.FrontSprite;

        var secondSequence = DOTween.Sequence();
        secondSequence.Append(pokemonSprite.DOColor(Color.white, 0.5f));
        secondSequence.Join(pokemonSprite.transform.DOScale(1f, 0.5f));

        yield return secondSequence.WaitForCompletion();

        yield return new WaitForSeconds(0.5f);
    }

}
