using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAttack : MonoBehaviour
{
    [SerializeField] GameObject bullet;
    List<GameObject> targets = new List<GameObject>();
    WaitForSeconds cooldown = new WaitForSeconds(1);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collided = collision.gameObject;
        if (collided.CompareTag("Enemy"))
            targets.Add(collided);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject collided = collision.gameObject;
        if (targets.Contains(collided))
            targets.Remove(collided);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Shoot());
    }

    IEnumerator Shoot()
    {
        while (true)
        {
            if (targets.Count > 0)
            {
                Debug.Log(targets.ToString());
                GameObject newBullet = Instantiate(bullet, transform.position, Quaternion.identity);
                Bullet bulletScript = newBullet.GetComponent<Bullet>();
                bulletScript.SetTarget(targets[0]);
                yield return cooldown;
            }
            else
                yield return null;
        }
    }
}
