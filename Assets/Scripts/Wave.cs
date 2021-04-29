using System.Collections.Generic;
using System.Collections;
using UnityEngine;

// Wave object spawns enemies in waves.
// Each wave has a number. 
// Call Next to start next wave.
// Call IsOver to find out if wave is over.
// Inside one wave enemies spawn in packs.
// Strength of pack depends on strength accumulated
// since last spawn.
// Strength regenerates over time based on strengthRegen.
// Spawns happen in ceration moments of time controlled
// by spawnDelay.
public class Wave : MonoBehaviour {

    private class CurrentEnemy
    {
        public Enemy enemy;
        public int weight;
    }

    [Tooltip("List of enemies")]
    public List<Enemy> enemies;

    [Tooltip("Number of packs per wave")]
    public int packs = 10;

    [Tooltip("Wave strength regeneration per second")]
    public float strengthRegen = 10f;

    [Tooltip("Wave strength regeneration increase per wave #, percent")]
    public float strengthAccel = 5f;

    [Tooltip("Delay between spawns in seconds")]
    public float spawnDelay = 5f;

    [Tooltip("Delay between waves in seconds")]
    public float waveDelay = 10f;

    [Tooltip("Maximum relative weight of enemy type in pack. Bigger numbers cause skew to single enemy type more often.")]
    public int maxSpawnWeight = 6;

    [Tooltip("Minimum increase of spawn weight per pack")]
    public int minSpawnWeightIncrease = 1;

    [Tooltip("Maximum increase of spawn weight per pack")]
    public int maxSpawnWeightIncrease = 3;

    [Tooltip("Number of enemies have their weight increased per pack")]
    public int weightIncreases = 2;

    // Wave number
    public int number;

    Game game;

    float strength;
    float spawnCooldown;
    int packsLeft;

    // Enemy list for current wave;
    List<CurrentEnemy> enemyList;

    void Awake()
    {
        game = GameObject.Find("Game").GetComponent<Game>();
        ResetEnemyList();
    }

    void Update()
    {
        if (game.IsPaused())
        {
            return;
        }
        strength += Time.deltaTime * strengthRegen * strengthAccel * number / 100f;

        spawnCooldown -= Time.deltaTime;
        if (spawnCooldown <= 0 && packsLeft > 0)
        {
            SpawnPack();
        }
    }

    public void Reset() {
        spawnCooldown = 0;
        packsLeft = 0;
        number = 0;
        strength = 0;
    }

    void DelaySpawn(float seconds)
    {
        spawnCooldown = seconds;
    }

    // Initialize enemy list with random weights.
    void ResetEnemyList()
    {
        enemyList = new List<CurrentEnemy>();
        var rnd = new System.Random();
        string weights = "";
        foreach (Enemy we in enemies)
        {
            if (we.dormant > number)
            {
                continue;
            }
            var ce = new CurrentEnemy();
            ce.enemy = we;
            ce.weight = rnd.Next(1, maxSpawnWeight+1);
            enemyList.Add(ce);
            weights += ce.weight + " ";
        }
        //Debug.Log("New enemy list weights: " + weights);
    }

    public bool IsOver()
    {
        return packsLeft < 1 && spawnCooldown <= 0;
    }

    public void Next()
    {
        number++;
        packsLeft = packs;
        ResetEnemyList();
        DelaySpawn(waveDelay);
    }

    // Add random weights to enemies in list.
    void UpdateEnemyList()
    {
        var rnd = new System.Random();
        List<int> updateIndices = new List<int>();
        int i = 0;
        for (; i < weightIncreases; i++)
        {
            var ind = rnd.Next(0, updateIndices.Count);
            if (updateIndices.Contains(ind))
            {
                i--;
                continue;
            }
            updateIndices.Add(ind);
        }

        string weights = "";
        i = 0;
        foreach (CurrentEnemy ce in enemyList)
        {
            if (!updateIndices.Contains(i))
            {
                continue;
            }
            ce.weight += new System.Random().Next(minSpawnWeightIncrease, maxSpawnWeightIncrease+1);
            weights += ce.weight + " ";
        }
        //Debug.Log("Updated enemy list weights: " + weights);
    }

    // Picks enemy from list; pos is position of 0 to 1
    // in summary list weight.
    Enemy PickEnemy(float pos)
    {
        if (enemyList.Count == 0)
        {
            return null;
        }
        int sumWeight = 0;
        foreach (CurrentEnemy ce in enemyList)
        {
            sumWeight += ce.weight;
        }
        int posWeight = (int)(pos * sumWeight);
        // int enemyInd = 0;
        foreach (CurrentEnemy ce in enemyList)
        {
            posWeight -= ce.weight;
            if (posWeight <= 0)
            {
                // Debug.Log("PickEnemy pos=" + pos + " enemy chosen=" + (enemyInd+1) + " of " + enemyList.Count);
                return ce.enemy;
            }
            // enemyInd++;
        }
        // Debug.Log("PickEnemy pos=" + pos + " enemy chosen=last");
        return enemyList[enemyList.Count-1].enemy;
    }

    // SpawnPack spawns a pack of enemies.
    void SpawnPack()
    {
        spawnCooldown = spawnDelay;
        packsLeft--;
        int enemyCount = 0;
        float strengthSpent = 0;
        while (true) {
            if (strength <= 0)
            {
                strength = 0;
                break;
            }
            var pos = UnityEngine.Random.Range(0f, 1f);
            var we = PickEnemy(pos);
            strength -= we.cost;
            strengthSpent += we.cost;
            enemyCount++;

            Random.InitState((int)System.DateTime.Now.Ticks);
            var seed = (int)(Random.value * 0x7fffffffffffffff);
            we.ReseedTrajectoryGenerator(seed);
            var e = Instantiate(we);

            if (we.swarmCount > 1)
            {
                for (int i = 1; i < we.swarmCount; i++)
                {
                    SpawnInfo si;
                    si.enemyPrefab = we;
                    si.delay = we.swarmDelay * i;
                    si.seed = seed;
                    StartCoroutine("SpawnEnemyDelayed", si);
                }
            }
        }
        //Debug.Log("Spawned " + enemyCount + " enemies for " + strengthSpent + " strength cost; packs left " + packsLeft);
    }

    private struct SpawnInfo
    {
        public Enemy enemyPrefab;
        public float delay;
        public int seed;
    }

    IEnumerator SpawnEnemyDelayed(SpawnInfo si)
    {
        yield return new WaitForSeconds(si.delay);
        si.enemyPrefab.ReseedTrajectoryGenerator(si.seed);
        var e = Instantiate(si.enemyPrefab);
    }
}
