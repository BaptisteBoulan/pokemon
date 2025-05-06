using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Utils.GenericSelectionUI;
using System.Linq;

public class PartyScreen : SelectionUI<TextSlot>
{
    [SerializeField] Text messageText;

    PartyMemberUI[] memberSlots;
    List<Pokemon> pokemons;
    PokemonParty party;

    public Pokemon SelectedMember => pokemons[selectedItem];

    public void Init()
    {
        SetPartyData();
        party.OnUpdated += ClearItems;
        party.OnUpdated += SetPartyData;
    }

    public void SetPartyData()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true);
        party = PokemonParty.GetPlayerParty();
        pokemons = party.Pokemons;
        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < pokemons.Count)
            {
                memberSlots[i].gameObject.SetActive(true);
                memberSlots[i].Init(pokemons[i]);
            }
            else
            {
                memberSlots[i].gameObject.SetActive(false);
            }
        }

        var textSlots = memberSlots.Select(s => s.GetComponent<TextSlot>());

        SetSelectionType(SelectionType.Grid, 2);
        SetItems(textSlots.Take(pokemons.Count).ToList());
    }

    public void SetMessageText(string message)
    {
        messageText.text = message;
    }
    public void ShowIfTmIsUsable(TmItem tm)
    {
        for (int i = 0; i < pokemons.Count; i++)
        {
            if (tm.CanBeTaught(party.Pokemons[i]))
                memberSlots[i].SetMessage("ABLE");
            else
                memberSlots[i].SetMessage("NOT ABLE");
        }
    }
    public void ClearMemberSlotMessage()
    {
        for (int i = 0; i < pokemons.Count; i++)
        {
            memberSlots[i].SetMessage("");
        }
    }
}
