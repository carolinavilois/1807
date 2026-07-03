using UnityEngine;

public class Cannon : MonoBehaviour
{
    public float range = 5f;
    public float fireRate = 4f;
    public GameObject projectilePrefab;

    float lastFireTime;

    void Update()
    {
        Transform target = FindTarget();
        if (target != null && Time.time >= lastFireTime + fireRate)
        {
            Fire(target);
            lastFireTime = Time.time;
        }
    }

    Transform FindTarget()
    {
        float closestDistance = range;
        Transform closestEnemy = null;

        EnemyMovement[] enemies = FindObjectsOfType<EnemyMovement>();
        foreach (EnemyMovement enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }
        return closestEnemy;
    }

    void Fire(Transform target)
    {
        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        proj.GetComponent<Cannonball>().SetTarget(target);
    }
}
