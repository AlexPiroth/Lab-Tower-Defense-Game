using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static bool continuing = false;

    // Grid Dimensions
    public const int HEIGHT = 9, WIDTH = 15;
    public int TOP, LEFT;

    // Stats
    int level;
    public int playerHP; // Might change to float after some playtesting depending on how damage works
    public int memory;
    
    // Field Data
    public GameObject spawnPoint;
    public GameObject[] livingEnemies;
    public GameObject[,] grid;
    public GameObject cameraObj;

    // Wave Loading Checks
    int waveLength;
    bool waveDone = false;
    private readonly WaitForSeconds cooldown = new(1);

    // Prefabs
    [SerializeField] GameObject[] allEnemyTypes;
    [SerializeField] GameObject gridPrefab;

    cluster[] clusters;

    struct cluster
    {
        public GameObject type;
        public int count;
    }

    private void Update()
    {
        if (livingEnemies.Length == 0 && waveDone)
        {
            level++;
            waveDone = false;
            SpawnNextWave();
        }
    }

    private void Awake()
    {
        MakeEmptyGrid();
        grid = new GameObject[25,25];
        if (continuing)
            LoadSave();
    }

    private void LoadSave()
    {
        // Load the saved grid
    }
    
    private void MakeEmptyGrid()
    {
        Renderer renderer = gridPrefab.GetComponent<Renderer>();
        Camera cameraComp = cameraObj.GetComponent<Camera>();
        Vector2 topLeft = cameraComp.ViewportToWorldPoint(new Vector3(0, 1));
        float size = renderer.bounds.size.x;
        float xCoord = topLeft.x, yCoord = topLeft.y;
        for (int i = 0; i < HEIGHT; i++)
        {
            for (int j = 0; j < WIDTH; j++)
            {
                Instantiate(gridPrefab, new Vector2(xCoord, yCoord), Quaternion.identity);
                xCoord += size;
            }
            yCoord -= size;
            xCoord = topLeft.x;
        }
    }

    private void SpawnNextWave()
    {
        if (level > 100) // Random wave generation above wave 100
        {
            waveLength = level / 2; // Maybe change after balancing
            for (int i = 0; i < waveLength; i++)
            {
                StartCoroutine(nameof(SpawnRandomCluster));
            }
        }
        else // Manual wave generation, read from clusters
        {
            for (int i = 0; i < waveLength; i++)
            {
                StartCoroutine(nameof(SpawnManualCluster));
            }
        }
        
        waveDone = true;
    }

    IEnumerator SpawnRandomCluster()
    {
        int enemyType = UnityEngine.Random.Range(1, 11); // Assuming 10 enemy types, pick one to spawn

        for (int j = 0; j < UnityEngine.Random.Range(1, 7); j++) // Maximum of 6 enemies in a spawn cluster, maybe amend after playtesting
            Instantiate(allEnemyTypes[enemyType], spawnPoint.transform.position, Quaternion.identity); // Spawn an enemy at spawnPoint

            yield return cooldown; // Potentially change cooldown time after playtesting
    }

    IEnumerator SpawnManualCluster()
    {
        for (int j = 0; j < 8888888; j++)
            Instantiate(clusters[j].type, spawnPoint.transform.position, Quaternion.identity); // Spawn an enemy at spawnPoint

            yield return cooldown;
    }

}
