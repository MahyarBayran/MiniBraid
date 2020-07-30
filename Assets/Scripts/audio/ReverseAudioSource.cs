using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseAudioSource : MonoBehaviour
{
    Timer timer;
    AudioSource audioSource;

    List<ReverseAudioSourceState> states;
    bool isRewinding;

    // do we even need to store this?
    int sample;

    public void Initialize(AudioClip clip)
    {
        states = new List<ReverseAudioSourceState>();

        timer = GetComponent<Timer>();
        audioSource = GetComponent<AudioSource>();

        isRewinding = false;

        audioSource.clip = clip;

        audioSource.pitch = -1.0f;
        audioSource.timeSamples = audioSource.clip.samples - 1;
        audioSource.Play();

        timer.Duration = audioSource.clip.length;
        timer.Run();

        PushState();
    }

    // Update is called once per frame
    void Update()
    {
        // pay attention that this class works in the opposite
        // direrciton by default

        // check for rewinds
        if (Input.GetButtonUp("Fire1"))
            StartRewind();
        if (Input.GetButtonDown("Fire1"))
            StopRewind();

        if (isRewinding)
        {
            PopState();
        }
        else
        {
            PushState();
        }
    }

    void TimerFinishedAction()
    {
        Destroy(gameObject);
    }

    void StartRewind()
    {
        isRewinding = true;

        audioSource.pitch = 1.0f;

        timer.Stop();
    }

    void StopRewind()
    {
        isRewinding = false;

        audioSource.pitch = -1.0f;

        timer.Run();
    }

    void PushState()
    {
        ReverseAudioSourceState rass = new ReverseAudioSourceState();

        sample = audioSource.timeSamples;
        rass.sample = sample;
        rass.timerSecondsLeft = timer.SecondsLeft;

        states.Add(rass);
    }
    
    void PopState()
    {
        int lastIndex = states.Count - 1;

        if (lastIndex > -1)
        {
            ReverseAudioSourceState prevState = states[lastIndex];
            states.RemoveAt(lastIndex);

            sample = prevState.sample;
            timer.Duration = prevState.timerSecondsLeft;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
