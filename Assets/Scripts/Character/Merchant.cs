using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchant : MonoBehaviour
{
    [SerializeField] List<ItemBase> availablesItems;

    public List<ItemBase> AvailablesItems => availablesItems;
    public IEnumerator Trade()
    {
        yield return ShopController.instance.StartTrading(this);
    }
}
