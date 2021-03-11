using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

/*
    To be control and initiate gameloop & misc local game actions
 */

public class GameManager : MonoBehaviour
{
    private GameLoop gameLoop;

    [SerializeField] private GameObject player1Spawn;
    [SerializeField] private GameObject player2Spawn;
    [SerializeField]private GameObject player1;
    [SerializeField]private GameObject player2;
    [SerializeField] Material player1Mat;
    [SerializeField] Material player2Mat;

    private PhotonView view;
    
    void Start()
    {
        gameLoop = GameLoop.instance;
        view = GetComponent<PhotonView>();

        if (!PhotonNetwork.IsConnected)
        {
            // go back to 'menu'
            SceneManager.LoadScene("PhotonPrototyping");
            return;
        }

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

        // gameLoop.AddPlayer(gameObject.AddComponent<Player>());
        Player p1 = gameLoop.AddReturnPlayer(gameObject.AddComponent<Player>());
        // gameLoop.AddPlayer(gameObject.AddComponent<Player>());
        Player p2 = gameLoop.AddReturnPlayer(gameObject.AddComponent<Player>());
        StartCoroutine(gameLoop.Play());

        view.ObservedComponents.Add(p1);
        view.ObservedComponents.Add(p2);
    }
}
