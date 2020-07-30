using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Chest : MonoBehaviour
{
    [SerializeField]
    Sprite openChestSprite;
    [SerializeField]
    Sprite closedChestSprite;

    SpriteRenderer spriteRenderer;

    // time rewind support
    [SerializeField]
    bool timeRewindable;
    List<ChestTimeState> states;
    bool isRewinding;
    bool isOpen;

    Timer finishedOpeningTimer;
    bool finishedOpening;

    ReverseAudioManager reverseAudioManager;

    // event support
    ChestOpenedEvent chestOpenedEvent;
    ChestClosedEvent chestClosedEvent;

    public bool IsOpen
    {
        get { return isOpen; }
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // time rewind setup
        states = new List<ChestTimeState>();

        chestClosedEvent = new ChestClosedEvent();
        chestOpenedEvent = new ChestOpenedEvent();

        finishedOpeningTimer = gameObject.AddComponent<Timer>();
        finishedOpeningTimer.AddTimerFinishedEventListener(FinishedOpeningTimerAction);

        reverseAudioManager = Camera.main.GetComponent<ReverseAudioManager>();

        // setup events
        EventManager.AddChestOpenedInvoker(this);
        EventManager.AddChestClosedInvoker(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        isOpen = false;
        isRewinding = false;

        finishedOpening = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeRewindable)
        {
            // check for rewinds
            if (Input.GetButtonDown("Fire1"))
                StartRewind();
            if (Input.GetButtonUp("Fire1"))
                StopRewind();

            if (isRewinding)
            {
                PopState();
            }
            else
            {
                PushState();

                // no 2 adjacent frames should have finishedOpening set to true
                finishedOpening = false;
            }
        }
    }

    public void Open()
    {
        if (!isOpen)
        {
            isOpen = true;
            spriteRenderer.sprite = openChestSprite;

            chestOpenedEvent.Invoke();

            finishedOpeningTimer.Duration = AudioManager.
                GetAudioLength(AudioClipName.ChestOpen);
            finishedOpeningTimer.Run();

            // play sound effects
            if (timeRewindable)
            {
                AudioManager.PlayOneShot(AudioClipName.ChestOpen);
            }
            else
            {
                AudioManager.PlayOneShotNonRewindable(AudioClipName.ChestOpen);
            }
        }
    }

    void Close()
    {
        if (isOpen)
        {
            isOpen = false;
            spriteRenderer.sprite = closedChestSprite;

            chestClosedEvent.Invoke();
        }
    }

    void StartRewind()
    {
        isRewinding = true;

        finishedOpeningTimer.Stop();
    }

    void StopRewind()
    {
        isRewinding = false;

        finishedOpeningTimer.Run();
    }

    void PushState()
    {
        ChestTimeState cts = new ChestTimeState();

        cts.isOpen = isOpen;
        cts.finishedOpening = finishedOpening;
        cts.openingTimerSecondsLeft = finishedOpeningTimer.SecondsLeft;

        states.Add(cts);
    }

    void PopState()
    {
        int lastIndex = states.Count - 1;

        if (lastIndex > -1)
        {
            ChestTimeState lastState = states[lastIndex];
            states.RemoveAt(lastIndex);

            if (!lastState.isOpen && isOpen)
            {
                Close();
            }

            if (lastState.finishedOpening)
            {
                reverseAudioManager.
                    CreateReverseAudioSource(AudioClipName.ChestOpen);
            }
            finishedOpening = lastState.finishedOpening;

            finishedOpeningTimer.Duration = lastState.openingTimerSecondsLeft;
        }
    }

    void FinishedOpeningTimerAction()
    {
        finishedOpening = true;
    }

    public void AddChestOpenedListener(UnityAction listener)
    {
        chestOpenedEvent.AddListener(listener);
    }

    public void AddChestClosedListener(UnityAction listener)
    {
        chestClosedEvent.AddListener(listener);
    }

}
