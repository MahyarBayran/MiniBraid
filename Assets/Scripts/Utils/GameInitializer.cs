using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ScreenUtils.Initialize();
        MenuManager.Initialize();
        ConfigurationUtils.Initialize();

        // playbackground music
        AudioManager.PlayBackground(AudioClipName.BackgroundMenu);
    }
}
