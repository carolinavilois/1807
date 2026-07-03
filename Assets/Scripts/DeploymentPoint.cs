using UnityEngine;

public class DeploymentPoint : MonoBehaviour
{
    // Al hacer click en este punto del mapa, abre el panel de selecci�n de unidades
    void OnMouseDown()
    {
        FindAnyObjectByType<UnitSelectionUI>().Show(transform);
    }
}
