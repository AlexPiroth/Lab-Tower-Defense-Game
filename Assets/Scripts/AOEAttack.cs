using System.Collections;
using UnityEngine;

public class AOEAttack : MonoBehaviour
{
    [SerializeField] float duration, maxSize;
    [SerializeField] int damage;
    CircleCollider2D circleCollider;
    SpriteRenderer spriteRenderer;
    float scale, transparency;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scale = maxSize / (duration / Time.deltaTime);
        transparency = 1f / (duration / Time.deltaTime);
        circleCollider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(nameof(Expand));
    }

    IEnumerator Expand()
    {
        while (transform.localScale.x < maxSize)
        {
            transform.localScale += (Vector3) new Vector2(scale, scale);
            circleCollider.radius = transform.localScale.x / 9;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a - transparency);
            yield return new WaitForEndOfFrame();
        }
        this.GetComponentInParent<TowerAttack>().AOEfinished = true;
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collided = collision.gameObject;
        if (collided.CompareTag("Enemy"))
        {
            testEnemy enemy = collided.GetComponent<testEnemy>();
            enemy.health -= damage;
        }
    }

}
