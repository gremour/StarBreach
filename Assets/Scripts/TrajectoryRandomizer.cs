using UnityEngine;

public class TrajectoryRandomizer : MonoBehaviour
{
    [Tooltip("Minimum position")]
    public Vector3 minPosition;

    [Tooltip("Maximum position")]
    public Vector3 maxPosition;

    [Tooltip("Minimum speed per second on each axis")]
    public Vector3 minSpeed;

    [Tooltip("Maximum speed per second on each axis")]
    public Vector3 maxSpeed;

    [Tooltip("Minimum speed increase per second on each axis")]
    public Vector3 minAccel;

    [Tooltip("Maximum speed increase per second on each axis")]
    public Vector3 maxAccel;

    [Tooltip("Minimum amplitude of sine to add to position")]
    public Vector3 minSineAmplitude;

    [Tooltip("Maximum amplitude of sine to add to position")]
    public Vector3 maxSineAmplitude;

    [Tooltip("Minimum speed of sine phase change (1 equals to 360 degrees per second)")]
    public Vector3 minSineSpeed;

    [Tooltip("Maximum speed of sine phase change (1 equals to 360 degrees per second)")]
    public Vector3 maxSineSpeed;

    [Tooltip("Minimum phase of sine to add to position, degrees")]
    public Vector3 minSinePhase;

    [Tooltip("Minimum phase of sine to add to position, degrees")]
    public Vector3 maxSinePhase;

    [Tooltip("Minumum expiration time in seconds (for external users of class)")]
    public float minExpire;

    [Tooltip("Maximum expiration time in seconds (for external users of class)")]
    public float maxExpire;

    [Tooltip("Random seed for this trajectory (0 is to use time)")]
    public int seed;

    public Trajectory NewTrajectory(GameObject obj)
    {
        var newTraj = obj.AddComponent<Trajectory>();
        Random.InitState(seed == 0? (int)System.DateTime.Now.Ticks : seed);
        newTraj.origin = scatter(minPosition, maxPosition);
        newTraj.speed = scatter(minSpeed, maxSpeed);
        newTraj.accel = scatter(minAccel, maxAccel);
        newTraj.sineAmplitude = scatter(minSineAmplitude, maxSineAmplitude);
        newTraj.sineSpeed = scatter(minSineSpeed, maxSineSpeed);
        newTraj.sinePhase = scatter(minSinePhase, maxSinePhase);
        newTraj.expire = Random.Range(minExpire, maxExpire);
        return newTraj;
    }

    Vector3 scatter(Vector3 min, Vector3 max)
    {
        return new Vector3(
            Random.Range(min.x, max.x),
            Random.Range(min.y, max.y),
            Random.Range(min.z, max.z));
    }
}
