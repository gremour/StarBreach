using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Tooltip("Hull integrity points")]
    [SerializeField] int maxIntegrity = 15;

    [Tooltip("Score value")]
    [SerializeField] int score = 10;

    [Tooltip("Blaster projectile prefab")]
    [SerializeField] Projectile projectilePrefab;

    [Tooltip("Delay between subsequent blaster shots")]
    [SerializeField] float blasterDelay = 2f;

    [SerializeField] Trajectory trajectoryPrefab;

    // Game reference
    Game game;

    private int integrity;

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
        integrity = maxIntegrity;
        timeSpawn = Time.time;
        initialPos = transform.position;
        initialRot = transform.rotation;
        var hb = GetComponentInChildren<Healthbar>();
        if (hb != null)
        {
            hb.maximumValue = maxIntegrity;
            hb.SetValue(maxIntegrity);
            hb.SetVisible(false);
        }
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
        if (trajectoryPrefab == null)
        {
            Debug.Log("Enemy trajectory prefab is null");
            return;
        }
        // Make dead enemy stop
        if (dead) {
            return;
        }
        var dt = Time.time - timeSpawn;
        var traj = trajectoryPrefab.Position(dt);
        transform.position = initialPos + traj;
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
            integrity -= proj.damage;
            Destroy(other.gameObject, 0.01f);
        }
        else if (playerCollision)
        {
            integrity = 0;
        }

        if (integrity <= 0)
        {
            Terminate(true);
        }

        var hb = GetComponentInChildren<Healthbar>();
        if (hb != null)
        {
            hb.SetValue(integrity);
            hb.SetVisible(integrity > 0 && integrity < maxIntegrity);
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
