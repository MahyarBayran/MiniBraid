using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    // spawn support
    [SerializeField]
    GameObject prefabEnemy;

    float spawnOffSetX = 0;
    float spawnOffSetY = 0;

    [SerializeField]
    bool spawnFacingLeft;

    Timer spawnTimer;

    int spawnCounter;

    // time rewind support
    [SerializeField]
    bool timeRewindable;
    List<EnemySpawnerTimeState> states = new List<EnemySpawnerTimeState>();
    bool rewinding;

    private void Awake()
    {
        // setup spawn offset
        BoxCollider2D col2d = GetComponent<BoxCollider2D>();
        spawnOffSetY = col2d.size.y / 2 * transform.localScale.y;

        // get the dimension of the enemy and use for spawn position offset
        GameObject tmp = Instantiate(prefabEnemy);
        CapsuleCollider2D tmpCol = tmp.GetComponent<CapsuleCollider2D>();
        spawnOffSetY += tmpCol.size.y;
        spawnOffSetY += tmpCol.offset.y;
        Destroy(tmp);

        // setup spawn timer
        spawnTimer = gameObject.AddComponent<Timer>();
        spawnTimer.AddTimerFinishedEventListener(TimerFinishedAction);

        // setup spawn event
        EventManager.AddEnemyDeathListener(this, EnemyDiedAction);
        EventManager.AddEnemyResurrectListener(this, EnemyResurrectedAction);

        // time rewind support
        states = new List<EnemySpawnerTimeState>();
    }

    // Start is called before the first frame update
    void Start()
    {
        spawnCounter = 0;

        rewinding = false;

        TimerFinishedAction();
    }

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
            }
            else
            {
                PushState();
            }
        }
    }

    /// <summary>
    /// spawns an enemy and resets the spawn timer
    /// </summary>
    void SpawnEnemy()
    {
        Vector3 position = transform.position;
        position.x += spawnOffSetX;
        position.y += spawnOffSetY;
        GameObject enemy = Instantiate(prefabEnemy, position, Quaternion.identity);
        Enemy enemyComponent = enemy.GetComponent<Enemy>();
        enemyComponent.setMoveDirection(spawnFacingLeft);
        enemyComponent.setSpawner(this);

        spawnCounter++;

        ResetSpawnTimer();
    }

    /// <summary>
    /// action taken when the spawn timer is finished
    /// </summary>
    void TimerFinishedAction()
    {
        // spawn only if spawner is not saturated
        if (spawnCounter < ConfigurationUtils.MaxEnemiesPerSpawner)
            SpawnEnemy();
    }

    /// <summary>
    /// Resets the timer for spawning
    /// </summary>
    void ResetSpawnTimer()
    {
        spawnTimer.Duration = ConfigurationUtils.SpawnInterval;
        spawnTimer.Run();
    }

    /// <summary>
    /// Listener for enemy death event
    /// </summary>
    void EnemyDiedAction()
    {
        spawnCounter--;

        if (!spawnTimer.Running)
        {
            ResetSpawnTimer();
        }
    }

    /// <summary>
    /// Listener for enemy resurrect event
    /// </summary>
    void EnemyResurrectedAction()
    {
        spawnCounter++;
    }

    /// <summary>
    /// let the gameObject know we're rewinding in time
    /// </summary>
    void StartRewind()
    {
        rewinding = true;

        spawnTimer.Stop();
    }

    /// <summary>
    /// let the gameObject know we've stopped rewinding
    /// </summary>
    void StopRewind()
    {
        rewinding = false;

        spawnTimer.Run();
    }

    /// <summary>
    /// record the state of the gameObject
    /// </summary>
    void PushState()
    {
        EnemySpawnerTimeState ests = new EnemySpawnerTimeState();

        ests.timerSecondsLeft = spawnTimer.SecondsLeft;
        ests.spawnCounter = spawnCounter;

        states.Add(ests);
    }

    /// <summary>
    /// Rewind the state by 1 stored frame
    /// </summary>
    void PopState()
    {
        int lastIndex = states.Count - 1;
        if (lastIndex > -1)
        {
            EnemySpawnerTimeState lastState = states[lastIndex];
            states.RemoveAt(lastIndex);

            spawnTimer.Duration = lastState.timerSecondsLeft;
            spawnCounter = lastState.spawnCounter;
        }
    }
}