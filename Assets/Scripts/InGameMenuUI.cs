using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Botón pequeño (esquina superior derecha) que abre un menú flotante y pausa el juego.
// Si existen los objetos persistentes en la escena (GameMenuButton / GameMenuPanel) los usa.
// Si no, los crea en runtime (modo automático).
public class InGameMenuUI : MonoBehaviour
{
    [Header("Botón pequeño")]
    public Button menuButton;
    public TextMeshProUGUI menuButtonLabel;
    public string menuButtonText = "MENU";
    public Color buttonColor = new Color(0.75f, 0.6f, 0.4f);
    public Color textColor = new Color(0.1f, 0.06f, 0.02f);

    [Header("Menú flotante")]
    public GameObject menuPanel;
    public Button resumeButton;
    public Button restartButton;
    public Slider volumeSlider;
    public Color panelColor = new Color(0.75f, 0.6f, 0.4f);

    GameObject menuRoot;
    bool IsPaused => menuRoot != null && menuRoot.activeSelf;

    void Awake()
    {
        // Intenta resolver los objetos persistentes de la escena
        ResolvePersistentUI();

        if (menuButton != null && menuPanel != null)
        {
            menuRoot = menuPanel;
            menuRoot.SetActive(false);
            WireButtons();
        }
        else
        {
            CreateRuntimeUI();
        }
    }

    // Busca los objetos persistentes por nombre en el Canvas y asigna las referencias
    void ResolvePersistentUI()
    {
        Canvas canvas = FindCanvas();
        if (canvas == null) return;

        if (menuButton == null)
        {
            Transform t = canvas.transform.Find("GameMenuButton");
            if (t != null)
            {
                menuButton = t.GetComponent<Button>();
                if (menuButtonLabel == null)
                    menuButtonLabel = t.GetComponentInChildren<TextMeshProUGUI>(true);
            }
        }

        if (menuPanel == null)
        {
            Transform t = canvas.transform.Find("GameMenuPanel");
            if (t != null)
                menuPanel = t.gameObject;
        }

        if (menuPanel != null)
        {
            Transform card = menuPanel.transform.Find("Card");
            if (card != null)
            {
                if (resumeButton == null)
                {
                    Transform r = card.Find("ReanudarButton");
                    if (r != null) resumeButton = r.GetComponent<Button>();
                }
                if (restartButton == null)
                {
                    Transform r = card.Find("ReiniciarButton");
                    if (r != null) restartButton = r.GetComponent<Button>();
                }
                if (volumeSlider == null)
                {
                    Transform s = card.Find("SliderVolumen");
                    if (s != null) volumeSlider = s.GetComponent<Slider>();
                }
            }
        }
    }

    void WireButtons()
    {
        if (menuButton != null)
        {
            menuButton.onClick.RemoveAllListeners();
            menuButton.onClick.AddListener(ToggleMenu);
        }
        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.AddListener(CloseMenu);
        }
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartGame);
        }
        // Hover con barrido (mismo efecto que el resto de los botones)
        AddHover(resumeButton);
        AddHover(restartButton);

        // Slider de volumen maestro: controla TODO el audio vía AudioListener
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(ApplyMasterVolume);
            volumeSlider.onValueChanged.AddListener(ApplyMasterVolume);
            float saved = PlayerPrefs.GetFloat("MasterVolume", 1f);
            volumeSlider.SetValueWithoutNotify(Mathf.Clamp01(saved));
            ApplyMasterVolume(saved);
        }
    }

    void ApplyMasterVolume(float value)
    {
        AudioListener.volume = value;   // 0 = sin sonido, 1 = volumen normal
        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();
    }

    void CreateRuntimeUI()
    {
        Canvas canvas = FindCanvas();
        if (canvas == null) return;

        // Botón pequeño
        if (menuButton == null)
        {
            GameObject btn = new GameObject("GameMenuButton", typeof(RectTransform));
            btn.layer = 5;
            btn.transform.SetParent(canvas.transform, false);
            RectTransform rt = btn.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(1f, 1f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot = new Vector2(1f, 1f);
            rt.anchoredPosition = new Vector2(-12f, -12f);
            rt.sizeDelta = new Vector2(56f, 40f);

            Image img = btn.AddComponent<Image>();
            img.color = buttonColor;
            img.raycastTarget = true;

            TextMeshProUGUI label = btn.AddComponent<TextMeshProUGUI>();
            label.text = menuButtonText;
            label.alignment = TextAlignmentOptions.Center;
            label.fontSize = 20;
            label.fontStyle = FontStyles.Bold;
            label.color = textColor;
            label.raycastTarget = false;

            menuButton = btn.AddComponent<Button>();
            menuButton.targetGraphic = img;
            menuButtonLabel = label;
        }

        // Menú flotante
        if (menuPanel == null)
        {
            menuPanel = BuildMenuRuntime();
        }

        menuRoot = menuPanel;
        menuRoot.SetActive(false);
        WireButtons();
    }

    Canvas FindCanvas()
    {
        Canvas best = null;
        foreach (var c in FindObjectsByType<Canvas>(FindObjectsSortMode.None))
        {
            if (c.gameObject.name == "Canvas") return c;
            if (best == null || c.transform.childCount > best.transform.childCount)
                best = c;
        }
        return best;
    }

    GameObject BuildMenuRuntime()
    {
        Canvas canvas = FindCanvas();
        TMP_FontAsset font = null;
        TextMeshProUGUI anyText = canvas.GetComponentInChildren<TextMeshProUGUI>(true);
        if (anyText != null) font = anyText.font;

        GameObject root = new GameObject("GameMenuPanel", typeof(RectTransform));
        root.layer = 5;
        root.transform.SetParent(canvas.transform, false);
        RectTransform rootRect = root.GetComponent<RectTransform>();
        rt_FullScreen(rootRect);

        Image bg = root.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.6f);
        bg.raycastTarget = true;

        GameObject card = new GameObject("Card");
        card.layer = 5;
        card.transform.SetParent(root.transform, false);
        RectTransform cardRect = card.AddComponent<RectTransform>();
        cardRect.anchorMin = new Vector2(0.5f, 0.5f);
        cardRect.anchorMax = new Vector2(0.5f, 0.5f);
        cardRect.pivot = new Vector2(0.5f, 0.5f);
        cardRect.sizeDelta = new Vector2(380f, 260f);
        Image cardImage = card.AddComponent<Image>();
        cardImage.color = panelColor;
        cardImage.raycastTarget = true;

        // Reanudar juego
        Button resume = CreateMenuButtonInternal(card, font, "Reanudar juego", new Vector2(0f, 50f));
        resumeButton = resume;

        // REINICIAR
        Button restart = CreateMenuButtonInternal(card, font, "REINICIAR", new Vector2(0f, -40f));
        restartButton = restart;

        return root;
    }

    Button CreateMenuButtonInternal(GameObject parent, TMP_FontAsset font, string text, Vector2 position)
    {
        GameObject btn = new GameObject(text + "Button");
        btn.layer = 5;
        btn.transform.SetParent(parent.transform, false);
        RectTransform rt = btn.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = position;
        rt.sizeDelta = new Vector2(220f, 60f);

        Image img = btn.AddComponent<Image>();
        img.color = new Color(0.9f, 0.85f, 0.8f);
        img.raycastTarget = true;

        GameObject labelGO = new GameObject("Label");
        labelGO.layer = 5;
        labelGO.transform.SetParent(btn.transform, false);
        RectTransform labelRect = labelGO.AddComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        TextMeshProUGUI label = labelGO.AddComponent<TextMeshProUGUI>();
        label.text = text;
        label.alignment = TextAlignmentOptions.Center;
        label.fontSize = 26;
        label.fontStyle = FontStyles.Bold;
        label.color = textColor;
        label.font = font;
        label.raycastTarget = false;

        Button button = btn.AddComponent<Button>();
        button.targetGraphic = img;

        // Hover con barrido (mismo efecto que el resto de los botones)
        if (btn.GetComponent<ButtonHoverToggle>() == null)
            btn.AddComponent<ButtonHoverToggle>();

        return button;
    }

    void AddHover(Button button)
    {
        if (button != null && button.GetComponent<ButtonHoverToggle>() == null)
            button.gameObject.AddComponent<ButtonHoverToggle>();
    }

    void ToggleMenu()
    {
        if (IsPaused)
            CloseMenu();
        else
            OpenMenu();
    }

    void OpenMenu()
    {
        if (menuRoot == null) return;
        menuRoot.SetActive(true);
        Time.timeScale = 0f;   // Pausa al abrir

        // Música enlatada mientras el menú esté abierto
        WaveSpawner ws = FindAnyObjectByType<WaveSpawner>();
        if (ws != null) ws.SetMusicMuffled(true);
    }

    void CloseMenu()
    {
        if (menuRoot != null)
            menuRoot.SetActive(false);
        Time.timeScale = 1f;   // Reanuda al cerrar

        // Restaura la música normal
        WaveSpawner ws = FindAnyObjectByType<WaveSpawner>();
        if (ws != null) ws.SetMusicMuffled(false);
    }

    void RestartGame()
    {
        Time.timeScale = 1f;
        WaveSpawner ws = FindAnyObjectByType<WaveSpawner>();
        if (ws != null) ws.SetMusicMuffled(false);
        SceneManager.LoadScene("MenuScene");
    }

    static void rt_FullScreen(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Init()
    {
        if (SceneManager.GetActiveScene().name != "MainScene") return;
        if (FindAnyObjectByType<InGameMenuUI>() != null) return;
        GameObject host = new GameObject("InGameMenuUI");
        host.AddComponent<InGameMenuUI>();
    }
}
