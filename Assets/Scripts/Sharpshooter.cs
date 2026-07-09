using UnityEngine;

// Unidad defensiva de largo alcance: costo medio (40P), rango alto (7), disparo lento (2s)
public class Sharpshooter : MonoBehaviour
{
    public float range = 7f;                // Mayor rango de todas las unidades
    public float fireRate = 2f;             // Dispara m�s lento que el Soldado
    public GameObject projectilePrefab;     // Proyectil simple (sin da�o de �rea)

    float lastFireTime;                     // �ltimo momento en que dispar�

    void Update()
    {
        Transform target = FindTarget();

        // Aplica multiplicador de velocidad de disparo desde las Cabalgatas (Avituallamiento)
        WaveSpawner ws = FindObjectOfType<WaveSpawner>();
        float effectiveCooldown = fireRate * (ws != null ? ws.fireRateMultiplier : 1f);

        if (target != null && Time.time >= lastFireTime + effectiveCooldown)
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

    void Fire(Transform target)  // Crea un proyectil en la posici�n del tirador hacia el objetivo
    {
        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        proj.GetComponent<Projectile>().SetTarget(target);
    }
}