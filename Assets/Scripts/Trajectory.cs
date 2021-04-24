using UnityEngine;
using System;

// Trajectory defines position in space after some time.
// Initial position is zero vector. Add it to initial transform.position
// to calculate actual position.
public class Trajectory : MonoBehaviour
{
    public enum Type
    {
        Static, // Always 0;
        Straight, // Moves in straigth line;
        Sine // Moves in sine;
    }

    [Tooltip("Type of trajectory on X axis")]
    [SerializeField] public Type typeX = Type.Static;

    [Tooltip("X amplitude (Sine only)")]
    [SerializeField] public float amplitudeX;

    [Tooltip("X starting phase, degrees (Sine only)")]
    [SerializeField] public float phaseX;

    [Tooltip("X speed")]
    [SerializeField] public float speedX;

    [Tooltip("Type of trajectory on Z axis")]
    [SerializeField] public Type typeZ = Type.Straight;

    [Tooltip("Z amplitude (Sine only)")]
    [SerializeField] public float amplitudeZ;

    [Tooltip("Z starting phase, degrees (Sine only)")]
    [SerializeField] public float phaseZ;

    [Tooltip("Z speed")]
    [SerializeField] public float speedZ;

    // Returns position in trajectory after "time" seconds passed
    public Vector3 Position(float time)
    {
        return new Vector3(
            Value(time, typeX, amplitudeX, phaseX, speedX),
            0,
            Value(time, typeZ, amplitudeZ, phaseZ, speedZ));
    }

    private float Value(float time, Type t, float amp, float ph, float spd)
    {
        switch (t)
        {
        case Type.Sine:
            return (float)(amp * Math.Sin(time * Math.PI * 2 * spd + ph * Math.PI / 180f));
        case Type.Straight:
            return spd * time;
        default:
        case Type.Static:
            return 0;
        }
    }
}

