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

    // Shield animator reference
    Animator shieldAnim;
    // Explosion animator reference
    Animator explosionAnim;

    // Hull integrity
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
        game = GameObject.Find("Game").GetComponent<Game>();
        shieldAnim = transform.Find("Shield").GetComponent<Animator>();
        explosionAnim = transform.Find("Explosion").GetComponent<Animator>();
        integrity = maxIntegrity;
        timeSpawn = game.GameTime();
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
        if (game.IsPaused())
        {
            return;
        }

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
        var dt = game.GameTime() - timeSpawn;
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
            integrity -= proj.damage;
            Destroy(other.gameObject, 0.01f);
            if (integrity > 0)
            {
                shieldAnim.Play("ShieldPulse");
            }
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
            explosionAnim.Play("Explode");
        } 
        else
        {
            Destroy(gameObject, 0.01f);
        }
    }

}
