using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSelectionUI : MonoBehaviour
{
    public GameObject selectionPanel;
    public GameObject destroyPanel;
    public GameObject soldierPrefab;
    public int soldierCost = 20;
    public GameObject sharpshooterPrefab;
    public int sharpshooterCost = 40;
    public GameObject cannonPrefab;
    public int cannonCost = 60;
    public UnityEngine.UI.Button soldierButton;
    public UnityEngine.UI.Button sharpshooterButton;
    public UnityEngine.UI.Button cannonButton;
    public UnityEngine.UI.Button destroyButton;

    Transform selectedPoint;
    GameObject selectedUnit;
    Camera cam;
    Canvas canvas;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        cam = FindAnyObjectByType<Camera>();
        if (soldierButton != null)
            soldierButton.onClick.AddListener(DeploySoldier);
        if (sharpshooterButton != null)
            sharpshooterButton.onClick.AddListener(DeploySharpshooter);
        if (cannonButton != null)
            cannonButton.onClick.AddListener(DeployCannon);
        if (destroyButton != null)
            destroyButton.onClick.AddListener(DestroyUnit);
        if (selectionPanel != null)
            selectionPanel.SetActive(false);
        if (destroyPanel != null)
            destroyPanel.SetActive(false);
    }

    void Update()
    {
        if (selectionPanel != null && selectionPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                selectionPanel.SetActive(false);
                return;
            }
            if (Input.GetMouseButtonDown(0))
            {
                RectTransform panelRect = selectionPanel.GetComponent<RectTransform>();
                if (!RectTransformUtility.RectangleContainsScreenPoint(panelRect, Input.mousePosition, cam))
                {
                    selectionPanel.SetActive(false);
                }
            }
        }

        if (destroyPanel != null && destroyPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                destroyPanel.SetActive(false);
                return;
            }
            if (Input.GetMouseButtonDown(0))
            {
                RectTransform panelRect = destroyPanel.GetComponent<RectTransform>();
                if (!RectTransformUtility.RectangleContainsScreenPoint(panelRect, Input.mousePosition, cam))
                {
                    destroyPanel.SetActive(false);
                }
            }
        }
    }

    public void Show(Transform point)
    {
        // Busca si ya hay una unidad en este punto
        Collider2D[] hits = Physics2D.OverlapCircleAll(point.position, 0.3f);
        foreach (Collider2D hit in hits)
        {
            if (hit.GetComponent<Soldier>() != null ||
                hit.GetComponent<Sharpshooter>() != null ||
                hit.GetComponent<Cannon>() != null)
            {
                selectedUnit = hit.gameObject;
                selectedPoint = point;
                destroyPanel.SetActive(true);
                Vector2 screenPos = cam.WorldToScreenPoint(point.position);
                RectTransform canvasRect = canvas.GetComponent<RectTransform>();
                RectTransform panelRect = destroyPanel.GetComponent<RectTransform>();
                Vector2 anchoredPos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, cam, out anchoredPos);
                panelRect.anchoredPosition = anchoredPos + new Vector2(100, 0);
                return;
            }
        }

        selectedPoint = point;
        selectionPanel.SetActive(true);
        Vector2 screenPos2 = cam.WorldToScreenPoint(point.position);
        RectTransform canvasRect2 = canvas.GetComponent<RectTransform>();
        RectTransform panelRect2 = selectionPanel.GetComponent<RectTransform>();
        Vector2 anchoredPos2;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect2, screenPos2, cam, out anchoredPos2);
        panelRect2.anchoredPosition = anchoredPos2 + new Vector2(100, 0);
    }

    void DestroyUnit()
    {
        if (selectedUnit != null)
            Destroy(selectedUnit);
        if (destroyPanel != null)
            destroyPanel.SetActive(false);
    }

    void DeploySoldier()
    {
        if (selectedPoint == null) return;
        if (PatriotismManager.Instance.Spend(soldierCost))
        {
            Instantiate(soldierPrefab, selectedPoint.position, Quaternion.identity);
            selectionPanel.SetActive(false);
        }
        else
        {
            Debug.Log("No tenés suficiente Patriotismo. Necesités: " + soldierCost);
        }
    }

    void DeploySharpshooter()
    {
        if (selectedPoint == null) return;
        if (PatriotismManager.Instance.Spend(sharpshooterCost))
        {
            Instantiate(sharpshooterPrefab, selectedPoint.position, Quaternion.identity);
            selectionPanel.SetActive(false);
        }
        else
        {
            Debug.Log("No tenés suficiente Patriotismo. Necesités: " + sharpshooterCost);
        }
    }

    void DeployCannon()
    {
        if (selectedPoint == null) return;
        if (PatriotismManager.Instance.Spend(cannonCost))
        {
            Instantiate(cannonPrefab, selectedPoint.position, Quaternion.identity);
            selectionPanel.SetActive(false);
        }
        else
        {
            Debug.Log("No tenés suficiente Patriotismo. Necesités: " + cannonCost);
        }
    }

    public void Hide()
    {
        selectionPanel.SetActive(false);
    }
}
