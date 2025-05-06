using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CutScene))]
public class CutSceneEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var cutscene = target as CutScene;

        using(var scope = new GUILayout.HorizontalScope()) {
            if (GUILayout.Button("Dialog"))
            {
                cutscene.AddAction(new DialogAction());
            }
            else if (GUILayout.Button("Interact"))
            {
                cutscene.AddAction(new NPCIntaractableAction());
            }
            else if (GUILayout.Button("Battle"))
            {
                cutscene.AddAction(new StartBattleAction());
            }
        }
        

        using (var scope = new GUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Move Actor"))
            {
                cutscene.AddAction(new MoveActorAction());
            }
            else if (GUILayout.Button("Turn Actor"))
            {
                cutscene.AddAction(new TurnActorAction());
            }
            else if (GUILayout.Button("Teleport"))
            {
                cutscene.AddAction(new TeleportObjectAction());
            }
        }


        using (var scope = new GUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Enable"))
            {
                cutscene.AddAction(new EnableObjectAction());
            }
            else if (GUILayout.Button("Disable"))
            {
                cutscene.AddAction(new DisableObjectAction());
            }
        }

        using (var scope = new GUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Enable Pickup"))
            {
                cutscene.AddAction(new EnablePickupAction());
            }
            else if (GUILayout.Button("Disable Pickup"))
            {
                cutscene.AddAction(new DisablePickupAction());
            }
        }

        using (var scope = new GUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Fade In"))
            {
                cutscene.AddAction(new FadeInAction());
            }
            else if (GUILayout.Button("Fade Out"))
            {
                cutscene.AddAction(new FadeOutAction());
            }
        }

        using (var scope = new GUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Move Object"))
            {
                cutscene.AddAction(new MoveObjectAction());
            }
        }

        base.OnInspectorGUI();
    }
}
