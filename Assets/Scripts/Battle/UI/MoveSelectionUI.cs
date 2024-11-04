using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Utils.GenericSelectionUI;

public class MoveSelectionUI : SelectionUI<TextSlot>
{
    [SerializeField] List<TextSlot> moveTexts;
    [SerializeField] Text ppText;
    [SerializeField] Text typeText;

    List<Move> moves;

    public void SetMoves(List<Move> moves)
    {
        selectedItem = 0;

        this.moves = moves;
        SetSelectionType(SelectionType.Grid, 2);
        SetItems(moveTexts.Take(moves.Count).ToList());

        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (i < moves.Count)
                moveTexts[i].SetText(moves[i].Base.Name);
            else
                moveTexts[i].SetText("-");
        }
    }

    public override void UpdateSelectionUI()
    {
        base.UpdateSelectionUI();

        var move = moves[selectedItem];

        ppText.text = $"{move.PP}/{move.Base.MaxPP}";
        typeText.text = move.Base.Type.ToString();

        if (move.PP == 0) ppText.color = Color.red;
        else ppText.color = Color.black;
    }
}
