using System.Collections.Generic;
using System;
using UnityEngine;

// WaveEnemy is an Enemy wrapper with
// swarm and trajectory randomness
// parameters.
// Spawn spawns an enemy or swarm to game field.
public class WaveEnemy : MonoBehaviour
{
    [Tooltip("Enemy prefab")]
    [SerializeField] Enemy prefab;

    [Tooltip("Enemy (or swarm) strength cost in pack")]
    [SerializeField] public float cost = 10;

    [Tooltip("Number of wave in which enemy starts to spawn")]
    [SerializeField] public int dormant = 0;

    [Tooltip("If positive, spawns several of this enemies in a swarm")]
    [SerializeField] int swarmCount;

    [Tooltip("Delay of spawns inside swarm")]
    [SerializeField] float swarmDelay = 1f;

    [SerializeField] float speedXMin = 1f;
    [SerializeField] float speedXMax = 5f;

    [SerializeField] float speedZMin = 1f;
    [SerializeField] float speedZMax = 3f;

    [SerializeField] float amplitudeXMin = 5f;
    [SerializeField] float amplitudeXMax = 15f;

    [SerializeField] float amplitudeZMin = 5f;
    [SerializeField] float amplitudeZMax = 15f;

    [SerializeField] float phaseXMin = 0f;
    [SerializeField] float phaseXMax = 360f;

    [SerializeField] float phaseZMin = 0f;
    [SerializeField] float phaseZMax = 360f;

    [SerializeField] float spawnZ;
    [SerializeField] float spawnXMin;
    [SerializeField] float spawnXMax;

    // Values of current spawn (need to save for swarms).
    float currentSpeedX;
    float currentSpeedZ;
    float currentAmplitudeX;
    float currentAmplitudeZ;
    float currentPhaseX;
    float currentPhaseZ;

    Game game;

    int swarmRemaining;
    float swarmCooldown;

    void Awake()
    {
        game = GameObject.Find("Game").GetComponent<Game>();
        if (spawnZ == 0)
        {
            spawnZ = game.spawnZ;
        }
        if (spawnXMin == 0)
        {
            spawnXMin = game.boundsMin.x * 0.8f;
            spawnXMax = game.boundsMax.x * 0.8f;
        }
    }

    void Update()
    {
        if (game.IsPaused())
        {
            return;
        }

        if (swarmRemaining > 0)
        {
            swarmCooldown -= Time.deltaTime;
            if (swarmCooldown <= 0)
            {
                swarmCooldown = swarmDelay;
                swarmRemaining--;
                SpawnEnemy();
            }
        }
    }

    // Spawns enemy or swarm.
    public void Spawn()
    {
        if (swarmCount == 0)
        {
            // Spawn one enemy
            SpawnEnemy();
            return;
        }

        // Spawn swarm (actual spawning is in Update method)
        swarmRemaining = swarmCount;
        swarmCooldown = 0;
    }

    // Spawns enemy prefab
    void SpawnEnemy()
    {
        // Make sure enemies spawned closer to edges will move to the center of map.
        var x = UnityEngine.Random.Range(spawnXMin, spawnXMax);
        var c = (spawnXMax + spawnXMin) / 2;
        var d = (spawnXMax - spawnXMin) / 2;
        var left = x < (c - d/2);
        var right = x > (c + d/2);
        var center = !left && !right;
        float sign;
        if (left)
        {
            sign = 1;
        }
        else if (right)
        {
            sign = -1;
        }
        else
        {
            sign = new System.Random().Next(0, 2) == 0? -1f : 1f;
        }
        currentSpeedX = sign * UnityEngine.Random.Range(speedXMin, speedXMax);
        currentSpeedZ = -UnityEngine.Random.Range(speedZMin, speedZMax);
        currentAmplitudeX = UnityEngine.Random.Range(amplitudeXMin, amplitudeXMax);
        currentAmplitudeZ = UnityEngine.Random.Range(amplitudeZMin, amplitudeZMax);
        currentPhaseX = UnityEngine.Random.Range(phaseXMin, phaseXMax);
        currentPhaseZ = UnityEngine.Random.Range(phaseZMin, phaseZMax);

        var enemy = Instantiate<Enemy>(prefab, new Vector3(x, 0, spawnZ), Quaternion.identity);
        enemy.trajectory.speedX = currentSpeedX;
        enemy.trajectory.speedZ = currentSpeedZ;
        enemy.trajectory.amplitudeX = currentAmplitudeX;
        enemy.trajectory.amplitudeZ = currentAmplitudeZ;
        enemy.trajectory.phaseX = currentPhaseX;
        enemy.trajectory.phaseZ = currentPhaseZ;
    }
}
