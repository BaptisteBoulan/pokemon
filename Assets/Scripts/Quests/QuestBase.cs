using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Quest", menuName = "Quest/Create new Quest")]
public class QuestBase : ScriptableObject
{
    [SerializeField] string name;
    [SerializeField] string description;

    [SerializeField] Dialog startDialog;
    [SerializeField] Dialog progressDialog;
    [SerializeField] Dialog completedDialog;

    [SerializeField] ItemBase requiredItem;
    [SerializeField] ItemBase rewardItem;

    public string Name => name;
    public string Description => description;

    public Dialog StartDialog => startDialog;
    public Dialog ProgresDialog => progressDialog?.Lines.Count > 0 ? progressDialog : startDialog;
    public Dialog CompletedDialog => completedDialog;

    public ItemBase RequiredItem => requiredItem;
    public ItemBase RewardItem => rewardItem;
}
