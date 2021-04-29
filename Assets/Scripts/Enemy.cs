using UnityEngine;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    [Tooltip("Hull integrity points")]
    public int maxIntegrity = 15;

    [Tooltip("Score value")]
    public int score = 10;

    [Tooltip("Blaster projectile prefab")]
    public Projectile projectilePrefab;

    [Tooltip("Delay between subsequent blaster shots")]
    public float blasterDelay = 2f;

    [Tooltip("Movement trajectories")]
    public List<Trajectory> trajectories;

    [Tooltip("Trajectory generators (if Trajectories are not set)")]
    public List<TrajectoryRandomizer> trajectoryGens;

    [Tooltip("Wave: enemy (or swarm) strength cost in pack")]
    public float cost = 10;

    [Tooltip("Wave: number of wave in which enemy starts to spawn")]
    public int dormant = 0;

    [Tooltip("Wave: if positive, spawns several of this enemies in a swarm")]
    public int swarmCount;

    [Tooltip("Wave: delay of spawns inside swarm")]
    public float swarmDelay = 1f;

    // Game reference
    Game game;

    // Shield animator reference
    Animator shieldAnim;
    // Explosion animator reference
    Animator explosionAnim;

    // Hull integrity
    int integrity;

    // Remaining cooldown on blaster
    float blasterCooldown;

    bool dead;

    // Current trajectory index
    int trajIndex = -1;

    void Awake()
    {
        game = GameObject.Find("Game").GetComponent<Game>();
        shieldAnim = transform.Find("Shield").GetComponent<Animator>();
        explosionAnim = transform.Find("Explosion").GetComponent<Animator>();
        integrity = maxIntegrity;
        var hb = GetComponentInChildren<Healthbar>();
        if (hb != null)
        {
            hb.maximumValue = maxIntegrity;
            hb.SetValue(maxIntegrity);
            hb.SetVisible(false);
        }
        blasterCooldown = blasterDelay;
        GenerateTrajectories();
    }

    // Update is called once per frame
    void Update()
    {
        if (dead)
        {
            return;
        }

        if (trajIndex < 0 || trajectories[trajIndex].IsExpired())
        {
            ChangeTrajectory();
        }

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
        Shoot();
    }

    public void GenerateTrajectories()
    {
        if ((trajectories != null && trajectories.Count > 0) || trajectoryGens == null)
        {
            return;
        }
        if (trajectories == null)
        {
            trajectories = new List<Trajectory>();
        }
        foreach(TrajectoryRandomizer tg in trajectoryGens)
        {
            var traj = tg.NewTrajectory(gameObject);
            traj.timeController = new TimeControllerGame(game);
            trajectories.Add(traj);
        }
    }

    public void ReseedTrajectoryGenerator(int seed)
    {
        Random.InitState(seed);
        foreach (TrajectoryRandomizer tg in trajectoryGens)
        {
            tg.seed = (int)(Random.value * 0x7fffffffffffffff);
        }
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
        if (dead)
        {
            return;
        }

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
        if (trajectories != null && trajIndex >= 0)
        {
            // Stop trajectory
            trajectories[trajIndex].Reset();
        }
        dead = true;
        
        if (explode)
        {
            Destroy(gameObject, 1f);
            explosionAnim.Play("Explode");
            game.AddScore(score);
        } 
        else
        {
            Destroy(gameObject, 0.01f);
        }
    }

    void ChangeTrajectory()
    {
        //Debug.Log("ChangeTrajectory index=" + trajIndex + ", trajs=" + trajectories.Count);
        if (trajIndex >= trajectories.Count - 1)
        {
            return;
        }
        trajIndex++;
        if (trajIndex > 0)
        {
            trajectories[trajIndex-1].Reset();
        }
        trajectories[trajIndex].targetTransform = transform;
    }
}
