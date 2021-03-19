using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

/*
    Abstract class to be inherited by a local player, networked player, client player?, and AI player
 */

// TODO move the IPunObservable to Networked player
public /* abstract */ class Player : MonoBehaviour//, IPunObservable
{
    public Player()
    {
        TurnComplete = true;
    }

    public void StartTurn()
    {
        TurnComplete = false;
    }

    public virtual void EndTurn()
    {
        TurnComplete = true;
    }

    // public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    // {
    //     if (stream.IsWriting)
    //     {
    //         stream.SendNext(TurnComplete);
    //     }
    //     else
    //     {
    //         TurnComplete = (bool)stream.ReceiveNext();
    //     }
    // }

    public bool TurnComplete ;//{ get; private set; }
}

