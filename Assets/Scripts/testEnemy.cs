using System.Collections;
using UnityEngine;

public class testEnemy : MonoBehaviour
{
    GameManager gameManager;
    public GameObject target;
    public int health = 5;
    [SerializeField] int damage, value;
    public float speed, attackCooldown;
    public int buildup = 0, type; // 0 = basic, 1 = ransomware
    public bool detectable = true;
    public Vector2 direction;
    Damageable attackTarget;
    WaitForSeconds cooldown;
    bool attacking = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        cooldown = new WaitForSeconds(attackCooldown);
        target = PathScript.previousPathNode;
        transform.position += new Vector3(0, 0, -2);
        SetDirection();
    }

    // Update is called once per frame
    void Update()
    {
        // Self-explanatory
        if (health <= 0)
            Die();

        // Check for a target to attack
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.05f, LayerMask.GetMask("Damageable"));
        if (hit)
        {
            if (!attacking)
            {
                attackTarget = hit.transform.gameObject.GetComponent<Damageable>();
                StartCoroutine(nameof(Attack));
                attacking = true;
            }
            return;
        }
        else if (attacking)
        {
            attackTarget = null;
            attacking = false;
            StopCoroutine(nameof(Attack));
        }

        // Calculate movement and move
        Vector2 dist = direction * speed;
        if (Vector2.Distance(transform.position, target.transform.position) > dist.magnitude)
            transform.Translate(dist);
        else if (target.name != "TestTower(Clone)")
        {
            transform.Translate(target.transform.position - transform.position);
            if (type == 1)
            {
                TowerAttack tower = target.GetComponent<TowerAttack>();
                if (tower != null)
                {
                    tower.ransom = true;
                    Destroy(this.gameObject);
                }
            }
            PathNode pathNode = target.GetComponent<PathNode>();
            target = pathNode.nextNode;
            SetDirection();
        }
    }

    public virtual void Die()
    {
        gameManager.GainMemory(value);
        Destroy(this.gameObject);
    }

    public void SetDirection()
    {
        direction = Vector2.Normalize(target.transform.position - transform.position);
    }

    public IEnumerator Attack()
    {
        while (true)
        {
            attackTarget.LoseHP(damage);
            yield return cooldown;
        }
    }
}
