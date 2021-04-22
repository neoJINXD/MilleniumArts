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

        print("Starting the photon game");
        //if (PhotonNetwork.IsMasterClient)
        //{
            Player p1 = gameObject.AddComponent<NetworkedPlayer>();
            //print(p1);
            view.ObservedComponents.Add(p1);

            Player p2 = gameObject.AddComponent<NetworkedPlayer>();
            //print(p2);
            view.ObservedComponents.Add(p2);

            gameLoop.GetPlayerList().Clear();
            gameLoop.AddPlayer(p1);
            gameLoop.AddPlayer(p2);
            //p2.EndTurn();
        //}

        StartCoroutine(gameLoop.Play());
    }

    private void Update() 
    {
        //print(PhotonNetwork.PlayerList.Length);
        if(networked && !isLeaving && PhotonNetwork.IsConnected && PhotonNetwork.PlayerList.Length != 2)
        {
            isLeaving = true;
            NetworkRoom.LeaveRoom();
            //     PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("PhotonPrototyping");
        }    
    }

    public Player GetCurrentPlayer() => gameLoop.GetCurrentPlayer();

    [PunRPC]
    public void EndCurrentPlayerTurn() => gameLoop.EndCurrentPlayer();
}
