using UnityEngine;

// Controla el movimiento del enemigo a lo largo de los waypoints de su camino asignado
// Al llegar al �ltimo waypoint (la base), avisa al WaveSpawner y se destruye
// Adem�s, ataca en melee a las unidades defensivas que encuentra en el camino
public class EnemyMovement : MonoBehaviour
{
    public WaypointPath waypointPath;  // Camino que este enemigo va a seguir (asignado por WaveSpawner)
    public float speed = 2f;           // Velocidad de movimiento (p�xels/segundo)
    public float meleeRange = 0.8f;    // Distancia a la que puede atacar una unidad defensiva
    public float attackCooldown = 1f;  // Segundos entre cada ataque melee

    int currentWaypointIndex = 0;      // �ndice del pr�ximo waypoint al que dirigirse
    float lastAttackTime;              // �ltimo momento en que atac�

    void Start()
    {
        // Arranca en el primer waypoint del camino
        transform.position = waypointPath.GetWaypoint(0).position;
        // No ataca al instante de aparecer; espera el cooldown completo
        lastAttackTime = Time.time;
    }

    void Update()
    {
        // Si el enemigo muri� (est� en animaci�n de muerte), no se mueve ni ataca
        if (GetComponent<Enemy>().isDead) return;

        // Intenta atacar a una unidad defensiva cercana (una vez por cooldown)
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            TryMeleeAttack();
        }

        // Mientras queden waypoints, moverse hacia el pr�ximo
        if (currentWaypointIndex < waypointPath.GetPathLength())
        {
            Transform target = waypointPath.GetWaypoint(currentWaypointIndex);
            transform.position = Vector2.MoveTowards(
                transform.position, target.position, speed * Time.deltaTime
            );

            // Si lleg� al waypoint actual, pasar al siguiente
            if (Vector2.Distance(transform.position, target.position) < 0.1f)
            {
                currentWaypointIndex++;

                // Si lleg� al �ltimo (la base), avisar al WaveSpawner y destruirse
                if (currentWaypointIndex >= waypointPath.GetPathLength())
                {
                    int typeIndex = GetComponent<Enemy>().enemyTypeIndex;
                    FindAnyObjectByType<WaveSpawner>().EnemyReachedBase(typeIndex);
                    Destroy(gameObject);
                }
            }
        }
    }

    void TryMeleeAttack()
    {
        int damage = GetComponent<Enemy>().meleeDamage;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, meleeRange);
        foreach (Collider2D hit in hits)
        {
            Soldier s = hit.GetComponent<Soldier>();
            if (s != null) { s.TakeDamage(damage); lastAttackTime = Time.time; return; }
            Sharpshooter ss = hit.GetComponent<Sharpshooter>();
            if (ss != null) { ss.TakeDamage(damage); lastAttackTime = Time.time; return; }
            Cannon c = hit.GetComponent<Cannon>();
            if (c != null) { c.TakeDamage(damage); lastAttackTime = Time.time; return; }
        }
    }

    // Asigna un camino al enemigo (usado por WaveSpawner al crearlo)
    public void SetPath(WaypointPath path)
    {
        waypointPath = path;
        currentWaypointIndex = 0;
        transform.position = waypointPath.GetWaypoint(0).position;
    }
}