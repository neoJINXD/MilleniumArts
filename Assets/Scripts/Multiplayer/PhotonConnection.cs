using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class PhotonConnection : MonoBehaviourPunCallbacks
{
    [SerializeField] string roomName; // TODO should eventually not have this hardcoded
    private NetworkSettings settings;
    private bool inRoom = false;

    [SerializeField] private GameObject joinButton;
    [SerializeField] private TMPro.TMP_Text infoText;

    private void Awake() 
    {
        settings = NetworkSettings.instance;    

        PhotonNetwork.AutomaticallySyncScene = true; // TODO is this needed?
    }
    private void Start()
    {
        joinButton.SetActive(false);

        if (!PhotonNetwork.IsConnected)
        {
            PlayerPrefs.DeleteAll();
            print("Attempting connection to Photon");

            PhotonNetwork.NickName = settings.Nickname;
            PhotonNetwork.GameVersion = settings.GameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
        else 
        {
            MenuManager.instance.CloseMenu("loading");
            MenuManager.instance.OpenMenu("main");
        }
    }

    private void Update() 
    {
        // TODO should add player count in room view
        print(PhotonNetwork.PlayerList.Length);
        if (inRoom && PhotonNetwork.PlayerList.Length == 2 && PhotonNetwork.IsMasterClient)
        {
            joinButton.SetActive(true);
        }
        if (PhotonNetwork.PlayerList.Length != 2)
        {
            joinButton.SetActive(false);
        }
    }

    public void JoinRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            print($"PhotonNetwork.IsConnected | Trying to create or join room {roomName}");
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 2;
            TypedLobby lobby = new TypedLobby(roomName, LobbyType.Default);
            PhotonNetwork.JoinOrCreateRoom(roomName, options, lobby);
        }
    }

    public void JoinGame()
    {
        // PhotonNetwork.LoadLevel("PhotonMain");
        PhotonNetwork.LoadLevel("PhotonGameMap");
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
        // PhotonNetwork.Instantiate("Cube", Vector3.zero, Quaternion.identity);
        // TODO should use PhotonNetwork's LoadScene method with a waiting lobby
        // SceneManager.LoadScene("PhotonMain");
        if (PhotonNetwork.IsMasterClient)
        {
            infoText.text = "You are the Blue Units";
        }
        else
        {
            infoText.text = "You are the Red Units";
        }
        inRoom = true;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print($"Disconnected form server for {cause}");
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        print($"{newPlayer.NickName} has joined the room");
    }
}
