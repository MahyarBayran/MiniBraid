using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    float minPositionX = 0.0f;
    float maxPositionX = 100.0f;

    Rigidbody2D rb2d;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClampCameraPositionX()
    {
        float posX = rb2d.position.x;
        posX = Math.Min(Math.Max(minPositionX, posX), maxPositionX);
        rb2d.position = new Vector2(posX, rb2d.position.y);
    }
}
