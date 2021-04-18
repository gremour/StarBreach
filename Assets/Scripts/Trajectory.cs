using UnityEngine;
using System;

// Trajectory defines position in space after some time.
// Initial position is zero vector. Add it to initial transform.position
// to calculate actual position.
public class Trajectory : MonoBehaviour
{
    enum Type
    {
        Static, // Always 0;
        Straight, // Moves in straigth line;
        Sine // Moves in sine;
    }

    [SerializeField] Type typeX = Type.Static;
    [SerializeField] Type typeZ = Type.Straight;

    [Tooltip("X amplitude (Sine only)")]
    [SerializeField] float amplitudeX;

    [Tooltip("Z amplitude (Sine only)")]
    [SerializeField] float amplitudeZ;

    [Tooltip("X starting phase (Sine only)")]
    [SerializeField] float phaseX;

    [Tooltip("Z starting phase (Sine only)")]
    [SerializeField] float phaseZ;

    [Tooltip("X speed")]
    [SerializeField] float speedX;

    [Tooltip("Z speed")]
    [SerializeField] float speedZ;

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

