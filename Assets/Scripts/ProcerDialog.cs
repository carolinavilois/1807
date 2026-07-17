using UnityEngine;
using UnityEngine.UI;

// Di�logo del pr�cer que aparece entre oleadas con informaci�n de la pr�xima
// Muestra un retrato, el nombre del pr�cer y un mensaje con los caminos amenazados
public class ProcerDialog : MonoBehaviour
{
    public GameObject dialogPanel;       // Panel UI que se activa/desactiva
    public Image portraitImage;          // Retrato del pr�cer (placeholder por ahora)
    public Text dialogText;              // Texto del mensaje (caminos y cantidad de enemigos)
    public Text nameText;                // Nombre del pr�cer (Nombre 1/2/3/4 seg�n la oleada)
    public Button entendidoButton;       // Bot�n para cerrar el di�logo

    void Start()
    {
        // Conecta el bot�n Entendido para ocultar el di�logo
        if (entendidoButton != null)
            entendidoButton.onClick.AddListener(Hide);
    }

    // Muestra el di�logo con el mensaje y el nombre del pr�cer
    public void Show(string message, string name)
    {
        if (dialogPanel != null)
            dialogPanel.SetActive(true);
        if (dialogText != null)
            dialogText.text = message;
        if (nameText != null)
            nameText.text = name;
    }

    void Hide()  // Cierra el di�logo (llamado por el bot�n Entendido)
    {
        if (dialogPanel != null)
            dialogPanel.SetActive(false);
    }

    // �til para que WaveSpawner sepa si el di�logo sigue abierto (bloquea Space)
    public bool IsOpen()
    {
        return dialogPanel != null && dialogPanel.activeSelf;
    }
}