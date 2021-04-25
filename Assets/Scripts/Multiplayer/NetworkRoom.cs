using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
public class NetworkRoom : MonoBehaviourPunCallbacks
{
    // private void Update() 
    // {
    //     if(PhotonNetwork.PlayerList.Length != 2)
    //     {
    //         PhotonNetwork.LeaveRoom();
    //         // SceneManager.LoadScene("PhotonPrototyping");
    //     }      
    // }
    
    public static void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
 
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("PhotonPrototyping");
    
        base.OnLeftRoom();
    }
}