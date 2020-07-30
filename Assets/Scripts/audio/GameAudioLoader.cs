using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAudioLoader : MonoBehaviour
{
    void Awake()
    {
        AudioManager.LoadAudios();
    }
}
