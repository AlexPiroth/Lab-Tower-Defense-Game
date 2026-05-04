using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] int damage;
    GameObject target;
    Vector2 move;
    bool delete = false;
    [SerializeField] bool isBuildup;

    public void SetTarget(GameObject setTarget)
    {
        target = setTarget;
        move = Vector2.Normalize(target.transform.position - transform.position);
        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.AddForce(move * speed * 100);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collided = collision.gameObject;
        if (collided.CompareTag("Enemy") && !delete)
        {
            delete = true;
            testEnemy enemy = collided.GetComponent<testEnemy>();
            if (isBuildup)
            {
                Debug.Log("doing " + damage * enemy.buildup + " damage");
                enemy.health -= damage * enemy.buildup;
                enemy.buildup++;
            }
            else
                enemy.health -= damage;
            Destroy(this.gameObject);
        }
    }
}
