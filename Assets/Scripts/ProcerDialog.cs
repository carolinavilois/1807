using UnityEngine;
using UnityEngine.UI;

public class ProcerDialog : MonoBehaviour
{
    public GameObject dialogPanel;
    public Image portraitImage;
    public Text dialogText;
    public Text nameText;
    public Button entendidoButton;

    void Start()
    {
        if (dialogPanel != null)
            dialogPanel.SetActive(false);
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
