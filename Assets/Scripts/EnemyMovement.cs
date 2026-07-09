using UnityEngine;

// Controla el movimiento del enemigo a lo largo de los waypoints de su camino asignado
// Al llegar al �ltimo waypoint (la base), avisa al WaveSpawner y se destruye
public class EnemyMovement : MonoBehaviour
{
    public WaypointPath waypointPath;  // Camino que este enemigo va a seguir (asignado por WaveSpawner)
    public float speed = 2f;           // Velocidad de movimiento (p�xels/segundo)

    int currentWaypointIndex = 0;      // �ndice del pr�ximo waypoint al que dirigirse

    void Start()
    {
        // Arranca en el primer waypoint del camino
        transform.position = waypointPath.GetWaypoint(0).position;
    }

    void Update()
    {
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

    // Asigna un camino al enemigo (usado por WaveSpawner al crearlo)
    public void SetPath(WaypointPath path)
    {
        waypointPath = path;
        currentWaypointIndex = 0;
        transform.position = waypointPath.GetWaypoint(0).position;
    }
}