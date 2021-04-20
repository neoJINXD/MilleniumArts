using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Zone.Core.Utils;

/*
    To be control and initiate gameloop & misc local game actions
 */

public class GameManager : Singleton<GameManager>
{
    private GameLoop gameLoop;

    [SerializeField] private bool networked;
    [SerializeField] private GameObject player1Spawn;
    [SerializeField] private GameObject player2Spawn;
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;
    [SerializeField] Material player1Mat;
    [SerializeField] Material player2Mat;

    [SerializeField] string gameHistory;

    public PhotonView view { get; private set; }

    private bool isLeaving = false;
    
    void Start()
    {
        gameLoop = GameLoop.instance;
        if (networked)
        {
            PhotonStart();
        }
        else
        {
            LocalStart();
        }
        isLeaving = false;
    }

    private void LocalStart()
    {
        StartCoroutine(gameLoop.Play());
    }

    private void PhotonStart()
    {
        if (!PhotonNetwork.IsConnected)
        {
            // go back to 'menu'
            SceneManager.LoadScene("PhotonPrototyping");
            return;
        }
        view = GetComponent<PhotonView>();


        // TODO change cube to king unit
        if (PhotonNetwork.IsMasterClient)
        {
            print("Player 1 creating");
            player1 = PhotonNetwork.Instantiate("Cube", 
                player1Spawn.transform.position, player1Spawn.transform.rotation, 0);
            player1.GetComponent<Renderer>().material = player1Mat;
        }
        else
        {
            print("Player 2 creating");
            player2 = PhotonNetwork.Instantiate("Cube",
                player2Spawn.transform.position, player2Spawn.transform.rotation, 0);
            player2.GetComponent<Renderer>().material = player2Mat;
        }

        Player p1 = gameLoop.AddReturnPlayer(gameObject.AddComponent<NetworkedPlayer>());
        Player p2 = gameLoop.AddReturnPlayer(gameObject.AddComponent<NetworkedPlayer>());
        StartCoroutine(gameLoop.Play());

        view.ObservedComponents.Add(p1);
        view.ObservedComponents.Add(p2);
    }

    private void Update() 
    {
        print(PhotonNetwork.PlayerList.Length);
        if(networked && !isLeaving && PhotonNetwork.IsConnected && PhotonNetwork.PlayerList.Length != 2)
        {
            isLeaving = true;
            NetworkRoom.LeaveRoom();
            //     PhotonNetwork.LeaveRoom();
            //     SceneManager.LoadScene("PhotonPrototyping");
        }    
    }

    public Player GetCurrentPlayer() => gameLoop.GetCurrentPlayer();

    [PunRPC]
    public void EndCurrentPlayerTurn() => gameLoop.EndCurrentPlayer();
}
