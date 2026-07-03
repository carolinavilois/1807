using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSelectionUI : MonoBehaviour
{
    public GameObject selectionPanel;
    public GameObject soldierPrefab;
    public int soldierCost = 20;
    public GameObject sharpshooterPrefab;
    public int sharpshooterCost = 40;
    public GameObject cannonPrefab;
    public int cannonCost = 60;
    public UnityEngine.UI.Button soldierButton;
    public UnityEngine.UI.Button sharpshooterButton;
    public UnityEngine.UI.Button cannonButton;

    Transform selectedPoint;
    Camera cam;
    Canvas canvas;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        cam = FindFirstObjectByType<Camera>();
        if (soldierButton != null)
            soldierButton.onClick.AddListener(DeploySoldier);
        if (sharpshooterButton != null)
            sharpshooterButton.onClick.AddListener(DeploySharpshooter);
        if (cannonButton != null)
            cannonButton.onClick.AddListener(DeployCannon);
        if (selectionPanel != null)
            selectionPanel.SetActive(false);
    }

    void Update()
    {
        if (selectionPanel == null || !selectionPanel.activeSelf)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            selectionPanel.SetActive(false);
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            RectTransform panelRect = selectionPanel.GetComponent<RectTransform>();
            Vector2 mousePos = Input.mousePosition;
            if (!RectTransformUtility.RectangleContainsScreenPoint(panelRect, mousePos, cam))
            {
                selectionPanel.SetActive(false);
            }
        }
    }

    public void Show(Transform point)
    {
        selectedPoint = point;
        selectionPanel.SetActive(true);
        Vector2 screenPos = cam.WorldToScreenPoint(point.position);
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        RectTransform panelRect = selectionPanel.GetComponent<RectTransform>();
        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, cam, out anchoredPos);
        panelRect.anchoredPosition = anchoredPos + new Vector2(100, 0);
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
            Debug.Log("No ten�s suficiente Patriotismo. Necesit�s: " + soldierCost);
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
            Debug.Log("No ten�s suficiente Patriotismo. Necesit�s: " + sharpshooterCost);
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
            Debug.Log("No ten�s suficiente Patriotismo. Necesit�s: " + cannonCost);
        }
    }

    public void Hide()
    {
        selectionPanel.SetActive(false);
    }
}
