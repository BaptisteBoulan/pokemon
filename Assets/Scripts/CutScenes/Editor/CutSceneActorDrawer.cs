using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CutSceneActor))]
public class CutSceneActorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, label);

        var offset = 70;

        var tooglePos = new Rect(position.x, position.y, offset, position.height);
        var fieldPos = new Rect(position.x + offset, position.y, position.width-offset, position.height);

        var isPlayerProperty = property.FindPropertyRelative("isPlayer");

        isPlayerProperty.boolValue = GUI.Toggle(tooglePos, isPlayerProperty.boolValue,"Is Player");
        isPlayerProperty.serializedObject.ApplyModifiedProperties();

        if (!isPlayerProperty.boolValue)
            EditorGUI.PropertyField(fieldPos, property.FindPropertyRelative("character"), GUIContent.none);

        EditorGUI.EndProperty();
    }
}
