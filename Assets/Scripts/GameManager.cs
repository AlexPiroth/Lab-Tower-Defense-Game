using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool continuing = false;

    // Grid Dimensions
    public const int HEIGHT = 10, WIDTH = 18;

    // Stats
    int level = 0;
    public int playerHP; // Might change to float after some playtesting depending on how damage works
    public int memory;
    
    // Field Data
    public GameObject spawnPoint;
    public GameObject[] livingEnemies;
    public GameObject[,] grid;
    public int[] spawnCoords;
    public GameObject cameraObj;
    [SerializeField] TMP_Text memoryText;

    // Current Play Data
    public GameObject selectedTower;
    public int selectedTowerCost;

    public GameObject testTower, testEnemy;

    // Wave Loading Checks
    int waveLength;
    bool waveDone = false;
    private readonly WaitForSeconds cooldown = new(1);
    private readonly WaitForSeconds longCooldown = new(3);
    private readonly WaitForSeconds warmup = new(5);

    // Prefabs
    [SerializeField] GameObject[] allEnemyTypes;
    [SerializeField] GameObject gridPrefab, spawnPointPrefab;

    [SerializeField] TextAsset levelData;

    public delegate void ResetButtons();
    public event ResetButtons Reset;

    string[] levels;

    bool startGame = false;
    public bool setup = true;

    Queue<int> pathQueue = new Queue<int>();
    Dictionary<Vector2, GameObject> towerDict = new Dictionary<Vector2, GameObject>();
    string filename = "saveFile.txt";
    public Damageable home;
    [SerializeField] GameObject pausePrompt;
    public int loadHP = -100;


    IEnumerator SpawnTestEnemy()
    {
        while (true)
        {
            if (levels[level] != null)
            {
                int[] currentLevelContents = Array.ConvertAll<string, int>(levels[level].Split('|'), int.Parse);
                for (int i = 0; i < currentLevelContents.Length; i += 2)
                {
                    for (int j = 0; j < currentLevelContents[i + 1]; j++)
                    {
                        Instantiate(allEnemyTypes[currentLevelContents[i]], spawnPoint.transform.position, Quaternion.identity);
                        yield return cooldown;
                    }
                }
            }
            else // Random generation after scripted waves end
            {
                int waves = UnityEngine.Random.Range(1, 6);
                for (int i = 0; i < waves; i++)
                {
                    int count = UnityEngine.Random.Range(1, 25);
                    int enemyNum = UnityEngine.Random.Range(0, 5);
                    for (int j = 0; j < count; j++)
                    {
                        Instantiate(allEnemyTypes[enemyNum], spawnPoint.transform.position, Quaternion.identity);
                        yield return cooldown;
                    }
                }
            }
            level++;
            yield return longCooldown;
        }
    }

    private void Update()
    {

    }

    private void Awake()
    {
        // Reset timeScale if it was modified
        Time.timeScale = 1;

        // Load level data
        levels = levelData.text.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);

        grid = new GameObject[HEIGHT, WIDTH];
        MakeEmptyGrid();
        MakeInitialGrid();

        if (continuing)
            LoadSave();

        continuing = false;

        memoryText.text = "Available Memory: " + memory;
        setup = false;
        StartCoroutine(SpawnTestEnemy());
    }

    private void LoadSave()
    {
        if (File.Exists(filename))
        {
            // Get data
            string fileText = File.ReadAllText(filename);
            SaveData data = JsonUtility.FromJson<SaveData>(fileText);

            // Load it
            level = data.level;
            memory = data.memory;
            loadHP = data.HP;
            int currDir;
            for (int i = 0; i < data.pathDirs.Length; i++)
            {
                currDir = data.pathDirs[i];
                ExtendPath(currDir);
                pathQueue.Enqueue(currDir);
            }
            for (int i = 0; i < data.towers.Length; i++)
            {
                grid[(int)data.coords[i].x, (int)data.coords[i].y].GetComponent<GridSpace>().SpawnTower(data.towers[i]);
            }
        }
    }

    public void Pause()
    {
        if (Time.timeScale == 0)
        {
            pausePrompt.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            pausePrompt.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void QuitToTitle()
    {
        SceneManager.LoadScene("Title Screen");
    }

    public void Save()
    {
        // Make save data
        SaveData data = new SaveData();
        data.level = level;
        data.memory = memory;
        data.HP = home.HP;
        data.pathDirs = pathQueue.ToArray();
        data.coords = towerDict.Keys.ToArray();
        data.towers = towerDict.Values.ToArray();

        // Save as json
        string jsonData = JsonUtility.ToJson(data);
        Debug.Log(jsonData);
        if (File.Exists(filename))
            File.Delete(filename);
        File.WriteAllText(filename, jsonData);
    }

    class SaveData
    {
        public int level, memory, HP;
        public int[] pathDirs;
        public Vector2[] coords;
        public GameObject[] towers;
    }

    public void TowerSpawned(GameObject tower, GameObject space)
    {
        for (int i = 0; i < HEIGHT; i++) // There's definitely an easier way to do this
        {
            for (int j = 0; j < WIDTH; j++)
            {
                if (grid[i, j] == space)
                    towerDict.Add(new Vector2(i, j), tower);
            }
        }
    }

    public void ReloadScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
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
                GridSpace space = newCell.GetComponent<GridSpace>();
                space.gameManager = this;
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

        if (!setup)
            pathQueue.Enqueue(direction);

        // Change current spawn to a path
        GridSpace spawn = spawnPoint.GetComponent<GridSpace>();
        spawn.path.Extend(direction);

        // Set new spawn point
        spawnPoint = newSpawn.gameObject;
        newSpawn.SetPath(direction);

        // Deduct price
        if (!setup)
            GainMemory(-20);

        // Reset Buttons
        Reset?.Invoke();
    }

    public bool checkLegality(int direction)
    {
        GridSpace check;
        switch (direction)
        {
            case 0:
                check = grid[spawnCoords[0] - 1, spawnCoords[1]].GetComponent<GridSpace>();
                if (check == null || !check.CheckSpawnLegality())
                    return false;
                break;

            case 1:
                check = grid[spawnCoords[0] + 1, spawnCoords[1]].GetComponent<GridSpace>();
                if (check == null || !check.CheckSpawnLegality())
                    return false;
                break;

            case 2:
                check = grid[spawnCoords[0] - 1, spawnCoords[1] - 1].GetComponent<GridSpace>();
                if (check == null || !check.CheckSpawnLegality())
                    return false;
                break;

            case 3:
                check = grid[spawnCoords[0] - 1, spawnCoords[1] + 1].GetComponent<GridSpace>();
                if (check == null || !check.CheckSpawnLegality())
                    return false;
                break;
        }
        return true;
    }

    public void GainMemory(int newMemory)
    {
        memory += newMemory;
        memoryText.text = "Available Memory: " + memory;
        Reset?.Invoke();
    }

    public void TriggerReset()
    {
        Reset?.Invoke();
    }

    IEnumerator WarmUp()
    {
        yield return warmup;
    }

}
