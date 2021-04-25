using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private SceneField m_onlineScene;
    [SerializeField] private SceneField m_aiScene;

    public void OpenOnlineScene()
    {
        SceneManager.LoadScene(m_onlineScene);
    }
    
    public void OpenAIScene()
    {
        SceneManager.LoadScene(m_aiScene);
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
