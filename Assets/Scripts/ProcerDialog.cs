using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.UI;

public class ProcerDialog : MonoBehaviour
{
    public GameObject dialogPanel;
    public Image portraitImage;
    public TMP_Text dialogText;
    public TMP_Text nameText;
    public Button entendidoButton;
    public Sprite[] procerPortraits;
    [Range(10f, 300f)] public float charsPerSecond = 60f;

    string[] procerNamesRef = { "Cornelio Saavedra", "Bernardo de Velasco", "César Balbiani", "Cornelio Saavedra" };
    Coroutine typingRoutine;

    void Start()
    {
        if (entendidoButton != null)
            entendidoButton.onClick.AddListener(Hide);
    }

    public void Show(string message, string name)
    {
        if (dialogPanel != null)
            dialogPanel.SetActive(true);
        if (dialogText != null)
        {
            dialogText.text = message;
            dialogText.ForceMeshUpdate();
            dialogText.maxVisibleCharacters = 0;
            if (typingRoutine != null) StopCoroutine(typingRoutine);
            typingRoutine = StartCoroutine(ShowTypewriter());
        }
        if (nameText != null)
            nameText.text = name;
        if (portraitImage != null)
        {
            int idx = System.Array.IndexOf(procerNamesRef, name);
            if (idx >= 0 && procerPortraits != null && idx < procerPortraits.Length && procerPortraits[idx] != null)
            {
                portraitImage.sprite = procerPortraits[idx];
                portraitImage.enabled = true;
            }
            else if (portraitImage.sprite == null)
            {
                portraitImage.enabled = false;
            }
        }
    }

    IEnumerator ShowTypewriter()
    {
        float shown = 0f;
        int total = 0;
        if (dialogText != null && dialogText.textInfo != null)
            total = dialogText.textInfo.characterCount;

        while (shown < total)
        {
            shown = Mathf.Min(shown + charsPerSecond * Time.deltaTime, total);
            int count = Mathf.FloorToInt(shown);
            if (count != dialogText.maxVisibleCharacters)
                dialogText.maxVisibleCharacters = count;
            yield return null;
        }
        dialogText.maxVisibleCharacters = total;
    }

    public void Hide()
    {
        if (dialogPanel != null)
            dialogPanel.SetActive(false);
    }

    public bool IsOpen()
    {
        return dialogPanel != null && dialogPanel.activeSelf;
    }
}