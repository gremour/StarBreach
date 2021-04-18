using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Tooltip("Projectile damage")]
    [SerializeField] public int damage = 10;

    [Tooltip("Direction in which projectile is travelling")]
    [SerializeField] public Vector3 dir = new Vector3(0, 0, -1);

    [Tooltip("Velocity of projectile in world coord units per second")]
    [SerializeField] public float velocity = 5;

    [Tooltip("Time to live in seconds")]
    [SerializeField] public float ttl = 2;

    [Tooltip("Amount of chaotic rotation")]
    [SerializeField] public float chaoticRotation = 0f;

    public bool isPlayerProjectile;

    float aliveTime;

    // Update is called once per frame
    void Update()
    {
        if (chaoticRotation != 0f) {
            var axis = Random.Range(0, 2);
            Vector3 drot;
            switch (axis)
            {
            case 0:
                drot = new Vector3(1, 0, 0);
                break;
            case 1:
                drot = new Vector3(0, 1, 0);
                break;
            default:
                drot = new Vector3(0, 0, 1);
                break;
            }
            var delta = Time.deltaTime * chaoticRotation;
            var rot = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(rot + drot * delta);
        }

        transform.position += dir * velocity * Time.deltaTime;
        aliveTime += Time.deltaTime;
        if (aliveTime > ttl)
        {
            Destroy(gameObject, 0.01f);
        }
    }
}
