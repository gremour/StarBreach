// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    [Tooltip("Distance ship is moving per second in world coords")]
    [SerializeField] float maneurability = 10;

    [Tooltip("Maximum rotation angle around Z axis when moving sideways")]
    [SerializeField] float maxRotationAngle = 30;

    [Tooltip("Rotation speed in degrees per second")]
    [SerializeField] float rotationSpeed = 180;


    // Game reference
    Game game;
    // Target ship rotation when moving
    float targetRotationAngle;
    // Direction vectors
    Vector3 dirLeft = new Vector3(-1, 0, 0);
    Vector3 dirRight = new Vector3(1, 0, 0);
    Vector3 dirForward = new Vector3(0, 0, 1);
    Vector3 dirBack = new Vector3(0, 0, -1);

    // Start is called before the first frame update
    void Start()
    {
        game = Game.instance;
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
        UpdateRotation();
    }

    void ProcessInput()
    {
        var movingForward = Input.GetKey("w") || Input.GetKey(KeyCode.UpArrow);
        var movingBack = Input.GetKey("s")|| Input.GetKey(KeyCode.DownArrow);
        var movingLeft = Input.GetKey("a") || Input.GetKey(KeyCode.LeftArrow);
        var movingRight = Input.GetKey("d") || Input.GetKey(KeyCode.RightArrow);

        // Move the ship
        if (movingForward)
        {
            Move(dirForward * maneurability * Time.deltaTime);
        }
        if (movingBack)
        {
            Move(dirBack * maneurability * Time.deltaTime);
        }
        if (movingLeft)
        {
            Move(dirLeft * maneurability * Time.deltaTime);
        }
        if (movingRight)
        {
            Move(dirRight * maneurability * Time.deltaTime);
        }
        LimitPositionToBounds();

        // Set target rotation angle
        if (movingLeft != movingRight)
        {
            if (movingLeft) {
                targetRotationAngle = maxRotationAngle;
            } 
            else if (movingRight) {
                targetRotationAngle = -maxRotationAngle;
            }
        } else {
            targetRotationAngle = 0;
        }

    }

    void UpdateRotation()
    {
        var rot = transform.rotation.eulerAngles.z;
        if (rot >= 180) {
            rot = rot - 360;
        }
        var rotDiff = Math.Abs(targetRotationAngle - rot);
        if (rotDiff > 0.1)
        {
            float dir = targetRotationAngle > rot? 1 : -1;
            var delta = rotationSpeed * Time.deltaTime;
            if (delta > rotDiff) 
            {
                delta = rotDiff;
            }
            transform.rotation = Quaternion.Euler(0, 0, rot + dir * delta);
        }
    }

    // Move ship in direction
    void Move(Vector3 dir) 
    {
        transform.position += dir;
    }

    // Keep ship in game field
    void LimitPositionToBounds()
    {
        var pos = transform.position;
        if (pos.x < game.boundsMin.x)
        {
            pos.x = game.boundsMin.x;
        }
        else if(pos.x > game.boundsMax.x)
        {
            pos.x = game.boundsMax.x;
        }

        if (pos.z < game.boundsMin.z)
        {
            pos.z = game.boundsMin.z;
        }
        else if (pos.z > game.boundsMax.z)
        {
            pos.z = game.boundsMax.z;
        }

        if (pos != transform.position)
        {
            transform.position = pos;
        }
    }
}
