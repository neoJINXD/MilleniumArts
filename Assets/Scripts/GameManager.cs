using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameLoop gameLoop;
    
    void Start()
    {
        gameLoop = GameLoop.Instance;
        gameLoop.AddPlayer(gameObject.AddComponent<Player>());
        gameLoop.AddPlayer(gameObject.AddComponent<Player>());
        StartCoroutine(gameLoop.Play());
    }
}
