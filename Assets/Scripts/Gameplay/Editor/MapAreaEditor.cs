using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapArea))]
public class MapAreaEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;

        int totalChance = serializedObject.FindProperty("totalChance").intValue;

        GUILayout.Label($"Total Chance : {totalChance}",style);

        if (totalChance != 100)
            EditorGUILayout.HelpBox("The total chance percent is not 100", MessageType.Error);

        int totalChanceWater = serializedObject.FindProperty("totalChanceWater").intValue;

        GUILayout.Label($"Total Chance on Water : {totalChanceWater}", style);

        if (totalChanceWater != 100)
            EditorGUILayout.HelpBox("The total chance on Water percent is not 100", MessageType.Error);
    }
}
