using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WalletUI : MonoBehaviour
{
    [SerializeField] Text text;


    private void Start()
    {
        Wallet.Instance.OnUpdated += SetText;
    }

    public void Show()
    {
        SetText();
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
    
    void SetText()
    {
        text.text = "$ " + Wallet.Instance.Money;
    }
}
