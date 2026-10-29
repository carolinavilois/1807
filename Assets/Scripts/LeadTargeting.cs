using UnityEngine;

// Calcula la dirección de disparo hacia el punto de interceptación de un enemigo en movimiento.
public static class LeadTargeting
{
    public static Vector2 ComputeDirection(Vector2 shooterPos, Transform enemyTransform, float projectileSpeed)
    {
        if (enemyTransform == null) return Vector2.zero;

        Vector2 enemyPos = enemyTransform.position;
        Vector2 velocity = Vector2.zero;

        EnemyMovement em = enemyTransform.GetComponent<EnemyMovement>();
        if (em != null) velocity = em.CurrentVelocity;

        Vector2 R = enemyPos - shooterPos;

        float s = projectileSpeed;
        float a = Vector2.Dot(velocity, velocity) - s * s;
        float b = 2f * Vector2.Dot(R, velocity);
        float c = Vector2.Dot(R, R);

        float t = 0f;
        if (Mathf.Approximately(a, 0f))
        {
            if (!Mathf.Approximately(b, 0f)) t = -c / b;
        }
        else
        {
            float disc = b * b - 4f * a * c;
            if (disc >= 0f)
            {
                float sqrt = Mathf.Sqrt(disc);
                float t1 = (-b - sqrt) / (2f * a);
                float t2 = (-b + sqrt) / (2f * a);
                if (t1 > 0f && t2 > 0f) t = Mathf.Min(t1, t2);
                else if (t1 > 0f) t = t1;
                else if (t2 > 0f) t = t2;
            }
        }

        if (t <= 0f || float.IsNaN(t) || float.IsInfinity(t))
        {
            // Sin solución válida: apuntar a la posición actual
            if (R.sqrMagnitude < 0.0001f) return Vector2.zero;
            return R.normalized;
        }

        Vector2 predicted = enemyPos + velocity * t;
        Vector2 dir = predicted - shooterPos;
        if (dir.sqrMagnitude < 0.0001f) return R.normalized;
        return dir.normalized;
    }
}