using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    private void Start()
    {
        Time.timeScale = 0;
    }

    public void HandleRestartButton()
    {
        AudioManager.PlayOneShot(AudioClipName.ButtonClick);
        Destroy(gameObject);
        MenuManager.GoToMenu(MenuName.Play);
        Time.timeScale = 1;
    }
}
