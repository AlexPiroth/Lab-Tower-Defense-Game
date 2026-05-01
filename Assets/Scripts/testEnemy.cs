using UnityEngine;

public class testEnemy : MonoBehaviour
{
    GameManager gameManager;
    public GameObject target;
    public int health = 5;
    public float speed = 0.005f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = PathScript.previousPathNode;
        Debug.Log(PathScript.previousPathNode.name);
        transform.position += new Vector3(0, 0, -1);
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
            Destroy(this.gameObject);
        Vector2 dist = Vector2.Normalize(target.transform.position - transform.position) * speed;
        if (Vector2.Distance(transform.position, target.transform.position) > dist.magnitude)
            transform.Translate(Vector2.Normalize(target.transform.position - transform.position) * 0.005f);
        else
        {
            transform.Translate(target.transform.position - transform.position);
            PathNode pathNode = target.GetComponent<PathNode>();
            target = pathNode.nextNode;
        }
    }
}
