using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using UnityEngine;

/// <summary>
/// The audio manager
/// </summary>
public static class AudioManager
{
    // does not handle one shot audios which start 
    // playing in reverse from the end of the clip

    static AudioSource audioSource;
    static AudioSource audioSourceBackground;
    static AudioSource audioSourceNonRewindable;

    static Dictionary<AudioClipName, AudioClip> audioClips =
        new Dictionary<AudioClipName, AudioClip>();

    /// <summary>
    /// Initializes the audio manager
    /// </summary>
    /// <param name="source">audio source</param>
    public static void Initialize(AudioSource[] sources)
    {
        audioSource = sources[0];
        audioSourceBackground = sources[1];
        audioSourceNonRewindable = sources[2];
    }

    public static void LoadAudios()
    {
        if (audioClips.Count > 0)
            return; 

        audioClips.Add(AudioClipName.ButtonClick,
            Resources.Load<AudioClip>("Audio/ButtonClick"));
        audioClips.Add(AudioClipName.KeyPickUp,
            Resources.Load<AudioClip>("Audio/DropKeys"));
        audioClips.Add(AudioClipName.ChestOpen,
            Resources.Load<AudioClip>("Audio/OpenDoor"));
        audioClips.Add(AudioClipName.BackgroundMenu,
            Resources.Load<AudioClip>("Audio/FreeAdventureMusic/Bards_Solo__1_Min_Harp"));
        audioClips.Add(AudioClipName.BackgroundLevel01,
            Resources.Load<AudioClip>("Audio/FreeAdventureMusic/An_Adventurous_Tale"));
        audioClips.Add(AudioClipName.Win,
            Resources.Load<AudioClip>("Audio/FreeAdventureMusic/Final_Glorious_Triumph__10s"));
        audioClips.Add(AudioClipName.JumpOnEnemy,
            Resources.Load<AudioClip>("Audio/Jump_On_Enemy"));
    }

    /// <summary>
    /// Plays the audio clip with the given name
    /// </summary>
    /// <param name="name">name of the audio clip to play</param>
    public static void PlayOneShot(AudioClipName name)
    {
        audioSource.PlayOneShot(audioClips[name]);
    }

    public static void PlayOneShotNonRewindable(AudioClipName name)
    {
        audioSourceNonRewindable.PlayOneShot(audioClips[name]);
    }

    public static void PlayBackground(AudioClipName name)
    {
        if (!audioSourceBackground.isPlaying || audioSourceBackground.clip == null)
        {
            audioSourceBackground.loop = true;
            audioSourceBackground.clip = audioClips[name];
            audioSourceBackground.Play();
        }
    }

    public static void Stop()
    {
        audioSource.Stop();
    }

    public static void StopBackground()
    {
        audioSourceBackground.Stop();
    }

    public static void Pause()
    {
        audioSourceBackground.Pause();
        audioSource.Pause();
    }

    public static void UnPause()
    {
        audioSourceBackground.UnPause();
        audioSource.UnPause();
    }

    public static void SetPitch(float pitch)
    {
        audioSource.pitch = pitch;
        audioSourceBackground.pitch = pitch;
    }

    public static float GetAudioLength(AudioClipName name)
    {
        return audioClips[name].length;
    }

    public static AudioClip GetAudioClip(AudioClipName name)
    {
        if (audioClips.ContainsKey(name))
        {
            return audioClips[name];
        }
        else
            return null;
    }
}
