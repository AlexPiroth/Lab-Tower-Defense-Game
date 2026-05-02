using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static bool continuing = false;

    // Grid Dimensions
    public const int HEIGHT = 10, WIDTH = 18;

    // Stats
    int level;
    public int playerHP; // Might change to float after some playtesting depending on how damage works
    public int memory;
    
    // Field Data
    public GameObject spawnPoint;
    public GameObject[] livingEnemies;
    public GameObject[,] grid;
    public int[] spawnCoords;
    public GameObject cameraObj;

    // Current Play Data
    public GameObject selectedTower;

    public GameObject testTower, testEnemy;

    // Wave Loading Checks
    int waveLength;
    bool waveDone = false;
    private readonly WaitForSeconds cooldown = new(3);
    private readonly WaitForSeconds warmup = new(5);

    // Prefabs
    [SerializeField] GameObject[] allEnemyTypes;
    [SerializeField] GameObject gridPrefab, spawnPointPrefab;

    cluster[] clusters;
    bool startGame = false;

    struct cluster
    {
        public GameObject type;
        public int count;
    }

    IEnumerator SpawnTestEnemy()
    {
        while (true)
        {
            Instantiate(testEnemy, spawnPoint.transform.position, Quaternion.identity);
            yield return cooldown;
        }
    }

    private void Update()
    {
        if (startGame)
        {
            if (livingEnemies.Length == 0 && waveDone)
            {
                level++;
                waveDone = false;
                SpawnNextWave();
            }
        }
    }

    private void Awake()
    {
        grid = new GameObject[HEIGHT, WIDTH];
        MakeEmptyGrid();
        if (continuing)
            LoadSave();
        else
            MakeInitialGrid();

        // Wait 5 seconds
        //StartCoroutine(nameof(WarmUp));

        // Start the game
        //startGame = true;
        StartCoroutine(SpawnTestEnemy());
    }

    private void LoadSave()
    {
        // Load the saved grid, figure this out later
    }

    private void MakeEmptyGrid()
    {
        // Make grid parent object
        GameObject gridParent = new GameObject();
        gridParent.name = "grid";

        // Calculate grid dimensions
        Renderer renderer = gridPrefab.GetComponent<Renderer>();
        Camera cameraComp = cameraObj.GetComponent<Camera>();
        Vector2 topLeft = cameraComp.ViewportToWorldPoint(new Vector3(0, 1));
        float size = renderer.bounds.size.x;
        topLeft.x += size / 2;
        topLeft.y -= size / 2;
        float xCoord = topLeft.x, yCoord = topLeft.y;

        // Make grid
        for (int i = 0; i < HEIGHT; i++)
        {
            for (int j = 0; j < WIDTH; j++)
            {
                GameObject newCell = Instantiate(gridPrefab, new Vector2(xCoord, yCoord), Quaternion.identity);
                grid[i,j] = newCell;
                newCell.transform.SetParent(gridParent.transform);
                xCoord += size;
            }
            yCoord -= size;
            xCoord = topLeft.x;
        }
    }

    private void MakeInitialGrid()
    {
        spawnCoords = new int[2];
        GridSpace gridSpace;
        for (int i = 0; i < 7; i++)
        {
            gridSpace = grid[4, 13 - i].GetComponent<GridSpace>();
            if (i == 0)
                gridSpace.SpawnTower(testTower); // Will be the "castle"
            else
                ExtendPath(2);
        }
        PathScript.previousPathNode = GameObject.Find("TestTower(Clone)"); // EXTREMELY TEMPORARY
    }

    public void SetSelectedTower(GameObject towerPrefab)
    {
        selectedTower = towerPrefab;
    }

    public void ExtendPath(int direction)
    {
        if (spawnPoint == null)
        {
            spawnPoint = grid[4, 12];
            spawnCoords[0] = 4;
            spawnCoords[1] = 12;
            GridSpace startSpawn = spawnPoint.GetComponent<GridSpace>();
            startSpawn.SetPath(direction);
            return;
        }

        // Get new spawn
        GridSpace newSpawn;
        switch (direction)
        {
            // UDLR
            case 0:
                newSpawn = grid[spawnCoords[0] - 1, spawnCoords[1]].GetComponent<GridSpace>();
                if (newSpawn == null || !newSpawn.CheckSpawnLegality())
                    return;
                spawnCoords[0]--;
                break;

            case 1:
                newSpawn = grid[spawnCoords[0] + 1, spawnCoords[1]].GetComponent<GridSpace>();
                if (newSpawn == null || !newSpawn.CheckSpawnLegality())
                    return;
                spawnCoords[0]++;
                break;

            case 2:
                newSpawn = grid[spawnCoords[0], spawnCoords[1] - 1].GetComponent<GridSpace>();
                if (newSpawn == null || !newSpawn.CheckSpawnLegality())
                    return;
                spawnCoords[1]--;
                break;
            
            case 3:
                newSpawn = grid[spawnCoords[0], spawnCoords[1] + 1].GetComponent<GridSpace>();
                if (newSpawn == null || !newSpawn.CheckSpawnLegality())
                    return;
                spawnCoords[1]++;
                break;

            default:
                newSpawn = null;
                break;
        }

        // Change current spawn to a path
        GridSpace spawn = spawnPoint.GetComponent<GridSpace>();
        spawn.path.Extend(direction);

        // Set new spawn point
        spawnPoint = newSpawn.gameObject;
        newSpawn.SetPath(direction);
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
            Debug.Log("blork");
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

    IEnumerator WarmUp()
    {
        yield return warmup;
    }

}
