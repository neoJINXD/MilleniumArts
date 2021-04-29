using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            pausePanel.SetActive(true);

    }

    public void Cancel()
    {
        pausePanel.SetActive(false);
    }


    public void LeaveGame()
    {
        if (GameManager.instance.networked)
        {
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene("Main");
        }
        else
        {
            SceneManager.LoadScene("Main");
        }
    }
}
