using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text hpBarText;
    [SerializeField] HpBar hpBar;
    [SerializeField] Text statusText;
    [SerializeField] GameObject XPbar;

    [SerializeField] Color psnColor;
    [SerializeField] Color brnColor;
    [SerializeField] Color parColor;
    [SerializeField] Color slpColor;
    [SerializeField] Color frzColor;

    Pokemon _pokemon;

    Dictionary<ConditionID, Color> statusColors;


    public void SetData(Pokemon pokemon)
    {
        if (_pokemon != null)
        {
            _pokemon.OnStatusChanged -= SetStatusText;
            _pokemon.OnHpChange -= UpdateHP;
        }

        _pokemon = pokemon;
        nameText.text = pokemon.Base.Name;
        SetLevel();
        hpBarText.text = pokemon.HP + "/" + pokemon.MaxHP;
        hpBar.SetHP((float) pokemon.HP / pokemon.MaxHP);

        statusColors = new Dictionary<ConditionID, Color>()
        {
            {ConditionID.psn, psnColor },
            {ConditionID.brn, brnColor },
            {ConditionID.par, parColor },
            {ConditionID.slp, slpColor },
            {ConditionID.frz, frzColor },
        };
        SetStatusText();
        _pokemon.OnStatusChanged += SetStatusText;
        _pokemon.OnHpChange += UpdateHP;
        SetXP();
    }

    public IEnumerator UpdateHpAsync()
    {
        yield return hpBar.SetSmoothHP(_pokemon.HP, _pokemon.MaxHP);
        hpBarText.text = _pokemon.HP + "/" + _pokemon.MaxHP;
    }

    public void UpdateHP()
    {
        StartCoroutine(UpdateHpAsync());
    }

    void SetStatusText()
    {
        if (_pokemon.Status == null)
        {
            statusText.text = "";
        }
        else
        {
            statusText.text = _pokemon.Status.Id.ToString().ToUpper();
            statusText.color = statusColors[_pokemon.Status.Id];
        }
    }

    public void SetXP()
    {
        if (XPbar == null) return;

        float normalizedXP = _pokemon.GetNormalizedXP();
        XPbar.transform.localScale = new Vector3(normalizedXP, 1f, 1f);
    }

    public IEnumerator SetXPsmooth(bool reset = false)
    {
        if (XPbar == null) yield break;

        if (reset)
        {
            XPbar.transform.localScale = new Vector3(0f, 1f, 1f);
        }

        float normalizedXP = _pokemon.GetNormalizedXP();
        yield return XPbar.transform.DOScaleX(normalizedXP, 1f).WaitForCompletion();
    }

    public void SetLevel()
    {
        levelText.text = "Lv." + _pokemon.Level;
    }

    public IEnumerator WaitForHpUpdate()
    {
        yield return new WaitUntil(() => !hpBar.IsUpdating);
    }

    public void ClearData()
    {
        if (_pokemon != null)
        {
            _pokemon.OnStatusChanged -= SetStatusText;
            _pokemon.OnHpChange -= UpdateHP;
        }
    }
}
