using UnityEngine;
using UnityEditor;
using Photon.Pun;

[CustomEditor(typeof(Player), true)]
public class PlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Player player = (Player)target;
        if (!player.TurnComplete)
            GUILayout.Label("Its my turn");

        DrawDefaultInspector();

        if(!player.TurnComplete && GUILayout.Button("End Turn"))
        {
            // player.EndTurn();
            if (GameManager.instance.GetCurrentPlayer() == player)
                GameManager.instance.view.RPC("EndCurrentPlayerTurn", RpcTarget.All);
        }
    }
}
