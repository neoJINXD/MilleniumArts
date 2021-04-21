using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayer : Player
{
    public override void StartTurn()
    {
        base.StartTurn();
        TurnManager.instance.ShowCardSelection();
    }
}
