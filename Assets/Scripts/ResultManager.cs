using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    public TMP_Text messageText;
    public TMP_Text replayButtonText;
    [TextArea(3, 6)] public string victoryMessage = "Las tropas aliadas resistieron cada ataque y lograron defender Buenos Aires.";
    [TextArea(3, 6)] public string defeatMessage = "La defensa fue superada. Reuní a tus tropas y prepará una nueva estrategia para defender Buenos Aires.";
    static bool won = true;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetResult() => won = true;

    public static void Show(bool victory)
    {
        won = victory;
        Time.timeScale = 1f;
        SceneManager.LoadScene("ResultScene");
    }

    void Start()
    {
        string title = won ? "Victoria" : "Derrota";
        messageText.text = "<b>" + title + "</b>\n\n" + (won ? victoryMessage : defeatMessage);
        replayButtonText.text = won ? "Volver a jugar" : "Reintentar";
    }

    public void PlayAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainScene");
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene");
    }
}