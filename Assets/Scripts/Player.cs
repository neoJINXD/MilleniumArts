using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Player : MonoBehaviour
{
    private bool turnComplete;

    public void StartTurn()
    {
        turnComplete = false;
    }

    public Task AwaitTurn()
    {
        while (!turnComplete)
        {}
        
        return null;
    }
}
