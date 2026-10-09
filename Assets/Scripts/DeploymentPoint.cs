using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

// Punto de despliegue en el mapa donde el jugador puede colocar unidades defensivas
// Detecta clicks del mouse y abre el panel de selecci\u00f3n/destrucci\u00f3n de unidades
public class DeploymentPoint : MonoBehaviour
{
    readonly List<RaycastResult> pointerHits = new List<RaycastResult>();

    bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;

        var pointer = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        pointerHits.Clear();
        EventSystem.current.RaycastAll(pointer, pointerHits);

        // Physics2DRaycaster also hits deployment colliders; only UI blocks selection.
        foreach (var hit in pointerHits)
        {
            if (hit.module is GraphicRaycaster)
                return true;
        }
        return false;
    }
    void Update()
    {
        // Solo reacciona al click izquierdo del mouse
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUI())
                return;

            // Convierte posici\u00f3n del mouse a coordenadas del mundo (necesario con Canvas en modo c\u00e1mara)
            Camera cam = FindAnyObjectByType<Camera>();
            if (cam == null) return;
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

            // Si el click est\u00e1 dentro del collider de este DeploymentPoint, abre el panel
            if (GetComponent<Collider2D>() != null && GetComponent<Collider2D>().OverlapPoint(mousePos))
            {
                FindAnyObjectByType<UnitSelectionUI>().Show(transform);
            }
        }
    }
}