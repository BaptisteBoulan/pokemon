using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectsDB<T> : MonoBehaviour where T :ScriptableObject
{
    static Dictionary<string, T> objects;

    public static void Init()
    {
        objects = new Dictionary<string, T>();

        var objArray = Resources.LoadAll<T>("");
        foreach (var obj in objArray)
        {
            if (objects.ContainsKey(obj.name))
            {
                Debug.Log("two have the same name...");
            }
            else
            {
                objects[obj.name] = obj;
            }
        }
    }

    public static T GetObjectByName(string name)
    {
        if (!objects.ContainsKey(name))
        {
            Debug.Log($"{name} not found");
            return null;
        }

        return objects[name];
    }
}
