using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Efecto de ambiente progresivo: tarde -> noche según la oleada.
// Overlay de pantalla completa con tinte variable, debajo del HUD/UI.
// v3: dropdown de Modo de capa (Normal/Multiply/Screen/Darken/Lighten) en el Inspector.
public class DayNightOverlay : MonoBehaviour
{
    public enum OverlayType { SunsetToNight, Sunset, Night, Black }

    public enum LayerMode { Normal, Multiply, Screen, Darken, Lighten }

    [Header("Tipo de overlay")]
    public OverlayType overlayType = OverlayType.SunsetToNight;

    [Header("Modo de capa")]
    public LayerMode layerMode = LayerMode.Normal;

    [Header("Tinte (color por oleada)")]
    public Color sunsetColor = new Color(1f, 0.82f, 0.55f, 1f);  // tarde cálida
    public Color nightColor = new Color(0.05f, 0.08f, 0.18f, 1f);  // noche fría

    [Header("Intensidad")]
    [Range(0f, 1f)] public float maxAlpha = 0.5f;
    public float fadeSpeed = 0.6f;

    float currentAlpha;
    float targetAlpha;
    WaveSpawner spawner;
    Image overlay;
    Material blendMat;

    void Awake()
    {
        spawner = FindAnyObjectByType<WaveSpawner>();

        Canvas canvas = spawner != null ? spawner.GetComponentInChildren<Canvas>(true) : null;
        if (canvas == null)
            canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null) return;

        GameObject go = new GameObject("DayNightOverlay", typeof(RectTransform));
        go.layer = 5;
        go.transform.SetParent(canvas.transform, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        // Arriba de todo para verificar visibilidad
        rt.SetAsLastSibling();

        overlay = go.AddComponent<Image>();
        overlay.color = new Color(0f, 0f, 0f, 0f);
        overlay.raycastTarget = false;

        Shader shader = Shader.Find("Custom/UIOverlayBlend");
        if (shader != null)
        {
            blendMat = new Material(shader);
            overlay.material = blendMat;
        }

        if (spawner != null)
            spawner.WaveStarted += OnWaveStarted;
    }

    void OnValidate()
    {
        ApplyBlendMode();
    }

    void ApplyBlendMode()
    {
        if (blendMat == null) return;

        float src, dst, op;
        switch (layerMode)
        {
            case LayerMode.Multiply:
                src = (float)BlendMode.DstColor; dst = (float)BlendMode.Zero; op = (float)BlendOp.Add;
                break;
            case LayerMode.Screen:
                src = (float)BlendMode.OneMinusDstColor; dst = (float)BlendMode.One; op = (float)BlendOp.Add;
                break;
            case LayerMode.Darken:
                src = (float)BlendMode.One; dst = (float)BlendMode.One; op = (float)BlendOp.Min;
                break;
            case LayerMode.Lighten:
                src = (float)BlendMode.One; dst = (float)BlendMode.One; op = (float)BlendOp.Max;
                break;
            default:
                src = (float)BlendMode.SrcAlpha; dst = (float)BlendMode.OneMinusSrcAlpha; op = (float)BlendOp.Add;
                break;
        }
        blendMat.SetFloat("_SrcBlend", src);
        blendMat.SetFloat("_DstBlend", dst);
        blendMat.SetFloat("_BlendOp", op);
    }

    void OnWaveStarted(int waveIndex)
    {
        int total = spawner.WaveCount;
        float t = (total <= 1) ? 1f : Mathf.Clamp01(waveIndex / (float)(total - 1));
        Color tint;
        switch (overlayType)
        {
            case OverlayType.Sunset:
                tint = sunsetColor;
                targetAlpha = maxAlpha;
                break;
            case OverlayType.Night:
                tint = nightColor;
                targetAlpha = maxAlpha;
                break;
            case OverlayType.Black:
                tint = Color.black;
                targetAlpha = maxAlpha;
                break;
            default:
                tint = Color.Lerp(sunsetColor, nightColor, t);
                targetAlpha = maxAlpha * t;
                break;
        }
        overlay.color = new Color(tint.r, tint.g, tint.b, currentAlpha);
    }

    void Update()
    {
        if (overlay == null) return;
        currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, fadeSpeed * Time.deltaTime);
        Color c = overlay.color;
        c.a = currentAlpha;
        overlay.color = c;
    }

    void OnDestroy()
    {
        if (spawner != null)
            spawner.WaveStarted -= OnWaveStarted;
        if (blendMat != null)
            Destroy(blendMat);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Init()
    {
        if (SceneManager.GetActiveScene().name != "MainScene") return;
        if (FindAnyObjectByType<DayNightOverlay>() != null) return;
        if (FindAnyObjectByType<WaveSpawner>() == null) return;
        GameObject host = new GameObject("DayNightOverlayHost");
        host.AddComponent<DayNightOverlay>();
    }
}