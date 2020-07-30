using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinMenu : MonoBehaviour
{
    private void Start()
    {
        Time.timeScale = 0;
    }
    public void HandleRestartButton()
    {
        AudioManager.PlayOneShot(AudioClipName.ButtonClick);
        Time.timeScale = 1;
        Destroy(gameObject);
        MenuManager.GoToMenu(MenuName.Play);
    }

    public void HandleGoBackButton()
    {
        AudioManager.PlayOneShot(AudioClipName.ButtonClick);
        Time.timeScale = 1;
        Destroy(gameObject);
        MenuManager.GoToMenu(MenuName.Main);
    }
}
