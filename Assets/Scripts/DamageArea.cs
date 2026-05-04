using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageArea : MonoBehaviour
{
    [SerializeField] float interval;
    [SerializeField] int damage;
    WaitForSeconds cooldown;
    List<GameObject> targets = new List<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cooldown = new WaitForSeconds(interval);
        StartCoroutine(nameof(Damage));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collided = collision.gameObject;
        if (collided.CompareTag("Enemy"))
            targets.Add(collided);
        collided.GetComponent<testEnemy>().speed /= 2f;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject collided = collision.gameObject;
        if (targets.Contains(collided))
            targets.Remove(collided);
        collided.GetComponent<testEnemy>().speed *= 2f;
    }
    IEnumerator Damage()
    {
        while (true)
        {
            foreach (GameObject target in targets)
            {
                if (target == null)
                    targets.Remove(target);
                else
                {
                    testEnemy targetScript = target.GetComponent<testEnemy>();
                    targetScript.health -= damage;
                }
            }
            yield return cooldown;
        }
    }
}
