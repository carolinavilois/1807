using TMPro;
using UnityEngine;

public class PatriotismManager : MonoBehaviour
{
    public static PatriotismManager Instance;  // Singleton: accesible desde cualquier otro script

    public int startingPatriotism = 30;     // Patriotismo inicial al empezar la partida
    public int passiveIncome = 1;           // Cuanto gana por segundo durante una oleada
    public TMP_Text patriotismText;

    int currentPatriotism;                  // Patriotismo actual del jugador

    void Awake()
    {
        Instance = this;  // Se registra a sí mismo para que otros scripts lo encuentren
    }

    void Start()
    {
        currentPatriotism = startingPatriotism;
        UpdateUI();
        // +1 por segundo mientras haya enemigos vivos en la oleada
        InvokeRepeating(nameof(AddPassiveIncome), 1f, 1f);

        // Auto-conecta el botón de debug de patriotismo (da 100 puntos)
        foreach (var button in FindObjectsByType<UnityEngine.UI.Button>(FindObjectsSortMode.None))
        {
            if (button.name.ToLower().StartsWith("debug patri"))
            {
                button.onClick.AddListener(AddDebug);
                break;
            }
        }
    }

    public void AddDebug()  // +100 patriotismo para pruebas
    {
        currentPatriotism += 100;
        UpdateUI();
    }

    void AddPassiveIncome()
    {
        // Solo da patriotismo pasivo si hay una oleada activa (enemigos vivos) y el juego no está pausado
        WaveSpawner ws = FindAnyObjectByType<WaveSpawner>();
        if (ws != null && ws.HasActiveEnemies() && Time.timeScale > 0f)
        {
            currentPatriotism += passiveIncome;
            UpdateUI();
        }
    }

    public void AddKillReward(int amount)  // Suma patriotismo al matar un enemigo
    {
        currentPatriotism += amount;
        UpdateUI();
    }

    public bool Spend(int amount)  // Gasta patriotismo; devuelve true si se pudo
    {
        if (currentPatriotism >= amount)
        {
            currentPatriotism -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    void UpdateUI()
    {
        if (patriotismText != null)
            patriotismText.text = "Patriotismo: " + currentPatriotism;
    }
}
