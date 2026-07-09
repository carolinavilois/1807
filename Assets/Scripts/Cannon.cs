using UnityEngine;

// Unidad defensiva pesada: alto costo (60P), da�o de �rea con Cannonball, disparo muy lento (4s)
public class Cannon : MonoBehaviour
{
    public float range = 5f;                // Rango medio (menor que Tirador, mayor que Soldado)
    public float fireRate = 4f;             // El m�s lento de todas las unidades
    public GameObject projectilePrefab;     // Prefab del Cannonball (explosi�n de �rea)

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

    void Fire(Transform target)  // Dispara una bala de ca��n que explota al impactar (da�o en �rea)
    {
        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        proj.GetComponent<Cannonball>().SetTarget(target);
    }
}