using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Unidad defensiva b�sica: bajo costo (20P), rango medio (4), velocidad de disparo normal (1.5s), vida 5
public class Soldier : MonoBehaviour
{
    public float range = 4f;
    public float fireRate = 1.5f;
    public GameObject projectilePrefab;
    public int maxHealth = 10;

    float lastFireTime;
    int currentHealth;
    SpriteRenderer spriteRenderer;
    Color originalColor;
    Image healthBarFill;
    float healthBarMaxWidth;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
        CreateHealthBar();
    }

    void CreateHealthBar()
    {
        GameObject canvasGO = new GameObject("HealthBarCanvas");
        canvasGO.transform.SetParent(transform);
        canvasGO.transform.localPosition = new Vector3(0, 0.8f, 0);

        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 10;

        float barWidth = 1f;
        float barHeight = 0.08f;

        RectTransform canvasRect = canvasGO.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(barWidth, barHeight);

        GameObject fillGO = new GameObject("Fill");
        fillGO.transform.SetParent(canvasGO.transform);
        healthBarFill = fillGO.AddComponent<Image>();
        healthBarFill.color = Color.green;

        RectTransform fillRect = fillGO.GetComponent<RectTransform>();
        fillRect.sizeDelta = new Vector2(barWidth, barHeight);
        fillRect.localPosition = Vector3.zero;
        fillRect.anchorMin = new Vector2(0, 0.5f);
        fillRect.anchorMax = new Vector2(0, 0.5f);
        fillRect.pivot = new Vector2(0, 0.5f);

        healthBarMaxWidth = barWidth;
    }

    void UpdateHealthBar()
    {
        if (healthBarFill == null) return;
        float pct = (float)currentHealth / maxHealth;
        RectTransform fillRect = healthBarFill.rectTransform;
        fillRect.sizeDelta = new Vector2(healthBarMaxWidth * pct, fillRect.sizeDelta.y);

        if (pct > 0.5f)
            healthBarFill.color = Color.green;
        else if (pct > 0.25f)
            healthBarFill.color = Color.yellow;
        else
            healthBarFill.color = Color.red;
    }

    void Update()
    {
        if (currentHealth <= 0) return;

        Transform target = FindTarget();

        WaveSpawner ws = FindAnyObjectByType<WaveSpawner>();
        float effectiveCooldown = fireRate * (ws != null ? ws.fireRateMultiplier : 1f);

        if (target != null && Time.time >= lastFireTime + effectiveCooldown)
        {
            Fire(target);
            lastFireTime = Time.time;
        }
    }

    Transform FindTarget()
    {
        float closestDistance = range;
        Transform closestEnemy = null;

        EnemyMovement[] enemies = FindObjectsByType<EnemyMovement>(FindObjectsSortMode.None);
        foreach (EnemyMovement enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }
        return closestEnemy;
    }

    void Fire(Transform target)
    {
        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        proj.GetComponent<Projectile>().SetTarget(target);
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;
        currentHealth -= damage;
        UpdateHealthBar();
        StartCoroutine(FlashTint());
        if (currentHealth <= 0)
            StartCoroutine(Die());
    }

    IEnumerator Die()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        yield return new WaitForSeconds(0.15f);
        Destroy(gameObject);
    }

    IEnumerator FlashTint()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = Color.red;
        for (int i = 0; i < 5; i++) yield return null;
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
        yield return null;
        if (spriteRenderer != null)
            spriteRenderer.color = Color.red;
        for (int i = 0; i < 3; i++) yield return null;
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }
}