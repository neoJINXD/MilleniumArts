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

            PhotonConnection connection = (PhotonConnection)target;
            Debug.Log($"Getting player list of {PhotonNetwork.PlayerList.Length}:");
            foreach (var item in PhotonNetwork.PlayerList)
            {
                Debug.Log(item.NickName);
            }
        }
    }
}