using UnityEngine;

// Sonido de muerte de una unidad aliada: elige una de las variantes al azar.
// Se agrega a los prefabs Soldier, Sharpshooter y Cannon.
public class AllyDeathSound : MonoBehaviour
{
    public AudioClip[] deathClips;              // Variantes del sonido de muerte
    [Range(0f, 1f)] public float volume = 1f;

    // Crea una fuente temporal para que el sonido no se corte
    // cuando la unidad se destruye a los 0.15 s.
    public void Play()
    {
        if (deathClips == null || deathClips.Length == 0) return;

        AudioClip clip = deathClips[Random.Range(0, deathClips.Length)];
        if (clip == null) return;

        GameObject audioGO = new GameObject("AllyDeathSound");
        audioGO.transform.position = transform.position;

        AudioSource source = audioGO.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.spatialBlend = 0f;   // 2D

        source.Play();
        Destroy(audioGO, clip.length);
    }
}
