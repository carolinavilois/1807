using UnityEngine;

public class PatriotismManager : MonoBehaviour
{
    public static PatriotismManager Instance;  // Singleton: accesible desde cualquier otro script

    public int startingPatriotism = 30;     // Patriotismo inicial al empezar la partida
    public int passiveIncome = 1;           // Cuanto gana por segundo durante una oleada
    public UnityEngine.UI.Text patriotismText;

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
    }

    void AddPassiveIncome()
    {
        // Solo da patriotismo pasivo si hay una oleada activa (enemigos vivos)
        WaveSpawner ws = FindAnyObjectByType<WaveSpawner>();
        if (ws != null && ws.HasActiveEnemies())
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
