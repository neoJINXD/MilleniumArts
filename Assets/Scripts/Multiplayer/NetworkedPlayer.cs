using Photon.Pun;

/*
    Networked variant of the player MonoBehaviour
*/

public class NetworkedPlayer : Player, IPunObservable
{

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