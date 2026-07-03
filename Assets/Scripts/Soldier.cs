using UnityEngine;

public class Soldier : MonoBehaviour
{
    public float range = 4f;                // Distancia m�xima a la que detecta enemigos
    public float fireRate = 1.5f;           // Segundos entre cada disparo
    public GameObject projectilePrefab;     // Prefab del proyectil que dispara

    float lastFireTime;                     // �ltimo momento en que dispar�

    void Update()
    {
        Transform target = FindTarget();
        // Si hay enemigo cerca y pas� el tiempo de recarga, dispara
        if (target != null && Time.time >= lastFireTime + fireRate)
        {
            Fire(target);
            lastFireTime = Time.time;
        }
    }

    Transform FindTarget()  // Busca el enemigo m�s cercano dentro del rango
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

    void Fire(Transform target)  // Crea un proyectil en la posici�n del soldado hacia el objetivo
    {
        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        proj.GetComponent<Projectile>().SetTarget(target);
    }
}
