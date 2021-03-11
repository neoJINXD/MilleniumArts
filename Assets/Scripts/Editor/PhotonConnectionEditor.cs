using UnityEngine;
using UnityEditor;
using Photon.Pun;

[CustomEditor(typeof(PhotonConnection))]
public class PhotonConnectionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if(PhotonNetwork.IsConnected && GUILayout.Button("Get Player List"))
        {

            Debug.Log("Getting player list:");
            foreach (var item in PhotonNetwork.PlayerList)
            {
                Debug.Log(item.NickName);
            }
        }
    }
}