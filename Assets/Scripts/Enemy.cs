using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 1;           // Vida máxima del enemigo aunque no se cambia acá, esto es default, se cambia en unity
    public int patriotismReward = 5;    // Patriotismo que da al morir
    public int enemyTypeIndex;          // 0=E1, 1=E2, 2=E3

    int currentHealth;                  // Vida actual
    Image healthBarFill;                // Referencia al fill de la barra de vida
    float healthBarMaxWidth;            // Ancho m�ximo de la barra (para escalar al recibir daño)

    void Start()
    {
        currentHealth = maxHealth;
        CreateHealthBar();
    }

    void CreateHealthBar()
    {
        // Crea un Canvas en WorldSpace como hijo del enemigo (la barra sigue al enemigo)
        GameObject canvasGO = new GameObject("HealthBarCanvas");
        canvasGO.transform.SetParent(transform);
        canvasGO.transform.localPosition = new Vector3(0, 1, 0);  // AC� se cambia la altura de la barra

        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 10;       // La barra se ve encima del enemigo

        float barWidth = 1f;       // Ancho de la barra
        float barHeight = 0.08f;   // Alto (grosor) de la barra

        RectTransform canvasRect = canvasGO.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(barWidth, barHeight);

        // Fill: la parte coloreada (verde/amarillo/rojo) que se achica al recibir da�o
        GameObject fillGO = new GameObject("Fill");
        fillGO.transform.SetParent(canvasGO.transform);
        healthBarFill = fillGO.AddComponent<Image>();
        healthBarFill.color = Color.green;

        RectTransform fillRect = fillGO.GetComponent<RectTransform>();
        fillRect.sizeDelta = new Vector2(barWidth, barHeight);
        fillRect.localPosition = Vector3.zero;
        fillRect.anchorMin = new Vector2(0, 0.5f);   // Anclado a la izquierda
        fillRect.anchorMax = new Vector2(0, 0.5f);
        fillRect.pivot = new Vector2(0, 0.5f);       // Pivote a la izquierda (se encoge desde la derecha)

        healthBarMaxWidth = barWidth;
    }

    void UpdateHealthBar()
    {
        // Escala el ancho del fill seg�n el % de vida restante
        if (healthBarFill == null) return;

        float pct = (float)currentHealth / maxHealth;
        RectTransform fillRect = healthBarFill.rectTransform;
        fillRect.sizeDelta = new Vector2(healthBarMaxWidth * pct, fillRect.sizeDelta.y);

        // Cambia color seg�n vida: verde > 50%, amarillo > 25%, rojo < 25%
        if (pct > 0.5f)
            healthBarFill.color = Color.green;
        else if (pct > 0.25f)
            healthBarFill.color = Color.yellow;
        else
            healthBarFill.color = Color.red;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            PatriotismManager.Instance.AddKillReward(patriotismReward);
            FindAnyObjectByType<WaveSpawner>().EnemyDied(enemyTypeIndex);
            Destroy(gameObject);
        }
    }
}
