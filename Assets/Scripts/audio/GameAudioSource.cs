using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An audio source for the entire game
/// </summary>
public class GameAudioSource : MonoBehaviour
{
	/// <summary>
	/// Awake is called before Start
	/// </summary>
	void Awake()
    {
        //AudioSource audioSourceEffects = GetComponents<AudioSource>()[0];
        //AudioSource audioSourceBackground = GetComponents<AudioSource>()[1];

        AudioManager.Initialize(GetComponents<AudioSource>());
    }
}
