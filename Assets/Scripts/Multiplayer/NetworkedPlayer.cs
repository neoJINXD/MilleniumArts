using Photon.Pun;

/*
    Networked variant of the player MonoBehaviour
*/

public class NetworkedPlayer : LocalPlayer, IPunObservable
{

    public bool amIP1 = false;

    // private void Start() 
    // {
    //     if (PhotonNetwork.IsMasterClient)
    //         amIP1 = true;
    // }

    public override void StartTurn()
    {
        TurnComplete = false;
        if (PhotonNetwork.IsMasterClient == amIP1)
            TurnManager.instance.ShowCardSelection();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(TurnComplete);
        }
        else
        {
            TurnComplete = (bool)stream.ReceiveNext();
        }
    }
}