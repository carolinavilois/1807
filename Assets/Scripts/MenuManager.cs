using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public string creditsContent = "Nombre 1\nNombre 2\nNombre 3\nNombre 4";

    GameObject creditsPanel;

    void Start()
    {
        // Auto-conecta el click del botón "Créditos"
        foreach (var button in FindObjectsByType<Button>(FindObjectsSortMode.None))
        {
            if (button.name == "Cr\xE9ditos")
            {
                button.onClick.AddListener(ShowCredits);
                break;
            }
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("TutorialScene");
    }

    public void ShowCredits()
    {
        if (creditsPanel == null)
            creditsPanel = BuildCreditsPanel();
        creditsPanel.SetActive(true);
    }

    Canvas GetCanvas()
    {
        return GetComponentInParent<Canvas>() ?? FindAnyObjectByType<Canvas>();
    }

    // Panel de créditos con un solo texto editable (los nombres se cambian a mano)
    GameObject BuildCreditsPanel()
    {
        Canvas canvas = GetCanvas();

        TMP_FontAsset font = null;
        TextMeshProUGUI anyText = canvas.GetComponentInChildren<TextMeshProUGUI>(true);
        if (anyText != null)
            font = anyText.font;

        GameObject root = new GameObject("CreditsPanel", typeof(RectTransform));
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
        cardRect.sizeDelta = new Vector2(700, 420);
        Image cardImage = card.AddComponent<Image>();
        cardImage.color = new Color(0.75f, 0.6f, 0.4f);
        cardImage.raycastTarget = true;

        GameObject title = new GameObject("Title");
        title.layer = 5;
        title.transform.SetParent(card.transform, false);
        RectTransform titleRect = title.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0f, -40f);
        titleRect.sizeDelta = new Vector2(-40, 60);
        TextMeshProUGUI titleText = title.AddComponent<TextMeshProUGUI>();
        titleText.text = "Cr\u00E9ditos";
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.fontSize = 36;
        titleText.color = new Color(0.1f, 0.06f, 0.02f);
        titleText.font = font;
        titleText.raycastTarget = false;

        GameObject content = new GameObject("Content");
        content.layer = 5;
        content.transform.SetParent(card.transform, false);
        RectTransform contentRect = content.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0.5f, 0.5f);
        contentRect.anchorMax = new Vector2(0.5f, 0.5f);
        contentRect.pivot = new Vector2(0.5f, 0.5f);
        contentRect.anchoredPosition = new Vector2(0f, -30f);
        contentRect.sizeDelta = new Vector2(640, 220);
        TextMeshProUGUI contentText = content.AddComponent<TextMeshProUGUI>();
        contentText.text = creditsContent;
        contentText.alignment = TextAlignmentOptions.TopLeft;
        contentText.fontSize = 26;
        contentText.color = new Color(0.1f, 0.06f, 0.02f);
        contentText.font = font;
        contentText.raycastTarget = false;

        GameObject close = new GameObject("CloseButton");
        close.layer = 5;
        close.transform.SetParent(card.transform, false);          // X en la esquina superior derecha de la tarjeta
        RectTransform closeRect = close.AddComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(1f, 1f);
        closeRect.anchorMax = new Vector2(1f, 1f);
        closeRect.pivot = new Vector2(1f, 1f);
        closeRect.anchoredPosition = new Vector2(-10f, -10f);
        closeRect.sizeDelta = new Vector2(48, 48);
        Image closeImage = close.AddComponent<Image>();
        closeImage.color = new Color(0.9f, 0.85f, 0.8f);
        closeImage.raycastTarget = true;

        GameObject closeLabel = new GameObject("CloseLabel");
        closeLabel.layer = 5;
        closeLabel.transform.SetParent(close.transform, false);
        RectTransform labelRect = closeLabel.AddComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        TextMeshProUGUI closeText = closeLabel.AddComponent<TextMeshProUGUI>();
        closeText.text = "X";
        closeText.alignment = TextAlignmentOptions.Center;
        closeText.fontSize = 30;
        closeText.color = new Color(0.1f, 0.06f, 0.02f);
        closeText.font = font;
        closeText.raycastTarget = false;

        Button closeButton = close.AddComponent<Button>();
        closeButton.targetGraphic = closeImage;
        closeButton.interactable = true;
        closeButton.onClick.AddListener(ClosePanel);

        root.SetActive(false);
        return root;
    }

    void ClosePanel()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(false);
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
}