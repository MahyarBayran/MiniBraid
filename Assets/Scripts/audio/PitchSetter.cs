using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitchSetter : MonoBehaviour
{

    bool isRewinding = false; 

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.SetPitch(1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        // check for rewinds
        if (Input.GetButtonDown("Fire1"))
            StartRewind();
        if (Input.GetButtonUp("Fire1"))
            StopRewind();

        if (isRewinding)
        {
            AudioManager.SetPitch(-1.0f);
        }
        else
        {
            AudioManager.SetPitch(1.0f);
        }
    }

    void StartRewind()
    {
        isRewinding = true;
    }

    void StopRewind()
    {
        isRewinding = false;
    }
}
