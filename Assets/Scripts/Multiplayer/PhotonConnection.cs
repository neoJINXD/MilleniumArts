using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class PhotonConnection : MonoBehaviourPunCallbacks
{
    // [SerializeField] TMP_InputField roomName;
    [SerializeField] string roomName; // TODO should eventually not have this hardcoded
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
        SceneManager.LoadScene("PhotonMain");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print($"Disconnected form server for {cause}");
    }
}
