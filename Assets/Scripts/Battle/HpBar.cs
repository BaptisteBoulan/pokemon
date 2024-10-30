using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField] GameObject health;
    [SerializeField] Text hpText;

    public bool IsUpdating { get; private set; }

    public void SetHP(float hpNormalized)
    {
        health.transform.localScale = new Vector3(hpNormalized, 1f);
    }

    public IEnumerator SetSmoothHP( int newHp, int maxHp)
    {
        IsUpdating = true;
        float newHpScale = (float)newHp / maxHp;
        float curHP = health.transform.localScale.x;
        float changeAmount = curHP - newHpScale;

        while (curHP - newHpScale > Mathf.Epsilon)
        {
            curHP -= changeAmount * Time.deltaTime;
            health.transform.localScale = new Vector3(curHP, 1f);
            hpText.text = (int)(curHP* maxHp) + "/" + maxHp;
            yield return null;
        }
        health.transform.localScale = new Vector3(newHpScale, 1f);
        IsUpdating = false;
    }
}
