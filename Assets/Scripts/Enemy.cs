using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Tooltip("Hull integrity points")]
    [SerializeField] int intergrity = 15;

    [Tooltip("Score value")]
    [SerializeField] int score = 10;

    [Tooltip("Blaster projectile prefab")]
    [SerializeField] Projectile projectilePrefab;

    [Tooltip("Delay between subsequent blaster shots")]
    [SerializeField] float blasterDelay = 2f;

    [SerializeField] Trajectory trajectoryX = new TrajectorySine(-5f, 10f, 0.3f);

    [SerializeField] Trajectory trajectoryZ = new TrajectoryLine(-3f);

    // Game reference
    Game game;

    // Spawn time point;
    float timeSpawn;

    // Remaining cooldown on blaster
    float blasterCooldown;

    Vector3 initialPos;
    Quaternion initialRot;

    bool dead;

    // Start is called before the first frame update
    void Start()
    {
        game = Game.instance;
        timeSpawn = Time.time;
        initialPos = transform.position;
        initialRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.z < game.despawnZ)
        {
            Terminate();
            return;
        }
        ProcessCooldowns();
        Move();
        Shoot();
    }

    void Move()
    {
        // Make dead enemy stop
        if (dead) {
            return;
        }
        var dpos = transform.position;
        dpos.x = trajectoryX == null? (game.boundsMax.x + game.boundsMin.x) / 2 : trajectoryX.Value(Time.time - timeSpawn);
        dpos.z = trajectoryZ == null? game.boundsMax.z : trajectoryZ.Value(Time.time - timeSpawn);
        transform.position = initialPos + dpos;
    }

    void Shoot()
    {
        if (blasterCooldown > 0f)
        {
            return;
        }
        blasterCooldown = blasterDelay;

        if (projectilePrefab == null)
        {
            Debug.Log("Projectile prefab is uninitialized");
            return;
        }
        Instantiate<Projectile>(
            projectilePrefab,
            transform.position + projectilePrefab.dir * 2,
            Quaternion.identity);
    }

    void ProcessCooldowns()
    {
        blasterCooldown -= Time.deltaTime;
        if (blasterCooldown < 0 ) {
            blasterCooldown = 0;
        }
    }

    void OnCollisionEnter(Collision other) {
        var playerCollision = other.gameObject.tag == "Player";
        var playerProjectileCollision = other.gameObject.tag == "PlayerProjectile";
        if (playerProjectileCollision)
        {
            var proj = other.gameObject.GetComponent<Projectile>();
            if (proj == null)
            {
                Debug.Log("No projectile component found in player projectile collision");
                return;
            }
            intergrity -= proj.damage;
            Destroy(other.gameObject, 0.01f);
        }
        else if (playerCollision)
        {
            intergrity = 0;
        }

        if (intergrity <= 0)
        {
            Terminate(true);
        }
    }

    void Terminate(bool explode = false)
    {
        dead = true;
        if (explode)
        {
            Destroy(gameObject, 1f);
            GetComponentInChildren<Animator>().enabled = true;
            GetComponentInChildren<SpriteRenderer>().enabled = true;
        } 
        else
        {
            Destroy(gameObject, 0.01f);
        }
    }

}