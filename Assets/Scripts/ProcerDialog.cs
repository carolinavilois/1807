using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProcerDialog : MonoBehaviour
{
    public GameObject dialogPanel;
    public Image portraitImage;
    public TextMeshProUGUI dialogText;
    public Text nameText;
    public Button entendidoButton;
    public Sprite[] procerPortraits;
    string[] procerNamesRef = { "Cornelio Saavedra", "Bernardo de Velasco", "César Balbiani", "Cornelio Saavedra" };

    void Start()
    {
        if (entendidoButton != null)
            entendidoButton.onClick.AddListener(Hide);
    }

    public void Show(string message, string name)
    {
        if (dialogPanel != null)
            dialogPanel.SetActive(true);
        if (dialogText != null)
            dialogText.text = message;
        if (nameText != null)
            nameText.text = name;
        if (portraitImage != null)
        {
            int idx = System.Array.IndexOf(procerNamesRef, name);
            if (idx >= 0 && procerPortraits != null && idx < procerPortraits.Length && procerPortraits[idx] != null)
            {
                portraitImage.sprite = procerPortraits[idx];
                portraitImage.enabled = true;
            }
            else if (portraitImage.sprite == null)
            {
                portraitImage.enabled = false;
            }
        }
    }

    void Hide()
    {
        if (dialogPanel != null)
            dialogPanel.SetActive(false);
    }

    public bool IsOpen()
    {
        return dialogPanel != null && dialogPanel.activeSelf;
    }
}