using UnityEngine;
using Zone.Core.Utils;
using Photon.Pun;

public class DestroyMe : MonoBehaviour
{
    private Timer timer;

    void Start()
    {
        timer = new Timer(0.5f);
        timer.OnTimerEnd += Die;
    }

    void Update()
    {
        timer.Tick(Time.deltaTime);
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
