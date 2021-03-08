using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using TMPro;

public class TestConnect : MonoBehaviourPunCallbacks
{
    // [SerializeField] TMP_InputField roomName;
    [SerializeField] string roomName;
    private NetworkSettings settings;

    private void Awake() 
    {
        settings = NetworkSettings.instance;    

        PhotonNetwork.AutomaticallySyncScene = true; // TODO is this needed?
    }
    private void Start()
    {
        PlayerPrefs.DeleteAll();
        print("Attempting connection to Photon");

        

        PhotonNetwork.NickName = settings.Nickname;
        PhotonNetwork.GameVersion = settings.GameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    // private void CreateRoom() 
    // {
    //     if (PhotonNetwork.IsConnected)
    //     {
    //         if (string.IsNullOrEmpty(roomName.text))
    //         {
    //             return;
    //         }
    //         PhotonNetwork.CreateRoom(roomName.text);
    //     }
    // }

    public void JoinRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            print($"PhotonNetwork.IsConnected | Trying to create or join room {roomName}");
            RoomOptions options = new RoomOptions();
            TypedLobby lobby = new TypedLobby(roomName, LobbyType.Default);
            PhotonNetwork.JoinOrCreateRoom(roomName, options, lobby);
        }
    }

    // Callbacks
    public override void OnConnectedToMaster() 
    {
        print("Connected to Server Succ");
        print($"My name is {PhotonNetwork.LocalPlayer.NickName}");
        PhotonNetwork.JoinLobby(); // TODO might wanna remove this later if we want to support multiple sessions
    }

    public override void OnJoinedLobby()
    {
        print("Connected to Lobby Succ");
        MenuManager.instance.CloseMenu("loading");
        MenuManager.instance.OpenMenu("main");
    }

    public override void OnJoinedRoom()
    {
        print("Connected to Room Succ");
        MenuManager.instance.CloseMenu("main");
        MenuManager.instance.OpenMenu("ingame");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print($"Disconnected form server for {cause}");
    }
}


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