using UnityEngine;
using System;

public class Trajectory : MonoBehaviour
{
    [Tooltip("Original point of trajectory (skip if targetTransform is used")]
    public Vector3 origin;

    [Tooltip("Speed per second on each axis")]
    public Vector3 speed;

    [Tooltip("Speed increase per second on each axis")]
    public Vector3 accel;

    [Tooltip("Amplitude of sine to add to position")]
    public Vector3 sineAmplitude;

    [Tooltip("Speed of sine phase change (1 equals to 360 degrees per second)")]
    public Vector3 sineSpeed;

    [Tooltip("Phase of sine to add to position, degrees")]
    public Vector3 sinePhase;

    [Tooltip("Expiration time in seconds (for external users of class)")]
    public float expire;

    [Tooltip("Transform object to control (keep None to use this GameObject transform)")]
    public Transform targetTransform;

    [HideInInspector]
    public TimeController timeController;

    // Time elapsed since trajectory start
    float elapsed;

    Vector3 posIncrease;
    Vector3 speedIncrease;

    void Awake()
    {
        if (timeController == null)
        {
            timeController = new TimeControllerUnity();
        }
        if (targetTransform == null && transform != null)
        {
            targetTransform = transform;
            origin = transform.position;
        }
    }

    void Update()
    {
        // Freeze trajectory that has no targetTransform assigned
        if (targetTransform == null)
        {
            return;
        }
        var deltaTime = timeController.DeltaTime();
        elapsed += deltaTime;
        posIncrease.x += speedIncrease.x + speed.x * deltaTime;
        posIncrease.y += speedIncrease.y + speed.y * deltaTime;
        posIncrease.z += speedIncrease.z + speed.z * deltaTime;
        speedIncrease.x += accel.x * deltaTime * deltaTime;
        speedIncrease.y += accel.y * deltaTime * deltaTime;
        speedIncrease.z += accel.z * deltaTime * deltaTime;
        var sine = Vector3.zero;
        if (sineAmplitude.x != 0f && sineSpeed.x != 0f)
        {
            sine.x = sineAmplitude.x * (float) Math.Sin(Math.PI * 2 * elapsed * sineSpeed.x + sinePhase.x * Math.PI / 180f);
        }
        if (sineAmplitude.y != 0f && sineSpeed.y != 0f)
        {
            sine.y = sineAmplitude.y * (float) Math.Sin(Math.PI * 2 * elapsed * sineSpeed.y + sinePhase.y * Math.PI / 180f);
        }
        if (sineAmplitude.z != 0f && sineSpeed.z != 0f)
        {
            sine.z = sineAmplitude.z * (float) Math.Sin(Math.PI * 2 * elapsed * sineSpeed.z + sinePhase.z * Math.PI / 180f);
        }
        var pos = targetTransform.position;
        pos.x = origin.x + posIncrease.x + sine.x;
        pos.y = origin.y + posIncrease.y + sine.y;
        pos.z = origin.z + posIncrease.z + sine.z;
        targetTransform.position = pos;
    }

    public void Reset()
    {
        elapsed = 0;
        posIncrease = Vector3.zero;
        speedIncrease = Vector3.zero;
        targetTransform = null;
    }

    public bool IsExpired()
    {
        return expire != 0f && elapsed >= expire;
    }
}

public abstract class TimeController{
    public abstract float DeltaTime();
}

public class TimeControllerUnity: TimeController{
    public override float DeltaTime()
    {
        return Time.deltaTime;
    }
}
