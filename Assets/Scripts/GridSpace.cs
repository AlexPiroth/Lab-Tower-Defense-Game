using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridSpace : MonoBehaviour, IPointerClickHandler
{
    private bool isPath = false, isOccupied = false;
    private GameObject tower;
    public PathScript path;
    [SerializeField] GameObject pathPrefab;
    public GameManager gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CheckSpawnLegality()
    {
        if (isPath || isOccupied)
            return false;
        return true;
    }

    public void SpawnTower(GameObject towerPrefab)
    {
        tower = Instantiate(towerPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z - 1), Quaternion.identity);
        isOccupied = true;
    }

    public void SetPath(int direction)
    {
        path = Instantiate(pathPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z - 1), Quaternion.identity).GetComponent<PathScript>();
        path.Spawn(direction);
        isPath = true;
    }

    public void MoveSpawnHere(GameObject spawnPoint)
    {
        tower = spawnPoint;
        isOccupied = true;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (!isOccupied && gameManager.selectedTower != null)
        {
            SpawnTower(gameManager.selectedTower);
            gameManager.SetSelectedTower(null);
        }
    }
}
