using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Sonido de disparo de una unidad aliada: un solo clip por unidad.
// Se agrega a los prefabs Soldier, Sharpshooter y Cannon y el clip se asigna en el Inspector.
public class AllyShootSound : MonoBehaviour
{
    public AudioClip shootClip;                 // Audio del disparo (uno por unidad)
    [Range(0f, 1f)] public float volume = 1f;

    AudioSource source;

    void Awake()
    {
        EnsureSource();
    }

    // Fuente propia en la unidad: no se destruye al disparar y permite superponer tiros
    void EnsureSource()
    {
        if (source != null) return;

        source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;   // 2D, igual que el sonido de muerte
    }

    public void Play()
    {
        if (shootClip == null) return;

        EnsureSource();
        source.PlayOneShot(shootClip, volume);
    }

#if UNITY_EDITOR
    // Gizmo en la vista de escena: esfera verde si tiene clip asignado, roja si falta.
    // Permite identificar rapidamente que unidades tienen sonido de disparo.
    void OnDrawGizmos()
    {
        Gizmos.color = shootClip != null ? new Color(0.2f, 0.9f, 0.3f, 0.9f) : new Color(0.9f, 0.2f, 0.2f, 0.9f);
        Gizmos.DrawWireSphere(transform.position, 0.6f);
    }

    void OnDrawGizmosSelected()
    {
        if (shootClip != null)
        {
            Handles.color = Color.white;
            Handles.Label(transform.position + Vector3.up * 0.9f, "Disparo: " + shootClip.name);
        }
        else
        {
            Handles.color = Color.red;
            Handles.Label(transform.position + Vector3.up * 0.9f, "Sin sonido de disparo");
        }
    }
#endif
}