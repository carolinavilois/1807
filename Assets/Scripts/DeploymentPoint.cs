using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

// Punto de despliegue en el mapa donde el jugador puede colocar unidades defensivas
// Detecta clicks del mouse y abre el panel de selecci\u00f3n/destrucci\u00f3n de unidades
public class DeploymentPoint : MonoBehaviour
{
    readonly List<RaycastResult> pointerHits = new List<RaycastResult>();
    readonly Collider2D[] occupancyHits = new Collider2D[8];

    public float occupancyRadius = 0.3f;
    public float occupancyCheckInterval = 0.1f;

    SpriteRenderer marker;
    GameObject occupant;
    float nextOccupancyCheck;

    void Awake()
    {
        marker = GetComponent<SpriteRenderer>();
    }

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
        UpdateMarkerVisibility();

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

    void UpdateMarkerVisibility()
    {
        if (marker == null) return;

        // Mientras la unidad exista, el punto queda oculto aunque su collider
        // se desactive durante la animacion de muerte.
        if (occupant != null)
        {
            if (marker.enabled) marker.enabled = false;
            return;
        }

        if (Time.time < nextOccupancyCheck) return;
        nextOccupancyCheck = Time.time + Mathf.Max(0.02f, occupancyCheckInterval);

        occupant = FindAlliedUnit();
        marker.enabled = occupant == null;
    }

    GameObject FindAlliedUnit()
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;
        filter.useLayerMask = false;
        filter.useDepth = false;
        filter.useNormalAngle = false;

        int count = Physics2D.OverlapCircle(transform.position, occupancyRadius, filter, occupancyHits);
        for (int i = 0; i < count; i++)
        {
            Collider2D hit = occupancyHits[i];
            if (hit == null) continue;

            Soldier soldier = hit.GetComponentInParent<Soldier>();
            if (soldier != null) return soldier.gameObject;

            Sharpshooter sharpshooter = hit.GetComponentInParent<Sharpshooter>();
            if (sharpshooter != null) return sharpshooter.gameObject;

            Cannon cannon = hit.GetComponentInParent<Cannon>();
            if (cannon != null) return cannon.gameObject;
        }

        return null;
    }
}