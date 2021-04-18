using System;

abstract public class Trajectory
{
    abstract public float Value(float time);
}

public class TrajectoryStatic: Trajectory
{
    // Position of a trajectory
    float pos;

    public TrajectoryStatic(float p)
    {
        pos = p;
    }

    override public float Value(float time){
        return pos;
    }
}

public class TrajectoryLine: Trajectory
{
    // Linear speed of a trajectory
    float speed;

    public TrajectoryLine(float s)
    {
        speed = s;
    }

    override public float Value(float time){
        return time * speed;
    }
}

public class TrajectorySine: Trajectory
{
    // Offset of the sine
    float offset;
    // Amplitude of the sine
    float amplitude;
    // Starting phase of the sine (degrees)
    float phase;
    // Speed of the sine (argument multiplier)
    float speed;

    public TrajectorySine(float offset = 0f, float amp = 10f, float sp = 1f, float ph = 0f)
    {
        amplitude = amp;
        speed = sp;
        phase = ph;
    }

    override public float Value(float time){
        return offset + (float)(amplitude * Math.Sin(time * Math.PI * 2 * speed + phase * Math.PI / 180f));
    }
}

