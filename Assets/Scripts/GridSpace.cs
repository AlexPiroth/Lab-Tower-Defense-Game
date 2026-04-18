using Unity.VisualScripting;
using UnityEngine;

public class GridSpace : MonoBehaviour
{
    private bool isPath = false, isOccupied = false;
    private GameObject tower;
    [SerializeField] private Sprite path;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnTower(GameObject towerPrefab)
    {
        tower = Instantiate(towerPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z - 1), Quaternion.identity);
        isOccupied = true;
    }

    public void SetPath()
    {
        this.gameObject.tag = "Path";
        isPath = true;
    }

    public void MoveSpawnHere(GameObject spawnPoint)
    {
        tower = spawnPoint;
        isOccupied = true;
    }
}
