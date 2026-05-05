using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAttack : Tower
{
    [SerializeField] GameObject bullet;
    public bool ransom = false, AOEfinished = true;
    [SerializeField] float cooldownTime;
    List<GameObject> targets = new List<GameObject>();
    WaitForSeconds cooldown;
    [SerializeField] bool isRandom, isAOE;

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
        cooldown = new WaitForSeconds(cooldownTime);
        StartCoroutine(Shoot());
    }

    private void Update()
    {
        for (int i = 0; i < targets.Count; i++)
        {
            testEnemy targetScript = targets[i].GetComponent<testEnemy>();
            if (!targetScript.detectable)
                targets.Remove(targets[i]);
        }

    }

    IEnumerator Shoot()
    {
        while (true)
        {
            if (!ransom)
            {
                if (isAOE && AOEfinished)
                {
                    Instantiate(bullet, transform.position + new Vector3(0, 0, 0.01f), Quaternion.identity, transform);
                    AOEfinished = false;
                    yield return cooldown;
                }
                else if (targets.Count > 0)
                {
                    foreach (GameObject obj in targets)
                        if (obj == null)
                            targets.Remove(obj);
                    GameObject newBullet = Instantiate(bullet, transform.position, Quaternion.identity);
                    Bullet bulletScript = newBullet.GetComponent<Bullet>();
                    if (isRandom)
                        bulletScript.SetTarget(targets[Random.Range(0, targets.Count)]);
                    else
                        bulletScript.SetTarget(targets[0]);
                    yield return cooldown;
                }
                else
                    yield return null;
            }
            else
                yield return null;
        }
    }
}
