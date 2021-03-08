using UnityEngine;
using UnityEditor;
using Photon.Pun;

[CustomEditor(typeof(TestConnect))]
public class TestConnectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TestConnect connection = (TestConnect)target;
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