using UnityEngine;

// Proyectil básico: viaja en línea recta hacia donde estaba el enemigo al disparar (sin tracking)
// Usado por Soldado y Tirador. Daño simple a un solo enemigo.
public class Projectile : MonoBehaviour
{
    public float speed = 5f;    // Velocidad a la que vuela el proyectil
    public int damage = 1;      // Daño que hace al impactar

    Vector2 direction;          // Dirección fija calculada al disparar (no persigue al enemigo)
    Enemy targetEnemy;          // Referencia al Enemy para hacerle daño

    void Start()
    {
        // Se autodestruye a los 3 segundos si no impactó (límite de alcance)
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        // Se mueve en línea recta hacia donde estaba el enemigo al disparar
        transform.position += (Vector3)direction * speed * Time.deltaTime;

        // Si el enemigo sigue vivo y el proyectil est\u00e1 cerca, impacta
        if (targetEnemy != null)
        {
            if (Vector2.Distance(transform.position, targetEnemy.transform.position) < 0.5f)
            {
                targetEnemy.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }

    public void SetTarget(Transform enemyTransform)  // Calcula direcci\u00f3n al momento de disparar
    {
        targetEnemy = enemyTransform.GetComponent<Enemy>();
        direction = (enemyTransform.position - transform.position).normalized;
    }
}