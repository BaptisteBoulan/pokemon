using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CutSceneAction
{
    [SerializeField] string name;
    [SerializeField] bool waitForCompletion = true;

    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    public bool WaitForCompletion => waitForCompletion;

    public virtual IEnumerator Play()
    {
        yield break;
    }
}
