using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text messageText;
    [SerializeField] Text levelText;
    [SerializeField] Text hpBarText;
    [SerializeField] HpBar hpBar;

    Pokemon _pokemon;
    public void Init(Pokemon pokemon)
    {
        _pokemon = pokemon;

        UpdateData();

        _pokemon.OnHpChange += UpdateData;

        SetMessage("");

    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            nameText.color = GlobalSettings.i.HighlightedColor; ;
        } else
        {
            nameText.color = Color.black;
        }

    }

    void UpdateData()
    {
        nameText.text = _pokemon.Base.Name;
        levelText.text = "lv." + _pokemon.Level;
        hpBarText.text = _pokemon.HP + "/" + _pokemon.MaxHP;
        hpBar.SetHP((float)_pokemon.HP / _pokemon.MaxHP);
    }

    public void SetMessage(string input)
    {
        messageText.text = input;
    }
}
