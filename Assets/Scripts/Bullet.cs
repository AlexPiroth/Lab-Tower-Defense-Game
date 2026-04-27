using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed;
    GameObject target;
    Vector2 move;

    public void SetTarget(GameObject setTarget)
    {
        target = setTarget;
        move = Vector2.Normalize(target.transform.position - transform.position);
        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.AddForce(move * 300);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collided = collision.gameObject;
        if (collided.CompareTag("Enemy"))
        {
            testEnemy enemy = collided.GetComponent<testEnemy>();
            enemy.health -= 3;
            Destroy(this.gameObject);
        }
    }
}
