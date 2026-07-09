using UnityEngine;

// Punto de despliegue en el mapa donde el jugador puede colocar unidades defensivas
// Detecta clicks del mouse y abre el panel de selecci\u00f3n/destrucci\u00f3n de unidades
public class DeploymentPoint : MonoBehaviour
{
    void Update()
    {
        // Solo reacciona al click izquierdo del mouse
        if (Input.GetMouseButtonDown(0))
        {
            // Convierte posici\u00f3n del mouse a coordenadas del mundo (necesario con Canvas en modo c\u00e1mara)
            Camera cam = FindObjectOfType<Camera>();
            if (cam == null) return;
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

            // Si el click est\u00e1 dentro del collider de este DeploymentPoint, abre el panel
            if (GetComponent<Collider2D>() != null && GetComponent<Collider2D>().OverlapPoint(mousePos))
            {
                FindObjectOfType<UnitSelectionUI>().Show(transform);
            }
        }
    }
}