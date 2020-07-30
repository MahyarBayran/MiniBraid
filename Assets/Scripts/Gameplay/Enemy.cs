using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    Animator animator;

    // Event Support
    KnightDeathEvent knightDeathEvent;
    EnemyDeathEvent enemyDeathEvent;
    EnemyResurrectEvent enemyResurrectEvent;

    // the spawner that has generated this enemy
    EnemySpawner spawner = null;

    bool movingLeft;
    float movingMag = 2.0f;

    float halfColHeight;
    float halfColWidth;
    float colliderOffsetY;
    float colliderOffsetX;

    Rigidbody2D rb2d;
    CapsuleCollider2D col2D;

    // timer for moving the enemy
    Timer moveTimer;
    float startMovingTime = 1.0f;

    // time rewind support
    // states in frames
    [SerializeField]
    bool timeRewindable;
    List<EnemyTimeState> states;
    bool rewinding;
    bool isKinematicInitially;
    bool isDead;
    bool isReallyDead;
    float moveTimerSecondsLeft;

    private void Awake()
    {
        // setup events
        //knightDeathEvent = new KnightDeathEvent();
        //EventManager.AddKnightDeathInvoker(this);
        enemyDeathEvent = new EnemyDeathEvent();
        enemyResurrectEvent = new EnemyResurrectEvent();

        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        isKinematicInitially = rb2d.isKinematic;

        col2D = GetComponent<CapsuleCollider2D>();
        halfColHeight = col2D.size.y / 2;
        halfColWidth = col2D.size.x / 2;
        colliderOffsetY = col2D.offset.y;
        colliderOffsetX = col2D.offset.x;

        moveTimer = gameObject.AddComponent<Timer>();
        moveTimer.AddTimerFinishedEventListener(StartMoving);

        // rewind setup
        states = new List<EnemyTimeState>();

        // make the enemies be at slightly different depths
        float offsetZ = Random.Range(-0.01f, 0.01f);
        Vector3 position = transform.position + offsetZ * Vector3.forward;
        transform.position = position;
    }

    // Start is called before the first frame update
    void Start()
    {
        moveTimer.Duration = startMovingTime;
        moveTimer.Run();

        isDead = false;
        isReallyDead = false;
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

            if (rewinding)
            {
                PopState();

                animator.SetFloat("TimeMultiplier", -1.0f);
            }
            else
            {
                PushState();

                animator.SetFloat("TimeMultiplier", 1.0f);
            }
        }

        animator.SetBool("isDead", isDead);
        animator.SetBool("isReallyDead", isReallyDead);
    }


    public void setSpawner(EnemySpawner spawner)
    {
        // only set spawner if not assigned already
        if (this.spawner == null)
        {
            this.spawner = spawner;
            EventManager.AddEnemyDeathInvoker(spawner, this);
            EventManager.AddEnemyResurrectInvoker(spawner, this);
        }
    }

    public void setMoveDirection(bool left)
    {
        movingLeft = left;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (rewinding)
            return;

        //if (collision.gameObject.CompareTag("Knight"))
        //{
        //    // check to see if the collision position is on the top or the sides
        //    if ((collision.GetContact(0).point.y
        //         - transform.position.y - colliderOffsetY) > halfColHeight * 0.8f)
        //    {
        //        Die();
        //    }
        //    else
        //    {
        //        knightDeathEvent.Invoke();
        //    }
        //}
        //else
        if (!collision.gameObject.CompareTag("Knight") &&
            Mathf.Abs(collision.GetContact(0).point.x
                 - transform.position.x - colliderOffsetX) > halfColWidth * 0.9f)
        // check direction if collision on the sides
        {
            // change moving direction
            movingLeft = !movingLeft;
            rb2d.velocity = Vector3.zero;
            StartMoving();

        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (rewinding)
            return;

        if (collision.gameObject.CompareTag("DieZone"))
        {
            ReallyDie();
        }
    }

    void StartMoving()
    {
        Vector2 movingDirection;
        if (movingLeft)
            movingDirection = Vector2.left;
        else
            movingDirection = Vector2.right;

        rb2d.AddForce(movingDirection * movingMag, ForceMode2D.Impulse);
    }

    public void Die()
    {
        // let the object fall
        col2D.isTrigger = true;
        isDead = true;
    }

    void Resurrect()
    {
        col2D.isTrigger = false;
        isDead = false;
    }

    void ReallyDie()
    {
        if (!isReallyDead)
        {
            // manage event
            enemyDeathEvent.Invoke();

            // do not destroy because of time rewind, instead set inactive
            rb2d.Sleep();
            rb2d.gravityScale = 0;

            isReallyDead = true;

            // remove from event invokers
            EventManager.RemoveEnemyDeathInvoker(spawner, this);
            EventManager.RemoveEnemyResurrectInvoker(spawner, this);
        }
    }

    void ReallyResurrect()
    {
        if (isReallyDead)
        {
            // manage event
            enemyResurrectEvent.Invoke();

            rb2d.WakeUp();
            rb2d.gravityScale = 1;

            isReallyDead = false;

            // add it again to the event invokers
            EventManager.AddEnemyDeathInvoker(spawner, this);
            EventManager.AddEnemyResurrectInvoker(spawner, this);
        }
    }

    public void AddDeathEventListener(UnityAction listener)
    {
        knightDeathEvent.AddListener(listener);
    }

    public void AddEnemyDeathListener(UnityAction listener)
    {
        enemyDeathEvent.AddListener(listener);
    }

    public void AddEnemyResurrectListener(UnityAction listener)
    {
        enemyResurrectEvent.AddListener(listener);
    }

    /// <summary>
    /// Adds the current state to the list
    /// </summary>
    void PushState()
    {
        EnemyTimeState ets = new EnemyTimeState();

        ets.position = rb2d.position;
        ets.rotation = rb2d.rotation;
        ets.linearVelocity = rb2d.velocity;
        ets.angularVelocity = rb2d.angularVelocity;
        ets.isDead = isDead;
        ets.isReallyDead = isReallyDead;
        ets.moveTimerSecondsLeft = moveTimer.SecondsLeft;
        ets.movingLeft = movingLeft;

        states.Add(ets);
    }

    /// <summary>
    /// Sets the files of the gameObject according to the last stored state
    /// </summary>
    void PopState()
    {
        int lastIndex = states.Count - 1;
        if (lastIndex > -1)
        {
            // pop the state from the list
            EnemyTimeState lastState = states[lastIndex];
            states.RemoveAt(lastIndex);

            // Resurrect if currently dead
            if (isReallyDead & !lastState.isReallyDead)
            {
                ReallyResurrect();
            } 
            else if (isDead && !lastState.isDead)
            {
                Resurrect();
            }

            // set the fields according to the current state
            rb2d.position = lastState.position;
            rb2d.rotation = lastState.rotation;
            rb2d.velocity = lastState.linearVelocity;
            rb2d.angularVelocity = lastState.angularVelocity;
            isDead = lastState.isDead;
            isReallyDead = lastState.isReallyDead;
            moveTimer.Duration = lastState.moveTimerSecondsLeft;
            movingLeft = lastState.movingLeft;
        }
        else
        {
            // no states left ...
            // Despawn if gameObject has a spawner
            if (spawner)
            {
                // vanish
                Destroy(gameObject);
                //EventManager.RemoveKnightDeathInvoker(this);
                EventManager.RemoveEnemyDeathInvoker(spawner, this);
            }
        }
    }
    void StartRewind()
    {
        rewinding = true;
        rb2d.isKinematic = true;
        col2D.enabled = false;

        moveTimer.Stop();
    }

    void StopRewind()
    {
        rewinding = false;
        rb2d.isKinematic = isKinematicInitially;
        col2D.enabled = true;

        // if seconds left is zero, this is ignored
        moveTimer.Run();
    }
}
