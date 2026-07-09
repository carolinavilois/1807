using UnityEngine;

// Proyectil del Ca��n: viaja en l�nea recta y al impactar explota en �rea
// Da�a a todos los enemigos dentro del radio de explosi�n
public class Cannonball : MonoBehaviour
{
    public float speed = 4f;                // Velocidad del proyectil (m�s lento que Projectile)
    public int damage = 1;                  // Da�o base por impacto
    public float explosionRadius = 0.6f;    // Radio de la explosi�n (da�o en �rea)

    Vector2 direction;                      // Direcci�n fija calculada al disparar
    Enemy targetEnemy;                     // Referencia al enemigo objetivo para detectar impacto

    void Start()
    {
        // Se autodestruye a los 4 segundos si no impact� (alcance m�ximo de la bala)
        Destroy(gameObject, 4f);
    }

    void Update()
    {
        // Se mueve en l�nea recta hacia donde estaba el enemigo al disparar
        transform.position += (Vector3)direction * speed * Time.deltaTime;

        // Si el enemigo sigue vivo y la bala est� cerca, explota
        if (targetEnemy != null)
        {
            if (Vector2.Distance(transform.position, targetEnemy.transform.position) < 0.5f)
            {
                Explode();
            }
        }
    }

    void Explode()  // Busca todos los enemigos cerca y les aplica da�o
    {
        EnemyMovement[] enemies = FindObjectsOfType<EnemyMovement>();
        foreach (EnemyMovement enemy in enemies)
        {
            if (Vector2.Distance(transform.position, enemy.transform.position) < explosionRadius)
            {
                enemy.GetComponent<Enemy>().TakeDamage(damage);
            }
        }
        Destroy(gameObject);  // La bala desaparece tras explotar
    }

    public void SetTarget(Transform enemyTransform)  // Asigna objetivo y calcula direcci�n inicial
    {
        targetEnemy = enemyTransform.GetComponent<Enemy>();
        direction = (enemyTransform.position - transform.position).normalized;
    }
}