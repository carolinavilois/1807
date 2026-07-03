using System.Collections.Generic;
using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    public List<Transform> waypoints;  // Puntos que los enemigos recorren en orden

    void Awake()
    {
        // Agrega todos los hijos (WP1, WP2...) como waypoints de este camino
        foreach (Transform child in transform)
        {
            waypoints.Add(child);
        }
    }

    public Transform GetWaypoint(int index)  // Devuelve el waypoint en la posici�n dada
    {
        return waypoints[index];
    }

    public int GetPathLength()  // Cantidad total de waypoints en este camino
    {
        return waypoints.Count;
    }
}
