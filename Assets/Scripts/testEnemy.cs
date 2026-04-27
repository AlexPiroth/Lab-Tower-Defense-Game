using UnityEngine;

public class testEnemy : MonoBehaviour
{
    GameManager gameManager;
    GameObject target;
    public int health = 5;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = GameObject.Find("TestTower(Clone)");
        transform.position += new Vector3(0, 0, -1);
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
            Destroy(this.gameObject);
        transform.Translate(Vector2.Normalize(target.transform.position - transform.position) * 0.005f);
    }
}
