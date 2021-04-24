using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkedUnit : Unit, IPunObservable
{
    private PhotonView view;
    protected void Start() 
    {
        view = PhotonView.Get(gameObject);
        view.ObservedComponents.Add(this);

        if (!view.IsMine)
        {
            switch(unitType)
            {
                case UnitTypes.Archer:
                    if (unitPlayerId == -1 && GameLoop.instance.GetCurrentPlayer().PlayerId == 0)
                    {
                        unitPlayerId = 0;
                        GetComponentInChildren<SpriteRenderer>().sprite = CardEffectManager.instance.sprite_archer_blue;
                    }
                    else
                    {
                        unitPlayerId = 1;
                        GetComponentInChildren<SpriteRenderer>().sprite = CardEffectManager.instance.sprite_archer_red;
                    }
                    break;

                case UnitTypes.Assassin:
                    if (unitPlayerId == -1 && GameLoop.instance.GetCurrentPlayer().PlayerId == 0)
                    {
                        unitPlayerId = 0;
                        GetComponentInChildren<SpriteRenderer>().sprite = CardEffectManager.instance.sprite_assassin_blue;
                    }
                    else
                    {
                        unitPlayerId = 1;
                        GetComponentInChildren<SpriteRenderer>().sprite = CardEffectManager.instance.sprite_assassin_red;
                    }
                    break;

                case UnitTypes.DragonRider:
                    if (unitPlayerId == -1 && GameLoop.instance.GetCurrentPlayer().PlayerId == 0)
                    {
                        unitPlayerId = 0;
                        GetComponentInChildren<SpriteRenderer>().sprite = CardEffectManager.instance.sprite_dragonRider_blue;
                    }
                    else
                    {
                        unitPlayerId = 1;
                        GetComponentInChildren<SpriteRenderer>().sprite = CardEffectManager.instance.sprite_dragonRider_red;
                    }
                    break;

                case UnitTypes.Knight:
                    if (unitPlayerId == -1 && GameLoop.instance.GetCurrentPlayer().PlayerId == 0)
                    {
                        unitPlayerId = 0;
                        GetComponentInChildren<SpriteRenderer>().sprite = CardEffectManager.instance.sprite_knight_blue;
                    }
                    else
                    {
                        unitPlayerId = 1;
                        GetComponentInChildren<SpriteRenderer>().sprite = CardEffectManager.instance.sprite_knight_red;
                    }
                    break;

                case UnitTypes.Priest:
                    if (unitPlayerId == -1 && GameLoop.instance.GetCurrentPlayer().PlayerId == 0)
                    {
                        unitPlayerId = 0;
                        GetComponentInChildren<SpriteRenderer>().sprite = CardEffectManager.instance.sprite_priest_blue;
                    }
                    else
                    {
                        unitPlayerId = 1;
                        GetComponentInChildren<SpriteRenderer>().sprite = CardEffectManager.instance.sprite_priest_red;
                    }
                    break;

                case UnitTypes.Soldier:
                    if (unitPlayerId == -1 && GameLoop.instance.GetCurrentPlayer().PlayerId == 0)
                    {
                        unitPlayerId = 0;
                        GetComponentInChildren<SpriteRenderer>().sprite = CardEffectManager.instance.sprite_soldier_blue;
                    }
                    else
                    {
                        unitPlayerId = 1;
                        GetComponentInChildren<SpriteRenderer>().sprite = CardEffectManager.instance.sprite_soldier_red;
                    }
                    break;

            }
        }
    }
    
    
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // if (stream.IsWriting)
        // {
        //     if (PhotonView.Get(gameObject).IsMine)
        //         stream.SendNext(unitPlayerId);
        // }
        // else
        // {
        //     //if (!PhotonView.Get(gameObject).IsMine)
        //     unitPlayerId = (int)stream.ReceiveNext();
        // }
    }
}
