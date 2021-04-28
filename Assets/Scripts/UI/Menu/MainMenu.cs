using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private SceneField m_onlineScene;
    [SerializeField] private SceneField m_aiScene;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject credits;

    public void OpenOnlineScene()
    {
        SceneManager.LoadScene(m_onlineScene);
    }
    
    public void OpenAIScene()
    {
        SceneManager.LoadScene(m_aiScene);
    }

    public void ShowMenu()
    {
        mainMenu.SetActive(true);
        credits.SetActive(false);
    }

    public void ShowCredits()
    {
        mainMenu.SetActive(false);
        credits.SetActive(true);
    }

    public void Quit()
    {
#if UNITY_STANDALONE
        Application.Quit();
#endif
 
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif    
    }
}
