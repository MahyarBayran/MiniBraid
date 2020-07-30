using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Listens for the OnClick events for the main menu buttons
/// </summary>
public class MainMenu : MonoBehaviour
{
    /// <summary>
    /// Handles the on click event from the play button
    /// </summary>
    public void HandlePlayButtonOnClickEvent()
    {
        AudioManager.PlayOneShot(AudioClipName.ButtonClick);
        AudioManager.StopBackground();
        AudioManager.PlayBackground(AudioClipName.BackgroundLevel01);
        MenuManager.GoToMenu(MenuName.Difficulty);
    }

    /// <summary>
    /// Handles the on click event from the help button
    /// </summary>
    public void HandleHelpButtonOnClickEvent()
    {
        AudioManager.PlayOneShot(AudioClipName.ButtonClick);
        MenuManager.GoToMenu(MenuName.Help);
    }

    /// <summary>
    /// Handles the on click event from the quit button
    /// </summary>
    public void HandleQuitButtonOnClickEvent()
    {
        AudioManager.PlayOneShot(AudioClipName.ButtonClick);
        Application.Quit();
    }
} 
