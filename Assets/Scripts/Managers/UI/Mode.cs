using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Mode : MonoBehaviour
{
    public string currentMode = "Normal";
    public static Mode Instance;
    public TextMeshProUGUI modeText;

    [Header("Time")]

    float TimeLeft = 0f;
    float TimeElapsed = 0f;
    public float[] MaxTime = { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };

    [Header("Moves")]

    public int Moves;
    public int MovesLeft = 0;
    public int[] MaxMoves = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    void Start()
    {
        currentMode = PlayerPrefs.GetString("GameMode", "Normal");
        Instance = this;
    }

    void Update()
    {
        if (currentMode == "Normal")
        {
            modeText.enabled = false;
            return;
        }
        else
        {
            modeText.enabled = true;
        }
        if (currentMode == "TimeAttack")
        {
            modeText.text = "Time Left: " + TimeLeft.ToString("F2") + "s";
        }
        else if (currentMode == "Moves")
        {
            int currentlevel = SceneManager.GetActiveScene().buildIndex - 1;
            MovesLeft = MaxMoves[currentlevel] - Moves;
            modeText.text = "Moves Left: " + MovesLeft.ToString();
        }

    }
}
