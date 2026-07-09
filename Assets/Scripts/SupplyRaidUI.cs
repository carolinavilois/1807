using UnityEngine;
using UnityEngine.UI;

public class SupplyRaidUI : MonoBehaviour
{
    public GameObject supplyPanel;          // El panel UI que contiene los 3 botones de bonos
    public Button refuerzosButton;          // Bot�n: +5 Patriotismo por kill en pr�xima oleada
    public Button murallasButton;           // Bot�n: +2 Vidas temporales en pr�xima oleada
    public Button avituallamientoButton;    // Bot�n: 1.5x velocidad de disparo en pr�xima oleada

    System.Action<int> onBonusSelected;     // Callback que recibe el �ndice del bono elegido (0, 1, 2)
    bool waitingForChoice;                  // Evita elegir m�s de una vez

    void Start()
    {
        // El panel empieza oculto; se activa desde WaveSpawner cuando toca
        if (supplyPanel != null)
            supplyPanel.SetActive(false);
        // Cada bot�n llama a ChooseBonus con su �ndice fijo
        if (refuerzosButton != null)
            refuerzosButton.onClick.AddListener(() => ChooseBonus(0));
        if (murallasButton != null)
            murallasButton.onClick.AddListener(() => ChooseBonus(1));
        if (avituallamientoButton != null)
            avituallamientoButton.onClick.AddListener(() => ChooseBonus(2));
    }

    // Muestra el panel y recibe un callback para cuando el jugador elija
    public void Show(System.Action<int> callback)
    {
        waitingForChoice = true;
        onBonusSelected = callback;
        if (supplyPanel != null)
            supplyPanel.SetActive(true);
    }

    // Al clickear un bot�n, cierra el panel y ejecuta el callback con el �ndice
    void ChooseBonus(int index)
    {
        if (!waitingForChoice) return;  // Ignora clicks si ya eligi�
        waitingForChoice = false;
        if (supplyPanel != null)
            supplyPanel.SetActive(false);
        onBonusSelected?.Invoke(index);
    }

    // �til para que WaveSpawner sepa si el panel est� abierto (bloquea Space)
    public bool IsOpen()
    {
        return supplyPanel != null && supplyPanel.activeSelf;
    }
}