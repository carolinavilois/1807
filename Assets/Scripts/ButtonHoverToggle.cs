using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Muestra la imagen de hover con un barrido lineal de izquierda a derecha.
public class ButtonHoverToggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject hoverImage;
    public float sweepDuration = 0.15f;

    public RectTransform textTransform;
    public float hoverTextScale = 1.05f;
    public float textScaleDuration = 0.12f;

    Image img;
    Coroutine running;
    RectTransform buttonRect;
    Vector3 textOriginalScale = Vector3.one;

    // Auto-agrega el efecto a todos los botones del menú y del tutorial
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void RegisterSceneHook()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "MenuScene" && scene.name != "TutorialScene") return;
        foreach (var button in Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
            if (button.GetComponent<ButtonHoverToggle>() == null)
                button.gameObject.AddComponent<ButtonHoverToggle>();
    }

    void Awake()
    {
        buttonRect = GetComponent<RectTransform>();

        // Texto: si no se asigna, busca un TMP hijo
        if (textTransform == null)
        {
            var tmp = GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (tmp != null) textTransform = tmp.rectTransform;
        }
        if (textTransform != null)
        {
            textTransform.pivot = new Vector2(0.5f, 0.5f);
            textOriginalScale = textTransform.localScale;
        }

        // Imagen de hover: si no se asigna, la crea automáticamente
        if (hoverImage == null)
        {
            GameObject go = new GameObject("BotonHoverImage");
            go.transform.SetParent(transform, false);
            var create = go.AddComponent<Image>();
            create.sprite = Resources.Load<Sprite>("Sprites/UI/Botonhover");
            create.raycastTarget = false;

            var rt = create.rectTransform;
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(0f, -buttonRect.rect.height * 0.45f);
            rt.sizeDelta = create.sprite != null ? create.sprite.rect.size : new Vector2(buttonRect.rect.width, 12f);

            hoverImage = go;
        }
    }

    void Start()
    {
        if (hoverImage == null) return;

        img = hoverImage.GetComponent<Image>();
        img.type = Image.Type.Filled;
        img.fillMethod = Image.FillMethod.Horizontal;
        img.fillOrigin = 0;     // 0 = izquierda
        img.fillAmount = 0f;
        hoverImage.SetActive(true);

        if (textTransform != null)
        {
            textTransform.pivot = new Vector2(0.5f, 0.5f);
            textOriginalScale = textTransform.localScale;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartSweep(1f);
        if (textTransform != null) StartCoroutine(AnimateTextScale(textOriginalScale * hoverTextScale));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartSweep(0f);
        if (textTransform != null) StartCoroutine(AnimateTextScale(textOriginalScale));
    }

    void StartSweep(float target)
    {
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(Sweep(target));
    }

    IEnumerator Sweep(float target)
    {
        float start = img.fillAmount;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.001f, sweepDuration);
            img.fillAmount = Mathf.Lerp(start, target, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
        img.fillAmount = target;
        running = null;
    }

    IEnumerator AnimateTextScale(Vector3 target)
    {
        Vector3 start;
        if (textTransform == null) yield break;
        start = textTransform.localScale;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.001f, textScaleDuration);
            textTransform.localScale = Vector3.Lerp(start, target, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
        textTransform.localScale = target;
    }
}