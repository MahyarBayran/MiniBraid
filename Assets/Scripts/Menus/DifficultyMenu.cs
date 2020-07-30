using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyMenu : MonoBehaviour
{
    public void HandleEasyButton()
    {
        AudioManager.PlayOneShot(AudioClipName.ButtonClick);
        ConfigurationUtils.SetDifficulty(Difficulty.Easy);
        MenuManager.GoToMenu(MenuName.Play);
    }

    public void HandleMediumButton()
    {
        AudioManager.PlayOneShot(AudioClipName.ButtonClick);
        ConfigurationUtils.SetDifficulty(Difficulty.Medium);
        MenuManager.GoToMenu(MenuName.Play);
    }

    public void HandleHardButton()
    {
        AudioManager.PlayOneShot(AudioClipName.ButtonClick);
        ConfigurationUtils.SetDifficulty(Difficulty.Hard);
        MenuManager.GoToMenu(MenuName.Play);
    }
}
