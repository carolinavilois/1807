using UnityEngine;

// Proyectil del Tirador: viaja en l�nea recta, m�s r�pido que el proyectil del Soldado
public class SharpshooterProjectile : MonoBehaviour
{
    public float speed = 7f;
    public int damage = 1;

    Vector2 direction;
    Enemy targetEnemy;

    void Start()
    {
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;

        if (targetEnemy != null)
        {
            if (Vector2.Distance(transform.position, targetEnemy.transform.position) < 0.5f)
            {
                targetEnemy.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }

    public void SetTarget(Transform enemyTransform)
    {
        targetEnemy = enemyTransform.GetComponent<Enemy>();
        direction = LeadTargeting.ComputeDirection(transform.position, enemyTransform, speed);
    }
}