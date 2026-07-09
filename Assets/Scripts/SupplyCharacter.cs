using UnityEngine;

// Personaje de la Cabalgata de Abastecimiento que aparece tras el di�logo del pr�cer
// Camina r�pido por un path aleatorio y al llegar al final llama al callback para mostrar los bonos
public class SupplyCharacter : MonoBehaviour
{
    public float speed = 6f;               // 3x m�s r�pido que los enemigos (speed=2)
    System.Action onArrivedAtEnd;          // Callback cuando llega al �ltimo waypoint

    WaypointPath path;                     // Camino asignado al spawnear
    int currentWaypointIndex;              // Pr�ximo waypoint al que moverse

    // Asigna un camino y arranca la animaci�n; al llegar al final ejecuta callback
    public void SetPathAndGo(WaypointPath wpPath, System.Action callback)
    {
        path = wpPath;
        onArrivedAtEnd = callback;
        currentWaypointIndex = 0;
        transform.position = path.GetWaypoint(0).position;  // Aparece en la puerta (WP0)
    }

    void Update()
    {
        if (path == null || currentWaypointIndex >= path.GetPathLength())
            return;  // Ya lleg�, espera orden o ya se fue

        // Moverse hacia el pr�ximo waypoint
        Transform target = path.GetWaypoint(currentWaypointIndex);
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // Si lleg� al waypoint actual, pasar al siguiente
        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            currentWaypointIndex++;
            // Si lleg� al �ltimo waypoint, avisar y destruirse
            if (currentWaypointIndex >= path.GetPathLength())
            {
                path = null;
                onArrivedAtEnd?.Invoke();   // Dispara el panel de bonos desde WaveSpawner
                Destroy(gameObject);         // El personaje desaparece al "entrar a la base"
            }
        }
    }
}