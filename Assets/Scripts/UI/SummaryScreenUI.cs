using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Utils.GenericSelectionUI;

public class SummaryScreenUI : SelectionUI<TextSlot>
{
    [Header("Left")]
    [SerializeField] Image pokemonImage;
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;

    [Header("Right")]
    [SerializeField] Text hpText;
    [SerializeField] Text attText;
    [SerializeField] Text defText;
    [SerializeField] Text speAtt;
    [SerializeField] Text speDef;
    [SerializeField] Text speedText;
    [SerializeField] Text ExpText;
    [SerializeField] Text nextLevelText;
    [SerializeField] Transform expBar;


    [Header("Moves")]
    [SerializeField] List<Text> moveTypes;
    [SerializeField] List<Text> moveNames;
    [SerializeField] List<Text> movePPs;
    [SerializeField] Text moveDescription;
    [SerializeField] Text powerText;
    [SerializeField] Text accuracytext;
    [SerializeField] GameObject moveStatsUI;

    List<TextSlot> moveSlots;


    Pokemon pokemon;

    bool inMoveSelection;

    public bool InMoveSelection
    {
        get => inMoveSelection;
        set
        {
            inMoveSelection = value;
            if (inMoveSelection)
            {
                SetItems(moveSlots.Take(pokemon.Moves.Count).ToList());
                moveStatsUI.SetActive(true);
            }
            else
            {
                ClearItems();
                moveStatsUI.SetActive(false);
            }
        }
    }

    private void Start()
    {
        moveSlots = moveNames.Select(m => m.GetComponent<TextSlot>()).ToList();
    }

    public void SetBasicDetails(Pokemon pokemon)
    {
        this.pokemon = pokemon;

        pokemonImage.sprite = pokemon.Base.FrontSprite;
        nameText.text = pokemon.Base.Name;
        levelText.text = "LV. " + pokemon.Level;
    }

    public void SetSkills()
    {
        hpText.text = pokemon.HP + "/" + pokemon.MaxHP;
        attText.text = "" + pokemon.Attack;
        defText.text = "" + pokemon.Defense;
        speAtt.text = "" + pokemon.SpeAttack;
        speDef.text = "" + pokemon.SpeDefense;
        speedText.text = "" + pokemon.Speed;


        ExpText.text = "" + pokemon.XP;
        nextLevelText.text = "" + (pokemon.Base.GetExpForLevel(pokemon.Level + 1) - pokemon.XP);

        expBar.localScale = new Vector2(pokemon.GetNormalizedXP(), 1);
    }

    public void SetMoves()
    {
        moveDescription.text = "";

        var moves = pokemon.Moves;

        for (int i = 0; i < PokemonBase.MaxMoveNumber; i++)
        {
            if (i < moves.Count)
            {
                var move = moves[i];

                moveTypes[i].text = "" + move.Base.Type;
                moveNames[i].text = "" + move.Base.Name;
                movePPs[i].text = move.PP + "/" + move.Base.MaxPP;     
            }
            else
            {
                moveTypes[i].text = "-";
                moveNames[i].text = "-";
                movePPs[i].text = "-/-";
            }

        }
    }

    public override void HandleUpdate()
    {
        if (inMoveSelection)
        {
            base.HandleUpdate();
            var move = pokemon.Moves[selectedItem].Base;

            moveDescription.text = move.Description;

            if (move.Power > 0) powerText.text = "" + move.Power;
            else powerText.text = "-";

            if (move.AlwaysHits) accuracytext.text = "-";
            else accuracytext.text = "" + move.Accuracy;
        }

    }
}
