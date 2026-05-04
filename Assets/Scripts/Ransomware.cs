using UnityEngine;

public class Ransomware : MonoBehaviour
{
    [SerializeField] testEnemy enemy;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("woo");
        TowerAttack collided = collision.gameObject.GetComponent<TowerAttack>();
        if (collided != null && !collided.ransom)
        {
            Debug.Log("hoo");
            enemy.target = collided.gameObject;
            enemy.SetDirection();
        }

    }
}
