using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightTimeState
{
    public Vector3 position;
    public Vector3 linearVelocity;
    public bool facingRight;
    public bool jumping;
    public bool canMove;
    public bool hasKey;
    public bool isDead;
    public float jumpMultiplier;
    public bool pickedUpKey;
    public float keyAudioTimerSecondsLeft;
    public bool finishedJumpedOnEnemy;
    public float jumpOnEnemyTimerSecondsLeft;
}
