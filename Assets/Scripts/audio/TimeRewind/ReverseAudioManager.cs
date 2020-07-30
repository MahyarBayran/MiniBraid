using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseAudioManager : MonoBehaviour
{
    [SerializeField]
    GameObject reverseAudioSourcePrefab;

    public void CreateReverseAudioSource(AudioClipName name)
    {
        GameObject ras = Instantiate<GameObject>(reverseAudioSourcePrefab);
        ras.GetComponent<ReverseAudioSource>().Initialize(
            AudioManager.GetAudioClip(name));
    }
}
