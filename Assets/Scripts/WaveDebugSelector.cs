using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Debug: llena un dropdown con "Oleada 1" ... "Oleada N" y permite saltar
// a cualquier oleada para testear. Se agrega al GameObject del Dropdown.
public class WaveDebugSelector : MonoBehaviour
{
    public WaveSpawner waveSpawner;
    public TMP_Dropdown dropdown;

    void Awake()
    {
        if (dropdown == null) dropdown = GetComponent<TMP_Dropdown>();
        if (waveSpawner == null) waveSpawner = FindAnyObjectByType<WaveSpawner>();
    }

    void Start()
    {
        if (dropdown == null || waveSpawner == null)
        {
            Debug.LogWarning("WaveDebugSelector: falta asignar el Dropdown o el WaveSpawner.");
            return;
        }

        // Genera las opciones a partir de la cantidad real de oleadas
        dropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < waveSpawner.WaveCount; i++)
            options.Add(new TMP_Dropdown.OptionData("Oleada " + (i + 1)));
        dropdown.AddOptions(options);

        dropdown.onValueChanged.AddListener(OnWaveSelected);
        waveSpawner.WaveStarted += OnWaveStarted;

        dropdown.SetValueWithoutNotify(0);
        dropdown.RefreshShownValue();
    }

    void OnDestroy()
    {
        if (dropdown != null) dropdown.onValueChanged.RemoveListener(OnWaveSelected);
        if (waveSpawner != null) waveSpawner.WaveStarted -= OnWaveStarted;
    }

    // Al elegir una opcion, salta directo a esa oleada
    void OnWaveSelected(int index)
    {
        waveSpawner.DebugStartWave(index);
    }

    // Mantiene el dropdown sincronizado cuando la oleada arranca con Espacio
    void OnWaveStarted(int index)
    {
        if (dropdown == null) return;
        dropdown.SetValueWithoutNotify(index);
        dropdown.RefreshShownValue();
    }
}
