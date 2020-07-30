using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestoyAudio : MonoBehaviour
{
    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("BackAudio");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
            return;
        }

        AudioManager.Initialize(GetComponents<AudioSource>());
        DontDestroyOnLoad(this.gameObject);
    }
}
