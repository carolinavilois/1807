using UnityEngine;

public class DeploymentPoint : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Camera cam = FindFirstObjectByType<Camera>();
            if (cam == null) return;
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            if (GetComponent<Collider2D>() != null && GetComponent<Collider2D>().OverlapPoint(mousePos))
            {
                FindFirstObjectByType<UnitSelectionUI>().Show(transform);
            }
        }
    }
}
