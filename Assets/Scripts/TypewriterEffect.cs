using System.Collections;
using TMPro;
using UnityEngine;

// Efecto máquina de escribir sobre un TextMeshProUGUI
[RequireComponent(typeof(TMP_Text))]
public class TypewriterEffect : MonoBehaviour
{
    [Range(10f, 300f)] public float charsPerSecond = 60f;

    public GameObject revealObject;   // se activa cuando el texto termina

    TMP_Text text;
    float charsShown;
    int total;

    void Awake()
    {
        text = GetComponent<TMP_Text>();
        if (revealObject != null)
            revealObject.SetActive(false);
    }

    void Start()
    {
        text.ForceMeshUpdate();
    }

    void OnEnable()
    {
        charsShown = 0f;
        total = 0;
        if (text != null)
            text.maxVisibleCharacters = 0;
        if (revealObject != null)
            revealObject.SetActive(false);
    }

    void Update()
    {
        if (text == null) return;
        total = text.textInfo != null ? text.textInfo.characterCount : 0;
        if (total <= 0) return;
        if (charsShown >= total)
        {
            if (revealObject != null && !revealObject.activeSelf)
                revealObject.SetActive(true);
            return;
        }

        charsShown = Mathf.Min(charsShown + charsPerSecond * Time.deltaTime, total);
        int count = Mathf.FloorToInt(charsShown);
        if (count != text.maxVisibleCharacters)
            text.maxVisibleCharacters = count;

        if (charsShown >= total)
        {
            text.maxVisibleCharacters = total;
            if (revealObject != null && !revealObject.activeSelf)
                revealObject.SetActive(true);
        }
    }
}