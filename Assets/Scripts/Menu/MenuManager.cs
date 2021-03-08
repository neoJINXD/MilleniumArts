using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Zone.Core.Utils;

public class MenuManager : Singleton<MenuManager>
{
    [SerializeField] private Menu[] menus;
    private void Start()
    {
        if (menus.Length == 0) 
        {
            menus = GetComponentsInChildren<Menu>();
        }
    }

    public void OpenMenu(string name)
    {
        var query = menus.Where(item => item.Name == name).ToArray().First();
        OpenMenu(query);
    }
    public void CloseMenu(string name)
    {
        var query = menus.Where(item => item.Name == name).ToArray().First();
        CloseMenu(query);
    }
    
    private void OpenMenu(Menu menu) => menu.Open();

    private void CloseMenu(Menu menu) => menu.Close();

    public void QuitGame()
    {
        // From https://forum.unity.com/threads/how-to-detect-application-quit-in-editor.344600/
        // If we are running in a standalone build of the game
#if UNITY_STANDALONE
        // Quit the application
        Application.Quit();
#endif
 
        // If we are running in the editor
#if UNITY_EDITOR
        // Stop playing the scene
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
