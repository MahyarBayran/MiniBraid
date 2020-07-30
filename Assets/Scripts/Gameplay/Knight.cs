using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Animator))]
public class Knight : MonoBehaviour
{
    [SerializeField]
    GameObject mainCamera;

    // movement support
    float moveMagnitude = 10.0f;
    float jumpPower = 7.5f;
    float jumpMultiplierConst = 1.35f;
    float jumpMultiplier;
    float maxJumpMultiplier;
    bool facingRight;
    bool jumping;
    bool canMove;
    float inAirMovementFactor = 0.5f;

    Rigidbody2D rb2d;
    Rigidbody2D camrb2d;
    float halfColWidth;
    float halfColHeight;
    float colOffsetX;
    float colOffSetY;
    CapsuleCollider2D col2D;

    // animation support
    Animator animator;

    // gameplay support
    GameObject keyObject;
    bool hasKey;

    int numOfChestsToOpen;
    int numOfChestsOpened;

    // time rewind support
    // states in frames
    List<KnightTimeState> states;
    bool rewinding;
    bool isKinematicInitially;
    bool isDead;
    bool audioIsPaused;
    
    Timer keyAudioTimer;
    float keyAudioTimerSecondsLeft;
    bool pickedUpKey;
    Timer jumpOnEnemyTimer;
    float jumpOnEnemyTimerSecondsLeft;
    bool finishedJumpedOnEnemy;

    ReverseAudioManager reverseAudioManager;

    // HUD
    [SerializeField]
    Text chestRemainingText;
    GameObject rewindAlertTextPrefab;
    GameObject rewindAlertText;
    bool pauseMenuPresent;

    void Awake()
    {
        // setup components
        rb2d = GetComponent<Rigidbody2D>();
        camrb2d = mainCamera.GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // setup events
        EventManager.AddChestOpenedListener(ChestOpenedAction);
        EventManager.AddChestClosedListener(ChestClosedAction);

        // time rewind
        states = new List<KnightTimeState>();
        isKinematicInitially = rb2d.isKinematic;
        rewindAlertTextPrefab = Resources.Load("Menus/RewindMenu") as GameObject;

        reverseAudioManager = Camera.main.GetComponent<ReverseAudioManager>();

        // setup timers
        keyAudioTimer = gameObject.AddComponent<Timer>();
        keyAudioTimer.AddTimerFinishedEventListener(KeyAudioTimerAction);
        jumpOnEnemyTimer = gameObject.AddComponent<Timer>();
        jumpOnEnemyTimer.AddTimerFinishedEventListener(JumpOnEnemyTimerAction);
    }

    // Start is called before the first frame update
    void Start()
    {
        col2D = GetComponent<CapsuleCollider2D>();
        halfColWidth = col2D.size.x / 2;
        halfColHeight = col2D.size.y / 2;
        colOffsetX = col2D.offset.x;
        colOffSetY = col2D.offset.y;

        facingRight = true;
        jumping = false;
        canMove = true;
        hasKey = false;
        isDead = false;
        audioIsPaused = false;
        finishedJumpedOnEnemy = false;
        pickedUpKey = false;

        jumpMultiplier = jumpMultiplierConst;
        maxJumpMultiplier = jumpMultiplierConst * jumpMultiplier * jumpMultiplier;

        // Setup time rewind
        // add the initial state 
        PushState();
        // time should move forward at first
        StopRewind();

        // reset timers
        keyAudioTimer.Duration = 0.0f;
        jumpOnEnemyTimer.Duration = 0.0f;

        // setup gameplay
        numOfChestsToOpen = GameObject.FindGameObjectsWithTag("Chest").Count();
        numOfChestsOpened = 0;

        pauseMenuPresent = false;
    }

    // Update is called once per frame
    void Update()
    {
        // check for rewinds
        if (Input.GetButtonDown("Fire1"))
            StartRewind();
        if (Input.GetButtonUp("Fire1"))
            StopRewind();

        // if rewinding ignore further inputs
        if (!rewinding)
        {
            if (Input.GetButtonDown("Jump") && canMove)
            {
                if (!jumping)
                {
                    Jump();
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (pauseMenuPresent)
                {
                    pauseMenuPresent = false;
                    MenuManager.ClosePauseMenu();
                }
                else
                {
                    pauseMenuPresent = true;
                    MenuManager.GoToMenu(MenuName.Pause);
                }
            }

            animator.SetFloat("TimeMultiplier", 1.0f);

            if (!isDead)
                // record time state
                PushState();

            // if this was true, it's already stored in the states
            finishedJumpedOnEnemy = false;

            if (audioIsPaused)
            {
                AudioManager.UnPause();
                audioIsPaused = false;
            }
       
        }
        else
        {
            animator.SetFloat("TimeMultiplier", -1.0f);

            // rewind time
            PopState();
        }

        // Update animation
        animator.SetBool("Jumping", jumping);
        animator.SetBool("isDead", isDead);
    }

    void FixedUpdate()
    {
        if (!rewinding)
        {
            float moveInput = Input.GetAxis("Horizontal");

            if (moveInput != 0 && canMove)
            {
                if (!jumping)
                {
                    float forceMag = moveInput * moveMagnitude;
                    rb2d.AddForce(Vector2.right * forceMag, ForceMode2D.Force);
                }
                else
                {
                    float forceMag = moveInput * moveMagnitude * inAirMovementFactor;
                    rb2d.AddForce(Vector2.right * forceMag, ForceMode2D.Force);
                }

                facingRight = moveInput > 0;
            }
        }

        if (facingRight)
            transform.eulerAngles = new Vector3(0, 0);
        else
            transform.eulerAngles = new Vector3(0, 180);

        // move the camera as the player moves
        camrb2d.position = new Vector2(rb2d.position.x, camrb2d.position.y);
        mainCamera.GetComponent<CameraController>().ClampCameraPositionX();

        // update animation
        animator.SetFloat("SpeedX", Mathf.Abs(rb2d.velocity.x));
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // just for safety
        if (rewinding)
            return;

        if (collision.gameObject.CompareTag("Ground"))
        {
            // stop jump if the collision is on the bottom
            if (collision.contacts[0].point.y
                - transform.position.y - colOffSetY < -0.9f * halfColHeight)
            {
                StopJump();
            }
        } 
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            // jump if the collision is on the bottom
            if (collision.contacts[0].point.y + - transform.position.y 
                - colOffSetY < -0.75f * halfColHeight)
            {
                JumpOnEnemy();

                collision.gameObject.GetComponent<Enemy>().Die();

                // start timer
                ResetJumpOnEnemyTimer();
                jumpOnEnemyTimer.Run();
            }
            else 
            {
                Die();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // just for safety
        if (rewinding)
            return;

        if (collision.gameObject.CompareTag("Chest") && hasKey)
        {
            Chest chest = collision.gameObject.GetComponent<Chest>();
            if (!chest.IsOpen)
            {
                // manage chest
                chest.Open();

                // manage key
                Key keyComp = keyObject.GetComponent<Key>();
                keyComp.GetBroken();
                keyComp.RemoveOwner();
                hasKey = false;
            }
        }
        else if (collision.gameObject.CompareTag("Key"))
        {
            // pickup the key
            keyObject = collision.gameObject;
            Key keyComp = keyObject.GetComponent<Key>();

            if (!keyComp.IsBroken)
            {
                keyComp.SetOwner();
                hasKey = true;
                pickedUpKey = true;

                // start timer
                ResetKeyAudioTimer();
                keyAudioTimer.Run();
            }
        }
        else if (collision.gameObject.CompareTag("DieZone"))
        {
            Die();
        }
    }

    void Die()
    {
        canMove = false;

        rb2d.Sleep();
        isDead = true;

        // disable collider
        col2D.enabled = false;
        rb2d.gravityScale = 0;

        // tell player to rewind time
        rewindAlertText = Instantiate(rewindAlertTextPrefab);

        Time.timeScale = 0;

        AudioManager.Pause();
    }

    void Resurrect()
    {
        canMove = true;

        rb2d.WakeUp();

        // disable collider because it's rewinding
        col2D.enabled = true;
        rb2d.gravityScale = 1;

        // delete alert menu
        if (rewindAlertText)
        {
            Destroy(rewindAlertText);
        }

        Time.timeScale = 1;
        AudioManager.UnPause();
    }

    void Jump()
    {
        // add a vertical impulse
        rb2d.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);

        jumping = true;
    }

    void JumpOnEnemy()
    {
        // add a vertical impulse
        rb2d.velocity = Vector2.right * rb2d.velocity.x;
        rb2d.AddForce(Vector2.up * jumpPower * jumpMultiplier, ForceMode2D.Impulse);

        jumpMultiplier *= jumpMultiplierConst;
        jumpMultiplier = Mathf.Min(jumpMultiplier, maxJumpMultiplier);

        jumping = true;

        // play audio
        AudioManager.PlayOneShot(AudioClipName.JumpOnEnemy);
    }

    void StopJump()
    {
        jumping = false;
        jumpMultiplier = jumpMultiplierConst;
    }

    void StartRewind()
    {
        rewinding = true;
        rb2d.isKinematic = true;
        col2D.enabled = false;

        if (isDead & !animator.IsInTransition(0))
        {
            animator.Play("Base Layer.Knight_Die");
        }

        keyAudioTimer.Stop();
        jumpOnEnemyTimer.Stop();
    }

    void StopRewind()
    {
        rewinding = false;
        rb2d.isKinematic = isKinematicInitially;
        col2D.enabled = true;

        // Knight_Die doesn't loop
        if (isDead)
        {
            animator.Play("Base Layer.Knight_Die");
        }

        keyAudioTimer.Run();
        jumpOnEnemyTimer.Run();
    }

    /// <summary>
    /// Adds the current state to the list
    /// </summary>
    void PushState()
    {
        KnightTimeState kts = new KnightTimeState();
        kts.position = rb2d.position;
        kts.linearVelocity = rb2d.velocity;
        kts.facingRight = facingRight;
        kts.jumping = jumping;
        kts.canMove = canMove;
        kts.hasKey = hasKey;
        kts.isDead = isDead;
        kts.jumpMultiplier = jumpMultiplier;
        kts.finishedJumpedOnEnemy = finishedJumpedOnEnemy;
        kts.keyAudioTimerSecondsLeft = keyAudioTimer.SecondsLeft;
        kts.jumpOnEnemyTimerSecondsLeft = jumpOnEnemyTimer.SecondsLeft;
        kts.pickedUpKey = pickedUpKey;

        states.Add(kts);
    }

    /// <summary>
    /// Sets the files of the gameObject according to the last stored state
    /// </summary>
    void PopState()
    {
        if (states.Count > 0)
        {
            // pop the state from the list
            int lastIndex = states.Count - 1;
            KnightTimeState prevState = states[lastIndex];
            states.RemoveAt(lastIndex);

            // Resurrect if currently dead
            if (isDead && !prevState.isDead)
            {
                Resurrect();
            }

            // set the fields according to the current state
            rb2d.position = prevState.position;
            rb2d.velocity = prevState.linearVelocity;
            facingRight = prevState.facingRight;
            jumping = prevState.jumping;
            canMove = prevState.canMove;
            isDead = prevState.isDead;
            jumpMultiplier = prevState.jumpMultiplier;

            keyAudioTimer.Duration = prevState.keyAudioTimerSecondsLeft;
            jumpOnEnemyTimer.Duration = prevState.jumpOnEnemyTimerSecondsLeft;
            
            if (hasKey && !prevState.hasKey)
            {
                // put the key back
                keyObject.GetComponent<Key>().RemoveOwner();
            }
            hasKey = prevState.hasKey;

            if (prevState.finishedJumpedOnEnemy && !finishedJumpedOnEnemy)
            {
                // Audio in reverse
                reverseAudioManager.
                CreateReverseAudioSource(AudioClipName.JumpOnEnemy);
            }
            finishedJumpedOnEnemy = prevState.finishedJumpedOnEnemy;

            if (prevState.pickedUpKey && !pickedUpKey)
            {
                // Audio in reverse
                reverseAudioManager.
                CreateReverseAudioSource(AudioClipName.KeyPickUp);
            }
            pickedUpKey = prevState.pickedUpKey;

        }
        else 
        {
            // pause audio
            AudioManager.Pause();
            audioIsPaused = true;

            // do nothing, like nothing is moving
        }

    }

    void ChestOpenedAction()
    {
        numOfChestsOpened++;

        int numOfChestsLeft = numOfChestsToOpen - numOfChestsOpened;

        chestRemainingText.text = "Chests remaining = " 
            + numOfChestsLeft;

        // check for win
        if (numOfChestsOpened == numOfChestsToOpen)
        {
            canMove = false;
            rb2d.Sleep();
            MenuManager.GoToMenu(MenuName.Win);

            // manage audio for win
            AudioManager.StopBackground();
            AudioManager.PlayOneShot(AudioClipName.Win);
        }
    }

    void ChestClosedAction()
    {
        numOfChestsOpened--;

        chestRemainingText.text = "Chests remaining = "
            + (numOfChestsToOpen - numOfChestsOpened);
    }

    void ResetKeyAudioTimer()
    {
        keyAudioTimer.Duration = AudioManager.GetAudioLength(AudioClipName.KeyPickUp);
    }

    void KeyAudioTimerAction()
    {
        pickedUpKey = false;
    }

    void ResetJumpOnEnemyTimer()
    {
        jumpOnEnemyTimer.Duration = AudioManager.GetAudioLength(AudioClipName.JumpOnEnemy);
    }

    void JumpOnEnemyTimerAction()
    {
        finishedJumpedOnEnemy = true;
    }
}
