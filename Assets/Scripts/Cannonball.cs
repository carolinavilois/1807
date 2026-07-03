using UnityEngine;

public class Cannonball : MonoBehaviour
{
    public float speed = 4f;
    public int damage = 1;
    public float explosionRadius = 0.6f;

    Vector2 direction;
    Enemy targetEnemy;

    void Start()
    {
        Destroy(gameObject, 4f);
    }

    void Update()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;

        if (targetEnemy != null)
        {
            if (Vector2.Distance(transform.position, targetEnemy.transform.position) < 0.5f)
            {
                Explode();
            }
        }
    }

    void Explode()
    {
        EnemyMovement[] enemies = FindObjectsOfType<EnemyMovement>();
        foreach (EnemyMovement enemy in enemies)
        {
            if (Vector2.Distance(transform.position, enemy.transform.position) < explosionRadius)
            {
                enemy.GetComponent<Enemy>().TakeDamage(damage);
            }
        }
        Destroy(gameObject);
    }

    public void SetTarget(Transform enemyTransform)
    {
        targetEnemy = enemyTransform.GetComponent<Enemy>();
        direction = (enemyTransform.position - transform.position).normalized;
    }
}
