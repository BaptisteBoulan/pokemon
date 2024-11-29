using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SummaryScreenUI : MonoBehaviour
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

    Pokemon pokemon;

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
}
