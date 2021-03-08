using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Player))]
public class PlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Player player = (Player)target;
        if(!player.TurnComplete && GUILayout.Button("End Turn"))
        {
            player.EndTurn();
        }
    }
}
