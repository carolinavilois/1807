using UnityEngine;
using UnityEngine.UI;

public class SupplyRaidUI : MonoBehaviour
{
    public GameObject supplyPanel;          // El panel UI que contiene los 3 botones de bonos
    public Button refuerzosButton;          // Botón: +5 Patriotismo por kill en próxima oleada
    public Button murallasButton;           // Botón: +2 Vidas temporales en próxima oleada
    public Button avituallamientoButton;    // Botón: 1.5x velocidad de disparo en próxima oleada

    System.Action<int> onBonusSelected;     // Callback que recibe el índice del bono elegido (0, 1, 2)
    bool waitingForChoice;                  // Evita elegir más de una vez

    void Start()
    {
        // El panel empieza oculto; se activa desde WaveSpawner cuando toca
        if (supplyPanel != null)
            supplyPanel.SetActive(false);
        // Cada botón llama a ChooseBonus con su índice fijo
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

    // Al clickear un botón, cierra el panel y ejecuta el callback con el índice
    void ChooseBonus(int index)
    {
        if (!waitingForChoice) return;  // Ignora clicks si ya eligió
        waitingForChoice = false;
        if (supplyPanel != null)
            supplyPanel.SetActive(false);
        onBonusSelected?.Invoke(index);
    }

    // útil para que WaveSpawner sepa si el panel esté abierto (bloquea Space)
    public bool IsOpen()
    {
        return supplyPanel != null && supplyPanel.activeSelf;
    }
}