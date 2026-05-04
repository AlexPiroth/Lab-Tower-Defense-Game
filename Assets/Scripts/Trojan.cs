using System.Collections;
using UnityEngine;

public class Trojan : testEnemy
{
    [SerializeField] GameObject packetEnemy;
    [SerializeField] int number;
    WaitForSeconds wait = new WaitForSeconds(1f);
    public override void Die()
    {
        Debug.Log("died");
        Collider2D hitbox = GetComponent<Collider2D>();
        hitbox.enabled = false;
        for (int i = 0; i < number; i++)
        {
            detectable = false;
            GameObject newEnemy = Instantiate(packetEnemy, this.transform.position, Quaternion.identity);
            newEnemy.transform.position += (Vector3) direction * i / 10;
            testEnemy newEnemyScript = newEnemy.GetComponent<testEnemy>();
            newEnemyScript.target = this.target;
            newEnemyScript.SetDirection();
        }
        Destroy(this.gameObject);
    }
}
