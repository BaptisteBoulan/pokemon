using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallet : MonoBehaviour, ISavable
{
    [SerializeField] int money;
    public int Money => money;

    public event Action OnUpdated;


    public static Wallet Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void AddMoney(int amount)
    {
        money += amount;
        OnUpdated?.Invoke();
    }
    public void RemoveMoney(int amount)
    {
        money -= amount;
        OnUpdated?.Invoke();
    }

    public object CaptureState()
    {
        return money;
    }

    public void RestoreState(object state)
    {
        money = (int)state;
    }
}
