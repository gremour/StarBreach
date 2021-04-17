using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Tooltip("Direction in which projectile is travelling")]
    [SerializeField] Vector3 dir = new Vector3(0, 0, 1);

    [Tooltip("Velocity of projectile in world coord units per second")]
    [SerializeField] float velocity = 20;

    [Tooltip("Time to live in seconds")]
    [SerializeField] float ttl = 2;

    float aliveTime;

    // Update is called once per frame
    void Update()
    {
        transform.position += dir * velocity * Time.deltaTime;
        aliveTime += Time.deltaTime;
        if (aliveTime > ttl)
        {
            Destroy(gameObject, 0.1f);
        }
    }
}
