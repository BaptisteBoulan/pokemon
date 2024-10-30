using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] ChoiceBox choiceBox;
    [SerializeField] Text dialogText;
    [SerializeField] int lettersPerSecond;

    public event Action OnShowDialog;
    public event Action OnCloseDialog;

    public bool IsShowing { get; private set; }

    public static DialogManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public IEnumerator ShowDialog(Dialog dialog, List<string> choices = null, Action<int> onSlected = null)
    {
        yield return new WaitForEndOfFrame();

        OnShowDialog?.Invoke();

        IsShowing = true;
        dialogBox.SetActive(true);

        foreach (var line in dialog.Lines) 
        {
            yield return TypeDialog(line);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
        }

        if (choices != null && choices.Count > 0)
        {
            yield return new WaitForEndOfFrame();
            yield return choiceBox.ShowChoices(choices, onSlected);
        }

        dialogBox.SetActive(false);
        IsShowing = false;
        OnCloseDialog?.Invoke();
    }

    public IEnumerator ShowDialogText(string dialog, Action OnFinished = null, bool waitForInput = true, bool autoClose = true, List<string> choices = null, Action<int> onSlected = null)
    {
        yield return new WaitForEndOfFrame();
        OnShowDialog?.Invoke();

        IsShowing = true;
        dialogBox.SetActive(true);
        yield return TypeDialog(dialog);

        if (waitForInput)
            yield return new WaitUntil(()=>Input.GetKeyDown(KeyCode.Return));

        if (choices != null && choices.Count > 0)
        {
            yield return new WaitForEndOfFrame();
            yield return choiceBox.ShowChoices(choices, onSlected);
        }

        if (autoClose)
        {
            dialogBox.SetActive(false);
            IsShowing = false;
        }
        OnCloseDialog?.Invoke();
    }

    public void CloseDialog()
    {
        dialogBox.SetActive(false);
        IsShowing = false;
    }
    public void handleUpdate()
    {
        
    }

    public IEnumerator TypeDialog(string line)
    {
        dialogText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
    }
}
