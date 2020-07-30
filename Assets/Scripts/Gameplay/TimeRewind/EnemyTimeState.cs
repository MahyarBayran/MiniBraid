using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTimeState
{
    public Vector3 position;
    public float rotation;
    public Vector3 linearVelocity;
    public float angularVelocity;
    public float moveTimerSecondsLeft;
    public bool isDead;
    public bool isReallyDead;
    public bool movingLeft;
}
