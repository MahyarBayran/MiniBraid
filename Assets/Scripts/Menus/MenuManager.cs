using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MenuManager
{
    static Object prefabPauseMenu;
    static Object prefabWinMenu;
    static Object prefabGameOverMenu;

    static GameObject pauseMenu;

    public static void Initialize()
    {
        prefabPauseMenu = Resources.Load("Menus/PauseMenu") ;
        prefabWinMenu = Resources.Load("Menus/WinMenu");
        prefabGameOverMenu = Resources.Load("Menus/GameOverMenu");
    }

    public static void GoToMenu(MenuName name)
    {
        switch(name)
        {
            case MenuName.Difficulty:
                //SceneManager.LoadScene("DifficultyMenu");
                ConfigurationUtils.SetDifficulty(Difficulty.Hard);
                GoToMenu(MenuName.Play);
                break;

            case MenuName.Play:
                SceneManager.LoadScene("Level_1");
                break;

            case MenuName.Main:
                SceneManager.LoadScene("MainMenu");
                break;

            case MenuName.Help:
                SceneManager.LoadScene("HelpMenu");
                break;

            case MenuName.Pause:
                pauseMenu = Object.Instantiate(prefabPauseMenu) as GameObject;
                break;

            case MenuName.Win:
                Object.Instantiate(prefabWinMenu);
                break;

            case MenuName.Gameover:
                Object.Instantiate(prefabGameOverMenu);
                break;
        }
    }

    public static void ClosePauseMenu()
    {
        if (pauseMenu)
            pauseMenu.GetComponent<PauseMenu>().HandleResumeButtonOnClickEvent();
    }
}
