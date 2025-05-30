using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] bool isPlayerUnit;
    [SerializeField] BattleHud hud;

    Image image;
    Vector3 originalPos;
    Color originalColor;

    public bool IsPlayerUnit {get {return isPlayerUnit ;}}
    private void Awake()
    {
        image = GetComponent<Image>();
        originalPos = image.transform.localPosition;
        originalColor = image.color;
    }
    public Pokemon Pokemon { get; set; }

    public BattleHud Hud { get { return hud; } }
    public void Setup(Pokemon pokemon)
    {
        Pokemon = pokemon;
        image.color = originalColor;
        if (isPlayerUnit)
        {
            image.sprite = Pokemon.Base.BackSprite;
        } else
        {
            image.sprite = Pokemon.Base.FrontSprite;
        }

        hud.gameObject.SetActive(true);
        hud.SetData(pokemon);

        transform.localScale = new Vector3(1f, 1f, 1f);
        PlayBattleEnterAnimation();
    }
    public void Clear()
    {
        hud.gameObject.SetActive(false);
    }
    void PlayBattleEnterAnimation()
    {
        if (isPlayerUnit)
        {
            image.transform.localPosition = new Vector3(-500f, originalPos.y);
        } else
        {
            image.transform.localPosition = new Vector3(500f, originalPos.y);
        }

        image.transform.DOLocalMoveX(originalPos.x, 0.8f);
    }

    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
        {
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x + 50f, 0.2f));
        } else
        {
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x - 50f, 0.2f));
        }
        sequence.Append(image.transform.DOLocalMoveX(originalPos.x, 0.2f));
    }

    public void PlayHitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.gray, 0.05f));
        sequence.Append(image.DOColor(originalColor, 0.05f));
    }

    public void PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, 0.4f));
        sequence.Join(image.DOFade(0f,0.4f));
    }

    public IEnumerator PlayCaptureAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(0, 0.4f));
        sequence.Join(transform.DOLocalMoveY(originalPos.y + 50f, 0.4f));
        sequence.Join(transform.DOScale(new Vector3(0.3f,0.3f,1f), 0.4f));
        yield return sequence.WaitForCompletion();
    }

    public IEnumerator PlayBreakOutAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(1, 0.2f));
        sequence.Join(transform.DOLocalMoveY(originalPos.y, 0.2f));
        sequence.Join(transform.DOScale(new Vector3(1f, 1f, 1f), 0.2f));
        yield return sequence.WaitForCompletion();
    }
}
