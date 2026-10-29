using System.Collections;
using UnityEngine;

// Efecto pop-up al spawnear: escala de forma breve con overshoot suave
public class SpawnPopScale : MonoBehaviour
{
    [Range(0.5f, 1f)] public float fromScale = 0.85f;   // escala inicial relativa
    [Range(1f, 1.5f)] public float peakScale = 1.06f;   // escala máxima relativa
    public float duration = 0.3f;

    Vector3 targetScale;
    Coroutine running;

    void Start()
    {
        targetScale = transform.localScale;
        StartCoroutine(Animate());
    }

    void OnEnable()
    {
        if (targetScale == Vector3.zero)
            return; // Start todavía no corrió
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.001f, duration);
            // curva con overshoot simple: sube hasta peakScale y baja a 1
            float progress = Mathf.Clamp01(t);
            float eased;
            if (progress < 0.6f)
            {
                eased = Mathf.Lerp(0f, 1f, progress / 0.6f);      // 0 -> peak
            }
            else
            {
                eased = Mathf.Lerp(1f, 0f, (progress - 0.6f) / 0.4f); // peak -> 0
            }
            float scale = Mathf.Lerp(fromScale, peakScale, eased);
            scale = Mathf.Lerp(scale, 1f, progress); // volver al target
            transform.localScale = targetScale * scale;
            yield return null;
        }
        transform.localScale = targetScale;
    }
}