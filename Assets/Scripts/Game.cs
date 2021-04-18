using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [Tooltip("Left bottom bound of game field")]
    [SerializeField] public Vector3 boundsMin = new Vector3(-15, 0, 0);

    [Tooltip("Right top bound of game field")]
    [SerializeField] public Vector3 boundsMax = new Vector3(15, 0, 20);

    [Tooltip("Z coodrinate at which enemies spawn")]
    [SerializeField] public float spawnZ = 30;

    [Tooltip("Z coodrinate at which enemies despawn")]
    [SerializeField] public float despawnZ = -10;

    [SerializeField] public float spawnInterval = 5;

    [SerializeField] public List<Enemy> enemyPrefabs;

    // Game instance
    public static Game instance;

    // Direction vectors
    static public Vector3 dirLeft = new Vector3(-1, 0, 0);
    static public Vector3 dirRight = new Vector3(1, 0, 0);
    static public Vector3 dirForward = new Vector3(0, 0, 1);
    static public Vector3 dirBack = new Vector3(0, 0, -1);

    float timeToSpawn;
    int enemyIndex;

    // Start is called before the first frame update
    void Awake() {
        instance = this;
    }

    // Returns true if pos is in bounds of game field, otherwise false
    public bool InBounds(Vector3 pos)
    {
        return (pos.x >= boundsMin.x &&
                pos.x <= boundsMax.x &&
                pos.z >= boundsMin.z &&
                pos.z <= boundsMax.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (timeToSpawn <= 0)
        {
            Spawn();
            timeToSpawn = spawnInterval;
        }

        timeToSpawn -= Time.deltaTime;
    }

    void Spawn()
    {
        if (enemyPrefabs == null)
        {
            Debug.Log("Game: enemy prefabs is null");
            return;
        }

        var enemy = Instantiate<Enemy>(enemyPrefabs[enemyIndex], new Vector3(0, 0, spawnZ), Quaternion.identity);

        enemyIndex++;
        if (enemyIndex >= enemyPrefabs.Count)
        {
            enemyIndex = 0;
        }
    }
}
