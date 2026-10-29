using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Muestra un circulo con el rango de la unidad aliada que esta bajo el mouse.
// El radio coincide con el range que usan Soldier, Sharpshooter y Cannon para buscar objetivos.
public class UnitRangeHover : MonoBehaviour
{
    public Color soldierColor = new Color(0.35f, 1f, 0.45f, 1f);
    public Color sharpshooterColor = new Color(0.35f, 0.85f, 1f, 1f);
    public Color cannonColor = new Color(1f, 0.6f, 0.2f, 1f);
    public float lineWidth = 0.05f;
    public int segments = 64;
    public int sortingOrder = 3;

    readonly List<RaycastResult> pointerHits = new List<RaycastResult>();
    readonly Collider2D[] overlapBuffer = new Collider2D[16];

    LineRenderer rangeCircle;
    PointerEventData pointerData;
    Camera cam;

    void Awake()
    {
        cam = Camera.main;
        CreateRangeCircle();
        if (EventSystem.current != null)
            pointerData = new PointerEventData(EventSystem.current);
    }

    void CreateRangeCircle()
    {
        // Objeto raiz para que no herede la escala de GameLogic (1.912).
        GameObject circleGO = new GameObject("UnitRangeCircle");
        rangeCircle = circleGO.AddComponent<LineRenderer>();
        rangeCircle.useWorldSpace = true;
        rangeCircle.loop = true;
        rangeCircle.positionCount = Mathf.Max(3, segments);
        rangeCircle.widthMultiplier = lineWidth;
        rangeCircle.numCapVertices = 2;
        rangeCircle.numCornerVertices = 2;
        rangeCircle.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        rangeCircle.receiveShadows = false;
        rangeCircle.sortingOrder = sortingOrder;
        rangeCircle.material = CreateLineMaterial();
        rangeCircle.enabled = false;
    }

    static Material CreateLineMaterial()
    {
        Shader shader = Shader.Find("Sprites/Default");
        if (shader == null) shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
        if (shader == null) shader = Shader.Find("Universal Render Pipeline/Unlit");

        Material material = new Material(shader);
        if (material.HasProperty("_MainTex"))
            material.mainTexture = Texture2D.whiteTexture;
        if (material.HasProperty("_Color"))
            material.color = Color.white;
        return material;
    }

    void Update()
    {
        if (rangeCircle == null) return;

        if (cam == null) cam = Camera.main;
        if (cam == null) cam = FindAnyObjectByType<Camera>();
        if (cam == null)
        {
            Hide();
            return;
        }

        if (IsPointerOverUI())
        {
            Hide();
            return;
        }

        Vector2 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);

        if (!TryFindUnit(mouseWorld, out Transform unit, out float range, out Color color))
        {
            Hide();
            return;
        }

        ShowRange(unit, range, color);
    }

    bool TryFindUnit(Vector2 point, out Transform unit, out float range, out Color color)
    {
        unit = null;
        range = 0f;
        color = Color.white;

        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;
        filter.useLayerMask = false;
        filter.useDepth = false;
        filter.useNormalAngle = false;

        int count = Physics2D.OverlapPoint(point, filter, overlapBuffer);

        for (int i = 0; i < count; i++)
        {
            Collider2D hit = overlapBuffer[i];
            if (hit == null) continue;

            Soldier soldier = hit.GetComponentInParent<Soldier>();
            if (soldier != null)
            {
                unit = soldier.transform;
                range = soldier.range;
                color = soldierColor;
                return true;
            }

            Sharpshooter sharpshooter = hit.GetComponentInParent<Sharpshooter>();
            if (sharpshooter != null)
            {
                unit = sharpshooter.transform;
                range = sharpshooter.range;
                color = sharpshooterColor;
                return true;
            }

            Cannon cannon = hit.GetComponentInParent<Cannon>();
            if (cannon != null)
            {
                unit = cannon.transform;
                range = cannon.range;
                color = cannonColor;
                return true;
            }
        }

        return false;
    }

    void ShowRange(Transform unit, float range, Color color)
    {
        rangeCircle.enabled = true;
        rangeCircle.startColor = color;
        rangeCircle.endColor = color;

        int count = rangeCircle.positionCount;
        float step = (Mathf.PI * 2f) / count;

        // El Renderer2D ordena por eje Y dentro del mismo sortingOrder, asi que la
        // posicion Z no influye: el orden visual se controla con sortingOrder.
        Vector3 center = unit.position;

        for (int i = 0; i < count; i++)
        {
            float angle = i * step;
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * range;
            rangeCircle.SetPosition(i, center + offset);
        }
    }

    void Hide()
    {
        if (rangeCircle != null)
            rangeCircle.enabled = false;
    }

    bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;
        if (pointerData == null)
            pointerData = new PointerEventData(EventSystem.current);

        // Mismo criterio que DeploymentPoint: solo la UI (GraphicRaycaster) bloquea,
        // no los colliders de unidades o puntos de despliegue.
        pointerData.position = Input.mousePosition;
        pointerHits.Clear();
        EventSystem.current.RaycastAll(pointerData, pointerHits);

        for (int i = 0; i < pointerHits.Count; i++)
        {
            if (pointerHits[i].module is GraphicRaycaster)
                return true;
        }

        return false;
    }

    void OnDestroy()
    {
        if (rangeCircle != null)
        {
            if (rangeCircle.material != null)
                Destroy(rangeCircle.material);
            Destroy(rangeCircle.gameObject);
        }
    }

    // Red de seguridad: si la escena abierta no tiene el componente (por ejemplo,
    // porque Unity todavia no recargo MainScene desde disco), lo crea al iniciar.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void EnsureInstance()
    {
        if (FindAnyObjectByType<UnitRangeHover>() != null) return;
        if (FindAnyObjectByType<WaveSpawner>() == null) return;

        GameObject host = new GameObject("UnitRangeHover");
        host.AddComponent<UnitRangeHover>();
    }
}
