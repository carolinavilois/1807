using UnityEngine;

// Proyectil del Cañón: viaja en línea recta y al impactar explota en área
// Daña a todos los enemigos dentro del radio de explosión
public class Cannonball : MonoBehaviour
{
    public float speed = 4f;                // Velocidad del proyectil (más lento que Projectile)
    public int damage = 1;                  // Daño base por impacto
    public float explosionRadius = 0.6f;    // Radio de la explosión (dáño en área)

    Vector2 direction;                      // Dirección fija calculada al disparar
    Enemy targetEnemy;                     // Referencia al enemigo objetivo para detectar impacto

    void Start()
    {
        // Se autodestruye a los 4 segundos si no impacto (alcance máximo de la bala)
        Destroy(gameObject, 4f);
    }

    void Update()
    {
        // Se mueve en línea recta hacia donde estaba el enemigo al disparar
        transform.position += (Vector3)direction * speed * Time.deltaTime;

        // Si el enemigo sigue vivo y la bala está cerca, explota
        if (targetEnemy != null)
        {
            if (Vector2.Distance(transform.position, targetEnemy.transform.position) < 0.5f)
            {
                Explode();
            }
        }
    }

    void Explode()  // Busca todos los enemigos cerca y les aplica daño
    {
        EnemyMovement[] enemies = FindObjectsByType<EnemyMovement>(FindObjectsSortMode.None);
        foreach (EnemyMovement enemy in enemies)
        {
            if (Vector2.Distance(transform.position, enemy.transform.position) < explosionRadius)
            {
                enemy.GetComponent<Enemy>().TakeDamage(damage);
            }
        }
        Destroy(gameObject);  // La bala desaparece tras explotar
    }

    public void SetTarget(Transform enemyTransform)  // Asigna objetivo y calcula dirección inicial
    {
        targetEnemy = enemyTransform.GetComponent<Enemy>();
        direction = (enemyTransform.position - transform.position).normalized;
    }
}